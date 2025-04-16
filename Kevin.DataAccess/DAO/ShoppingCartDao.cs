using Kevin.Models.Entities;
using Kevin.DataAccess.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Kevin.DataAccess.DAO
{
    public class ShoppingCartDao : IShoppingCartDao
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartDao(ApplicationDbContext db)
        {
            _db = db;
            //_db.Products.Include(u => u.Category).Include(u =>u.CategoryId);
        }

        // Add a product using raw SQL and SaveChanges with transaction
        public void AddShoppingCart(ShoppingCartEntity shoppingCart)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                // Raw SQL for inserting the ShoppingCarts
                string sql = "INSERT INTO ShoppingCarts (ProductId, Count, ApplicationUserId) " +
                             "VALUES (@ProductId, @Count, @ApplicationUserId)";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@ProductId", shoppingCart.ProductId),
                    new SqlParameter("@Count", shoppingCart.Count),
                    new SqlParameter("@ApplicationUserId", shoppingCart.ApplicationUserId));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Update a product using raw SQL and SaveChanges with transaction
        public void UpdateShoppingCart(ShoppingCartEntity shoppingCart)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = "UPDATE ShoppingCarts SET " +
                             "ProductId = @ProductId, " +
                             "Count = @Count, " +
                             "ApplicationUserId = @ApplicationUserId " +
                             "WHERE Id = @Id";
                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@ProductId", shoppingCart.ProductId),
                    new SqlParameter("@Count", shoppingCart.Count),
                    new SqlParameter("@ApplicationUserId", shoppingCart.ApplicationUserId),
                    new SqlParameter("@Id", shoppingCart.Id));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Delete a product by ID using raw SQL and SaveChanges with transaction
        public void DeleteShoppingCart(int id)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = "DELETE FROM ShoppingCarts WHERE Id = @Id";
                _db.Database.ExecuteSqlRaw(sql, new SqlParameter("@Id", id));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveShoppingCartRange(List<ShoppingCartEntity> shoppingCartList)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                // Remove the range of shopping cart entities
                _db.ShoppingCarts.RemoveRange(shoppingCartList);

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        //Get Product name -->  _db.ShoppingCarts.Include(u => u.Product).Include(u =>u.ProductId);
        public IEnumerable<ShoppingCartEntity> GetAllShoppingCarts()
        {

            string sql = "SELECT * FROM ShoppingCarts";
            var shoppingCarts = _db.ShoppingCarts.FromSqlRaw(sql).ToList();

            // Load related User and Product data for each ShoppingCart
            foreach (var shoppingCart in shoppingCarts)
            {
                _db.Entry(shoppingCart).Reference(p => p.Product).Load();
                _db.Entry(shoppingCart).Reference(p => p.ApplicationUser).Load();
            }

            return shoppingCarts;
        }

        public ShoppingCartEntity GetShoppingCartById(int id)
        {
            string sql = "SELECT * FROM ShoppingCarts WHERE Id = @Id";
            return _db.ShoppingCarts.FromSqlRaw(sql,
                new SqlParameter("@Id", id)).FirstOrDefault()!;
        }

        public IEnumerable<ApplicationUser> GetAllApplicationUsersName()
        {
            string sql = "SELECT * FROM AspNetUsers";
            return _db.ApplicationUsers.FromSqlRaw(sql).ToList();
        }

        public IEnumerable<OrderHeader> GetAllOrderHeader()
        {
            string sql = "SELECT * FROM OrderHeaders";
            var orderHeaders = _db.OrderHeaders.FromSqlRaw(sql).ToList();

            // Load related User and Product data for each ShoppingCart
            foreach (var order in orderHeaders)
            {
                _db.Entry(order).Reference(p => p.ApplicationUser).Load();
            }

            return orderHeaders;

            //using (var connection = _db.Database.GetDbConnection())
            //{
            //    connection.Open();

            //    using (var command = connection.CreateCommand())
            //    {
            //        command.CommandText = @"
            //                SELECT 
            //                    o.Id AS OrderId, o.OrderDate, o.ShippingDate, o.OrderTotal, 
            //                    o.OrderStatus, o.PaymentStatus, o.TrackingNumber, o.Carrier, 
            //                    o.PhoneNumber AS OrderPhoneNumber, o.StreetAddress AS OrderStreetAddress, 
            //                    o.City AS OrderCity, o.State AS OrderState, o.PostalCode AS OrderPostalCode, 
            //                    o.Name AS OrderName, a.Id AS ApplicationUserId, a.Name AS ApplicationUserName, 
            //                    a.StreetAddress AS UserStreetAddress, a.City AS UserCity, 
            //                    a.State AS UserState, a.PostalCode AS UserPostalCode, a.Email AS UserEmail
            //                FROM 
            //                    OrderHeaders o
            //                INNER JOIN 
            //                    AspNetUsers a ON o.ApplicationUserId = a.Id";

            //        using (var reader = command.ExecuteReader())
            //        {
            //            var orderHeaders = new List<OrderHeader>();

            //            while (reader.Read())
            //            {
            //                var order = new OrderHeader
            //                {
            //                    Id = (int)reader["OrderId"],
            //                    OrderDate = (DateTime)reader["OrderDate"],
            //                    ShippingDate = (DateTime)reader["ShippingDate"],
            //                    OrderTotal = (double)reader["OrderTotal"],
            //                    OrderStatus = reader["OrderStatus"] as string,
            //                    PaymentStatus = reader["PaymentStatus"] as string,
            //                    TrackingNumber = reader["TrackingNumber"] as string,
            //                    Carrier = reader["Carrier"] as string,
            //                    PhoneNumber = reader["OrderPhoneNumber"] as string,
            //                    StreetAddress = reader["OrderStreetAddress"] as string,
            //                    City = reader["OrderCity"] as string,
            //                    State = reader["OrderState"] as string,
            //                    PostalCode = reader["OrderPostalCode"] as string,
            //                    Name = reader["OrderName"] as string,
            //                    ApplicationUserId = reader["ApplicationUserId"] as string,
            //                    ApplicationUser = new ApplicationUser
            //                    {
            //                        Id = reader["ApplicationUserId"] as string,
            //                        Name = reader["ApplicationUserName"] as string,
            //                        StreetAddress = reader["UserStreetAddress"] as string,
            //                        City = reader["UserCity"] as string,
            //                        State = reader["UserState"] as string,
            //                        PostalCode = reader["UserPostalCode"] as string,
            //                        Email = reader["UserEmail"] as string
            //                    }
            //                };

            //                orderHeaders.Add(order);
            //            }

            //            return orderHeaders;
            //        }
            //    }
            //}

        }

        public OrderHeader GetOrderHeaderById(int id)
        {
            string sql = "SELECT * FROM OrderHeaders WHERE Id = @Id";
            var orderHeader = _db.OrderHeaders.FromSqlRaw(sql, new SqlParameter("@Id", id)).FirstOrDefault();

            if (orderHeader != null)
            {
                // Load the related ApplicationUser for the order
                _db.Entry(orderHeader).Reference(p => p.ApplicationUser).Load();
            }

            return orderHeader;
        }

        public IEnumerable<OrderDetail> GetOrderDetailById(int id)
        {
            string sql = "SELECT * FROM OrderDetails WHERE OrderHeaderId = @Id";
            var orderDetail = _db.OrderDetails.FromSqlRaw(sql, new SqlParameter("@Id", id)).ToList();

            // Load related User and Product data for each ShoppingCart
            foreach (var details in orderDetail)
            {
                _db.Entry(details).Reference(p => p.Product).Load();
            }

            //if (orderDetail != null)
            //{
            //    // Load the related Product for the order detail
            //    _db.Entry(orderDetail).Reference(p => p.Product).Load();
            //}

            return orderDetail;
        }

        // Add a new OrderHeader using raw SQL and SaveChanges with transaction
        public int AddOrderHeader(OrderHeader orderHeader)
        {
            try
            {
                // Handle default dates
                orderHeader.ShippingDate = orderHeader.ShippingDate == DateTime.MinValue ? new DateTime(1900, 1, 1) : orderHeader.ShippingDate;
                orderHeader.PaymentDate = orderHeader.PaymentDate == DateTime.MinValue ? new DateTime(1900, 1, 1) : orderHeader.PaymentDate;
                orderHeader.PaymentDueDate = orderHeader.PaymentDueDate == DateOnly.MinValue ? new DateOnly(1900, 1, 1) : orderHeader.PaymentDueDate;

                // Add the entity to the DbContext
                _db.OrderHeaders.Add(orderHeader);

                // Save changes and retrieve the generated Id
                _db.SaveChanges();

                return orderHeader.Id; // EF Core will populate this after SaveChanges
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding OrderHeader: {ex.Message}");
                throw;
            }
        }

        public void UpdateOrderHeader(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);

            // Save changes and retrieve the generated Id
            _db.SaveChanges();
        }

        // Update an existing OrderHeader using raw SQL and SaveChanges with transaction
        public void UpdateOrderHeaderStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                if(paymentStatus == null)
                {
                    //For updating the status when AdminUser change the Click the Start Processing button
                    string sql = @"UPDATE OrderHeaders SET 
                       OrderStatus = @OrderStatus
                       WHERE Id = @Id";

                    _db.Database.ExecuteSqlRaw(sql,
                        new SqlParameter("@OrderStatus", orderStatus),
                        new SqlParameter("@Id", id));
                }
                else
                {
                    //For updating the status when Users Click the Order Confirmation button
                    string sql = @"UPDATE OrderHeaders SET 
                       OrderStatus = @OrderStatus,
                       PaymentStatus = @PaymentStatus
                       WHERE Id = @Id";

                    _db.Database.ExecuteSqlRaw(sql,
                        new SqlParameter("@OrderStatus", orderStatus),
                        new SqlParameter("@PaymentStatus", paymentStatus),
                        new SqlParameter("@Id", id));
                }


                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Update an existing OrderHeader using raw SQL and SaveChanges with transaction
        public void UpdateOrderHeaderStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = @"UPDATE OrderHeaders SET 
                       SessionId = @SessionId,
                       PaymentIntentId = @PaymentIntentId,
                       PaymentDate = @PaymentDate
                       WHERE Id = @Id";

                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@SessionId", sessionId),
                    new SqlParameter("@PaymentIntentId", paymentIntentId ?? (object)DBNull.Value),
                    new SqlParameter("@PaymentDate", DateTime.Now),
                    new SqlParameter("@Id", id));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        //This method deletes an OrderHeader record by its id.
        public void DeleteOrderHeader(int id)
        {
            // Fetch the order header by id
            var orderHeader = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);

            if (orderHeader != null)
            {
                _db.OrderHeaders.Remove(orderHeader);
                _db.SaveChanges();
            }
            else
            {
                throw new Exception("Order Header not found.");
            }
        }

        //This method updates the OrderHeader record's Stripe-related fields.
        public void UpdateOrderHeaderStripeOrder(OrderHeader orderHeader)
        {
            // Fetch the order header by id
            var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == orderHeader.Id);

            if (orderHeaderFromDb != null)
            {
                _db.OrderHeaders.Update(orderHeaderFromDb);
                _db.SaveChanges();
            }
            else
            {
                throw new Exception("Order Header not found.");
            }
        }


        // Add a new OrderDetail using raw SQL and SaveChanges with transaction
        public void AddOrderDetail(OrderDetail orderDetail)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                // Raw SQL for inserting the OrderDetail
                string sql = @"INSERT INTO OrderDetails 
                       (OrderHeaderId, ProductId, Count, Price) 
                       VALUES 
                       (@OrderHeaderId, @ProductId, @Count, @Price)";

                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@OrderHeaderId", orderDetail.OrderHeaderId),
                    new SqlParameter("@ProductId", orderDetail.ProductId),
                    new SqlParameter("@Count", orderDetail.Count),
                    new SqlParameter("@Price", orderDetail.Price));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Update an existing OrderDetail using raw SQL and SaveChanges with transaction
        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            using var transaction = _db.Database.BeginTransaction(); // Start a transaction
            try
            {
                string sql = @"UPDATE OrderDetails SET 
                       OrderHeaderId = @OrderHeaderId, 
                       ProductId = @ProductId, 
                       Count = @Count, 
                       Price = @Price 
                       WHERE Id = @Id";

                _db.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@OrderHeaderId", orderDetail.OrderHeaderId),
                    new SqlParameter("@ProductId", orderDetail.ProductId),
                    new SqlParameter("@Count", orderDetail.Count),
                    new SqlParameter("@Price", orderDetail.Price),
                    new SqlParameter("@Id", orderDetail.Id));

                // Save tracked changes (optional, if EF Core is used elsewhere)
                _db.SaveChanges();

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Roll back the transaction in case of any error
                transaction.Rollback();
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
