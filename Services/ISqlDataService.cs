namespace ABCRetailApp.Services
{
    /// <summary>
    /// Interface for SQL Database operations
    /// Provides CRUD operations for Customer, Product, and Order entities
    /// Uses fully qualified names to avoid conflicts with Table Storage entities
    /// </summary>
    public interface ISqlDataService
    {
        // Customer operations
        Task<IEnumerable<Models.Customer>> GetAllCustomersAsync();
        Task<Models.Customer?> GetCustomerByIdAsync(int id);
        Task<Models.Customer?> GetCustomerByEmailAsync(string email);
        Task<Models.Customer> AddCustomerAsync(Models.Customer customer);
        Task<Models.Customer?> UpdateCustomerAsync(Models.Customer customer);
        Task<bool> DeleteCustomerAsync(int id);

        // Product operations
        Task<IEnumerable<Models.Product>> GetAllProductsAsync();
        Task<Models.Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Models.Product>> GetProductsByCategoryAsync(string category);
        Task<Models.Product> AddProductAsync(Models.Product product);
        Task<Models.Product?> UpdateProductAsync(Models.Product product);
        Task<bool> DeleteProductAsync(int id);

        // Order operations
        Task<IEnumerable<Models.Order>> GetAllOrdersAsync();
        Task<Models.Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Models.Order>> GetOrdersByCustomerIdAsync(int customerId);
        Task<IEnumerable<Models.Order>> GetOrdersByProductIdAsync(int productId);
        Task<Models.Order> AddOrderAsync(Models.Order order);
        Task<Models.Order?> UpdateOrderAsync(Models.Order order);
        Task<bool> DeleteOrderAsync(int id);

        // Analytics/Reporting operations
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<IEnumerable<Models.Product>> GetLowStockProductsAsync(int threshold = 10);
    }
}

