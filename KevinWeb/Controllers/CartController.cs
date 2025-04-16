using System.Security.Claims;
using Kevin.Models.Entities;
using Kevin.Utility;
using KevinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace KevinWeb.Controllers
{
    public class CartController : Controller
    {
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        private readonly IShoppingCartService _ShoppingCartService;

        public CartController(IShoppingCartService ShoppingCartService)
        {
            _ShoppingCartService = ShoppingCartService;
        }

        [Authorize]
        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Initialize ShoppingCartVM
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _ShoppingCartService.GetAllShoppingCarts()
                    .Where(u => u.ApplicationUserId == userId).ToList() // Ensure it's a list
            };


            // Initialize OrderHeader
            shoppingCartVM.OrderHeader = new OrderHeader
            {
                OrderTotal = 0 // Initialize OrderTotal
            };


            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(shoppingCartVM);
        }


        [HttpGet]
        public IActionResult GetCartCount()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Json(0); // Return 0 if the user is not logged in
            }

            int cartCount = _ShoppingCartService.GetAllShoppingCarts()
                                .Where(c => c.ApplicationUserId == userId)
                                .Sum(c => c.Count);

            return Json(cartCount);
        }


        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Initialize ShoppingCartVM
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _ShoppingCartService.GetAllShoppingCarts()
                    .Where(u => u.ApplicationUserId == userId).ToList() // Ensure it's a list
            };

            // Get the ApplicationUser for the current user
            var applicationUser = _ShoppingCartService.GetAllApplicationUsersName()
                .FirstOrDefault(u => u.Id == userId);

            // Initialize OrderHeader
            shoppingCartVM.OrderHeader = new OrderHeader
            {
                OrderTotal = 0, // Initialize OrderTotal
                Name = applicationUser.Name,
                PhoneNumber = applicationUser.PhoneNumber,
                StreetAddress = applicationUser.StreetAddress,
                City = applicationUser.City,
                State = applicationUser.State,
                PostalCode = applicationUser.PostalCode,
                ApplicationUser = applicationUser, // Assign the ApplicationUser
                ApplicationUserId = userId
            };


            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Initialize ShoppingCartVM
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _ShoppingCartService.GetAllShoppingCarts()
                       .Where(u => u.ApplicationUserId == userId).ToList() // Ensure it's a list
            };

            // Get the ApplicationUser for the current user
            ApplicationUser applicationUser = _ShoppingCartService.GetAllApplicationUsersName().First(u => u.Id == userId);

            // Initialize OrderHeader
            shoppingCartVM.OrderHeader = new OrderHeader
            {
                OrderTotal = 0, // Initialize OrderTotal
                OrderDate = System.DateTime.Now,
                Name = applicationUser.Name,
                PhoneNumber = applicationUser.PhoneNumber,
                StreetAddress = applicationUser.StreetAddress,
                City = applicationUser.City,
                State = applicationUser.State,
                PostalCode = applicationUser.PostalCode,
                ApplicationUser = applicationUser, // Assign the ApplicationUser
                ApplicationUserId = userId
            };



            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

            }


            int companyId = applicationUser.CompanyId ?? 0;


            if (companyId == 0)
            {
                // It is a regular customer account and we need to capture payment
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // It is a company user
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            int orderHeaderId = _ShoppingCartService.AddOrderHeader(shoppingCartVM.OrderHeader);


            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                _ShoppingCartService.AddOrderDetail(orderDetail);
            }


            if (companyId == 0)
            {
                // Stripe logic
                var domain = "https://localhost:7216/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"Cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "Cart",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in shoppingCartVM.ShoppingCartList)
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

                _ShoppingCartService.UpdateOrderHeaderStripePaymentID(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {

            OrderHeader orderHeader = _ShoppingCartService.GetOrderHeaderById(id);
                

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // this is an order by customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _ShoppingCartService.UpdateOrderHeaderStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _ShoppingCartService.UpdateOrderHeaderStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                }
            }

            List<ShoppingCartEntity> shoppingCartList = _ShoppingCartService.GetAllShoppingCarts()
                                    .Where(u => u.ApplicationUserId == orderHeader.ApplicationUserId)
                                    .ToList();


            // Remove the shopping cart items after processing
            _ShoppingCartService.RemoveShoppingCartRange(shoppingCartList);

            return View(id);
        }

        private double GetPriceBasedOnQuantity(ShoppingCartEntity shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else
                {
                    return shoppingCart.Product.Price100;
                }
            }
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _ShoppingCartService.GetShoppingCartById(cartId);
            cartFromDb.Count += 1;
            _ShoppingCartService.UpdateShoppingCart(cartFromDb);
            //return RedirectToAction("Index");
            return Json(new { success = true });
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _ShoppingCartService.GetShoppingCartById(cartId);
            if (cartFromDb.Count <= 1)
            {
                _ShoppingCartService.DeleteShoppingCart(cartId);
            }
            else
            {
                cartFromDb.Count -= 1;
                _ShoppingCartService.UpdateShoppingCart(cartFromDb);
            }
            //return RedirectToAction("Index");
            return Json(new { success = true });
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _ShoppingCartService.GetShoppingCartById(cartId);
            _ShoppingCartService.DeleteShoppingCart(cartId);
            //return RedirectToAction("Index");
            return Json(new { success = true });
        }
    }
}
