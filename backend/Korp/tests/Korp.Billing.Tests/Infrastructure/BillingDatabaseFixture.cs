using Korp.Billing.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Korp.Billing.Tests.Infrastructure
{
    public class BillingDatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer =
            new PostgreSqlBuilder()
                .WithImage("postgres:18.1-alpine")
                .WithDatabase("korp_billing_tests")
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

        public BillingDbContext CreateDbContext()
        {
            var options =
                new DbContextOptionsBuilder<BillingDbContext>()
                    .UseNpgsql(ConnectionString)
                    .Options;

            return new BillingDbContext(options);
        }

        public async Task ResetDatabaseAsync()
        {
            await using var dbContext = CreateDbContext();

            await dbContext.InvoiceItems.ExecuteDeleteAsync();
            await dbContext.Invoices.ExecuteDeleteAsync();
        }
    }
}