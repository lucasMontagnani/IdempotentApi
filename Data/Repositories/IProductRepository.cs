using IdempotentApi.Models;

namespace IdempotentApi.Data.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetProductsByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> CreateProductAsync(Product product);
    }
}
