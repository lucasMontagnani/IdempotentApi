using IdempotentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IdempotentApi.Data
{
    public class IdempotentApiDbContext : DbContext
    {
        public IdempotentApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}
