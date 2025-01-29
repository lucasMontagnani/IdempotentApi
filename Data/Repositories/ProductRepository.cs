using IdempotentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IdempotentApi.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        protected IdempotentApiDbContext _context;

        public ProductRepository(IdempotentApiDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetProductsByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
