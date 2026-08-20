using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Korp.Stock.Tests.Infrastructure
{
    public class StockDatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer =
            new PostgreSqlBuilder()
                .WithImage("postgres:18.1-alpine")
                .WithDatabase("korp_stock_tests")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

        public string ConnectionString =>
            _postgresContainer.GetConnectionString();

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();

            await using var dbContext = CreateDbContext();

            await dbContext.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
        }

        public StockDbContext CreateDbContext()
        {
            var options =
                new DbContextOptionsBuilder<StockDbContext>()
                    .UseNpgsql(ConnectionString)
                    .Options;

            return new StockDbContext(options);
        }

        public async Task ResetDatabaseAsync()
        {
            await using var dbContext = CreateDbContext();

            await dbContext.StockDebitOperations
                .ExecuteDeleteAsync();

            await dbContext.Products
                .ExecuteDeleteAsync();
        }
    }
}