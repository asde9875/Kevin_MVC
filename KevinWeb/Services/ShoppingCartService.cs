using Kevin.Models.Entities;
using Kevin.DataAccess.DAO;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartDao _shoppingCartDao;

        public ShoppingCartService(IShoppingCartDao shoppingCartDao)
        {
            _shoppingCartDao = shoppingCartDao;
        }

        public void AddShoppingCart(ShoppingCartEntity shoppingCart)
        {
            _shoppingCartDao.AddShoppingCart(shoppingCart);
        }

        public void UpdateShoppingCart(ShoppingCartEntity shoppingCart)
        {
            _shoppingCartDao.UpdateShoppingCart(shoppingCart);
        }

        public void DeleteShoppingCart(int id)
        {
            _shoppingCartDao.DeleteShoppingCart(id);
        }

        // Remove the range of shopping cart entities
        public void RemoveShoppingCartRange(List<ShoppingCartEntity> shoppingCart)
        {
            _shoppingCartDao.RemoveShoppingCartRange(shoppingCart);
        }

        public IEnumerable<ShoppingCartEntity> GetAllShoppingCarts()
        {
            return _shoppingCartDao.GetAllShoppingCarts();
        }

        public ShoppingCartEntity GetShoppingCartById(int id)
        {
            return _shoppingCartDao.GetShoppingCartById(id);
        }

        public IEnumerable<ApplicationUser> GetAllApplicationUsersName()
        {
            return _shoppingCartDao.GetAllApplicationUsersName();
        }

        public IEnumerable<OrderHeader> GetAllOrderHeader()
        {
            return _shoppingCartDao.GetAllOrderHeader();
        }

        public OrderHeader GetOrderHeaderById(int id)
        {
            return _shoppingCartDao.GetOrderHeaderById(id);
        }

        public IEnumerable<OrderDetail> GetOrderDetailById(int id)
        {
            return _shoppingCartDao.GetOrderDetailById(id);
        }


        public int AddOrderHeader(OrderHeader orderHeader)
        {
            return _shoppingCartDao.AddOrderHeader(orderHeader);
        }

        public void UpdateOrderHeader(OrderHeader orderHeader)
        {
            _shoppingCartDao.UpdateOrderHeader(orderHeader);
        }

        public void UpdateOrderHeaderStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            _shoppingCartDao.UpdateOrderHeaderStatus(id, orderStatus, paymentStatus);
        }

        public void UpdateOrderHeaderStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            _shoppingCartDao.UpdateOrderHeaderStripePaymentID(id, sessionId, paymentIntentId);
        }


        public void DeleteOrderHeader(int id)
        {
            _shoppingCartDao.DeleteOrderHeader(id);
        }

        public void UpdateOrderHeaderStripeOrder(OrderHeader orderHeader)
        {
            _shoppingCartDao.UpdateOrderHeaderStripeOrder(orderHeader);
        }


        public void AddOrderDetail(OrderDetail orderDetail)
        {
            _shoppingCartDao.AddOrderDetail(orderDetail);
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _shoppingCartDao.UpdateOrderDetail(orderDetail);
        }
    }
}
