using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Domain.Entities;
using Korp.Stock.Api.Features.Products.GetById;
using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Tests.Products.GetById
{
    public class GetProductByIdHandlerTests
    {
        private static async Task<TestDatabase> CreateDatabaseAsync()
        {
            var connection = new SqliteConnection("DataSource=:memory:");

            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<StockDbContext>()
                .UseSqlite(connection)
                .Options;

            var dbContext = new StockDbContext(options);

            await dbContext.Database.EnsureCreatedAsync();

            return new TestDatabase(connection, dbContext);
        }

        private sealed class TestDatabase : IAsyncDisposable
        {
            public SqliteConnection Connection { get; }
            public StockDbContext DbContext { get; }

            public TestDatabase(
                SqliteConnection connection,
                StockDbContext dbContext)
            {
                Connection = connection;
                DbContext = dbContext;
            }

            public async ValueTask DisposeAsync()
            {
                await DbContext.DisposeAsync();
                await Connection.DisposeAsync();
            }
        }

        [Fact]
        public async Task HandleAsync_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            await using var database = await CreateDatabaseAsync();

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = "PROD-001",
                Description = "Notebook Dell",
                StockQuantity = 10,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            database.DbContext.Products.Add(product);

            await database.DbContext.SaveChangesAsync();

            var handler = new GetProductByIdHandler(database.DbContext);

            // Act
            var response = await handler.HandleAsync(product.Id);

            // Assert
            Assert.Equal(product.Id, response.Id);
            Assert.Equal(product.Code, response.Code);
            Assert.Equal(product.Description, response.Description);
            Assert.Equal(product.StockQuantity, response.StockQuantity);
        }

        [Fact]
        public async Task HandleAsync_WhenProductDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            await using var database = await CreateDatabaseAsync();

            var handler = new GetProductByIdHandler(database.DbContext);

            var productId = Guid.NewGuid();

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => handler.HandleAsync(productId));

            // Assert
            Assert.Contains(productId.ToString(), exception.Message);
        }
    }
}