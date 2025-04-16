using Kevin.Models.Entities;
using System.Collections.Generic;

namespace KevinWeb.Services
{
    public interface IShoppingCartService
    {
        void AddShoppingCart(ShoppingCartEntity shoppingCart);
        void UpdateShoppingCart(ShoppingCartEntity shoppingCart);
        void DeleteShoppingCart(int id);

        // Remove the range of shopping cart entities
        void RemoveShoppingCartRange(List<ShoppingCartEntity> shoppingCart);
        IEnumerable<ShoppingCartEntity> GetAllShoppingCarts();
        ShoppingCartEntity GetShoppingCartById(int id);
        IEnumerable<ApplicationUser> GetAllApplicationUsersName();

        IEnumerable<OrderHeader> GetAllOrderHeader();
        OrderHeader GetOrderHeaderById(int id);
        IEnumerable<OrderDetail> GetOrderDetailById(int id);

        int AddOrderHeader(OrderHeader orderHeader);
        void UpdateOrderHeader(OrderHeader orderHeader);
        void UpdateOrderHeaderStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateOrderHeaderStripePaymentID(int id, string sessionId, string paymentIntentId);

        void DeleteOrderHeader(int id);
        void UpdateOrderHeaderStripeOrder(OrderHeader orderHeader);

        void AddOrderDetail(OrderDetail orderDetail);
        void UpdateOrderDetail(OrderDetail orderDetail);
    }
}
