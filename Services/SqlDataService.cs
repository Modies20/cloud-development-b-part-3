using Microsoft.EntityFrameworkCore;
using ABCRetailApp.Data;

namespace ABCRetailApp.Services
{
    /// <summary>
    /// Implementation of SQL Database operations using Entity Framework Core
    /// Provides full CRUD operations for Customer, Product, and Order entities
    /// Uses fully qualified names to avoid conflicts with Table Storage entities
    /// </summary>
    public class SqlDataService : ISqlDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SqlDataService> _logger;

        public SqlDataService(ApplicationDbContext context, ILogger<SqlDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Customer Operations

        public async Task<IEnumerable<Models.Customer>> GetAllCustomersAsync()
        {
            try
            {
                return await _context.Customers
                    .OrderBy(c => c.LastName)
                    .ThenBy(c => c.FirstName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                return Enumerable.Empty<Models.Customer>();
            }
        }

        public async Task<Models.Customer?> GetCustomerByIdAsync(int id)
        {
            try
            {
                return await _context.Customers
                    .Include(c => c.Orders)
                    .FirstOrDefaultAsync(c => c.CustomerId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID {CustomerId}", id);
                return null;
            }
        }

        public async Task<Models.Customer?> GetCustomerByEmailAsync(string email)
        {
            try
            {
                return await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with email {Email}", email);
                return null;
            }
        }

        public async Task<Models.Customer> AddCustomerAsync(Models.Customer customer)
        {
            try
            {
                customer.DateCreated = DateTime.UtcNow;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Customer {CustomerId} created successfully", customer.CustomerId);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding customer");
                throw;
            }
        }

        public async Task<Models.Customer?> UpdateCustomerAsync(Models.Customer customer)
        {
            try
            {
                var existing = await _context.Customers.FindAsync(customer.CustomerId);
                if (existing == null) return null;

                existing.FirstName = customer.FirstName;
                existing.LastName = customer.LastName;
                existing.Email = customer.Email;
                existing.Phone = customer.Phone;
                existing.Address = customer.Address;
                existing.LastModified = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Customer {CustomerId} updated successfully", customer.CustomerId);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", customer.CustomerId);
                return null;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null) return false;

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Customer {CustomerId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                return false;
            }
        }

        #endregion

        #region Product Operations

        public async Task<IEnumerable<Models.Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                return Enumerable.Empty<Models.Product>();
            }
        }

        public async Task<Models.Product?> GetProductByIdAsync(int id)
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Orders)
                    .FirstOrDefaultAsync(p => p.ProductId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID {ProductId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Models.Product>> GetProductsByCategoryAsync(string category)
        {
            try
            {
                return await _context.Products
                    .Where(p => p.Category == category)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products for category {Category}", category);
                return Enumerable.Empty<Models.Product>();
            }
        }

        public async Task<Models.Product> AddProductAsync(Models.Product product)
        {
            try
            {
                product.DateCreated = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product {ProductId} created successfully", product.ProductId);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product");
                throw;
            }
        }

        public async Task<Models.Product?> UpdateProductAsync(Models.Product product)
        {
            try
            {
                var existing = await _context.Products.FindAsync(product.ProductId);
                if (existing == null) return null;

                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.Category = product.Category;
                existing.StockQuantity = product.StockQuantity;
                existing.ImageUrl = product.ImageUrl;
                existing.LastModified = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Product {ProductId} updated successfully", product.ProductId);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId}", product.ProductId);
                return null;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return false;

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Product {ProductId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId}", id);
                return false;
            }
        }

        #endregion

        #region Order Operations

        public async Task<IEnumerable<Models.Order>> GetAllOrdersAsync()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Product)
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                return Enumerable.Empty<Models.Order>();
            }
        }

        public async Task<Models.Order?> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order with ID {OrderId}", id);
                return null;
            }
        }

        public async Task<IEnumerable<Models.Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Product)
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for customer {CustomerId}", customerId);
                return Enumerable.Empty<Models.Order>();
            }
        }

        public async Task<IEnumerable<Models.Order>> GetOrdersByProductIdAsync(int productId)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Where(o => o.ProductId == productId)
                    .OrderByDescending(o => o.DateCreated)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for product {ProductId}", productId);
                return Enumerable.Empty<Models.Order>();
            }
        }

        public async Task<Models.Order> AddOrderAsync(Models.Order order)
        {
            try
            {
                order.DateCreated = DateTime.UtcNow;
                order.TotalAmount = order.UnitPrice * order.Quantity;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} created successfully", order.OrderId);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order");
                throw;
            }
        }

        public async Task<Models.Order?> UpdateOrderAsync(Models.Order order)
        {
            try
            {
                var existing = await _context.Orders.FindAsync(order.OrderId);
                if (existing == null) return null;

                existing.Quantity = order.Quantity;
                existing.UnitPrice = order.UnitPrice;
                existing.TotalAmount = order.UnitPrice * order.Quantity;
                existing.Notes = order.Notes;
                existing.Status = order.Status;
                existing.LastModified = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} updated successfully", order.OrderId);
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId}", order.OrderId);
                return null;
            }
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null) return false;

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order {OrderId} deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {OrderId}", id);
                return false;
            }
        }

        #endregion

        #region Analytics/Reporting Operations

        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.Status == "Completed")
                    .SumAsync(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total revenue");
                return 0;
            }
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            try
            {
                return await _context.Orders.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting total orders");
                return 0;
            }
        }

        public async Task<IEnumerable<Models.Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            try
            {
                return await _context.Products
                    .Where(p => p.StockQuantity <= threshold)
                    .OrderBy(p => p.StockQuantity)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving low stock products");
                return Enumerable.Empty<Models.Product>();
            }
        }

        #endregion
    }
}

