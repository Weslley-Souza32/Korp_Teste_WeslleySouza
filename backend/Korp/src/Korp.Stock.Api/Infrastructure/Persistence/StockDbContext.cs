using Korp.Stock.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Api.Infrastructure.Persistence
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<StockDebitOperation> StockDebitOperations => Set<StockDebitOperation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);
        }
    }
}