using System.Data;
using System.Linq;
using System.Security.Claims;
using GoogleReCaptcha.V3.Interface;
using Kevin.Models.Entities;
using Kevin.Utility;
using KevinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace KevinWeb.Controllers
{
    [Authorize(Roles = SD.Role_Admin)] //Avoid someone using URL to access the Content Management
    public class OrderController : Controller
    {
        [BindProperty]
        public OrderVM orderVM { get; set; }
        private readonly IShoppingCartService _ShoppingCartService;

        public OrderController(IShoppingCartService ShoppingCartService)
        {
            _ShoppingCartService = ShoppingCartService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            orderVM = new()
            {
                OrderHeader = _ShoppingCartService.GetOrderHeaderById(orderId),
                OrderDetails = _ShoppingCartService.GetOrderDetailById(orderId)
            };

            return View(orderVM);
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _ShoppingCartService.GetAllOrderHeader().Where(u => u.Id == orderVM.OrderHeader.Id);
            foreach (var orderHeader in orderHeaderFromDb)
            {
                orderHeader.Name = orderVM.OrderHeader.Name;
                orderHeader.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
                orderHeader.StreetAddress = orderVM.OrderHeader.StreetAddress;
                orderHeader.City = orderVM.OrderHeader.City;
                orderHeader.State = orderVM.OrderHeader.State;
                orderHeader.PostalCode = orderVM.OrderHeader.PostalCode;

                if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
                {
                    orderHeader.Carrier = orderVM.OrderHeader.Carrier;
                }

                if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
                {
                    orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                }

                _ShoppingCartService.UpdateOrderHeader(orderHeader);

                TempData["Success"] = "Order Details Updated Successfully.";

            }

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.First().Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _ShoppingCartService.UpdateOrderHeaderStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeaders = _ShoppingCartService.GetAllOrderHeader().Where(u => u.Id == orderVM.OrderHeader.Id).ToList();

            foreach (var orderHeader in orderHeaders)
            {
                orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                orderHeader.Carrier = orderVM.OrderHeader.Carrier;
                orderHeader.OrderStatus = SD.StatusShipped;
                orderHeader.ShippingDate = DateTime.Now;

                if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
                {
                    orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
                }
            }


            _ShoppingCartService.UpdateOrderHeaderStripeOrder(orderHeaders.First());

            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeaders = _ShoppingCartService.GetAllOrderHeader().Where(u => u.Id == orderVM.OrderHeader.Id).ToList();
            foreach (var orderHeader in orderHeaders)
            {
                if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
                {
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId
                    };

                    var service = new RefundService();
                    Refund refund = service.Create(options);

                    _ShoppingCartService.UpdateOrderHeaderStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
                }
                else
                {
                    _ShoppingCartService.UpdateOrderHeaderStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                }
            }

            TempData["Success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }


        [ActionName("Details")]
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Details_PAY_NOW(string captcha)
        {

            OrderHeader orderHeaders = _ShoppingCartService.GetOrderHeaderById(orderVM.OrderHeader.Id);
            var orderDetails = _ShoppingCartService.GetOrderDetailById(orderVM.OrderHeader.Id).ToList();

            try
            {
                // Stripe logic
                var domain = "https://localhost:7216/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                    CancelUrl = domain + $"order/details?orderId={orderVM.OrderHeader.Id}",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in orderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "aud",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                _ShoppingCartService.UpdateOrderHeaderStripePaymentID(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);

                //Response.Headers.Add("Location", session.Url);
                Response.Headers["Location"] = session.Url; // Set directly
                return new StatusCodeResult(303);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes (optional)
                Console.WriteLine(ex);

                TempData["Error"] = "An error occurred while processing the payment. Please try again.";
                return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
            }
        }

        public IActionResult PaymentConfirmation(int orderHeaderIid)
        {

            OrderHeader orderHeader = _ShoppingCartService.GetOrderHeaderById(orderHeaderIid);


            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // this is an order by company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.PaymentIntentId = session.PaymentIntentId;
                    _ShoppingCartService.UpdateOrderHeaderStripePaymentID(orderHeaderIid, session.Id, orderHeader.PaymentIntentId);
                    _ShoppingCartService.UpdateOrderHeaderStatus(orderHeaderIid, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                }
            }

            //List<ShoppingCartEntity> shoppingCartList = _ShoppingCartService.GetAllShoppingCarts()
            //                        .Where(u => u.ApplicationUserId == orderHeader.ApplicationUserId)
            //                        .ToList();


            // Remove the shopping cart items after processing
            //_ShoppingCartService.RemoveShoppingCartRange(shoppingCartList);

            return View(orderHeaderIid);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;

            if (User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _ShoppingCartService.GetAllOrderHeader().ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _ShoppingCartService.GetAllOrderHeader().Where(u=>u.ApplicationUserId==userId).ToList();
            }
            

            //This code dynamically filters objOrderHeaders based on the status value.
            //It's commonly used in APIs or controller actions to return filtered data sets.
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusPending);
                    break;

                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;

                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;

                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;

                default:
                    break;
            }


            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}
