using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Domain.Entities;
using Korp.Stock.Api.Features.Products.Create;
using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Tests.Products.Create
{
    public class CreateProductHandlerTests
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
        public async Task HandleAsync_WhenRequestIsValid_ShouldCreateProduct()
        {
            // Arrange
            await using var database = await CreateDatabaseAsync();

            var handler = new CreateProductHandler(database.DbContext);

            var request = new CreateProductRequest
            {
                Code = "PROD-001",
                Description = "Notebook Dell",
                StockQuantity = 10
            };

            // Act
            var response = await handler.HandleAsync(request);

            // Assert
            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal(request.Code, response.Code);
            Assert.Equal(request.Description, response.Description);
            Assert.Equal(request.StockQuantity, response.StockQuantity);

            var product = await database.DbContext.Products
                .AsNoTracking()
                .SingleAsync();

            Assert.Equal(response.Id, product.Id);
            Assert.Equal(request.Code, product.Code);
            Assert.Equal(request.Description, product.Description);
            Assert.Equal(request.StockQuantity, product.StockQuantity);
        }

        [Fact]
        public async Task HandleAsync_WhenCodeAlreadyExists_ShouldThrowConflictException()
        {
            // Arrange
            await using var database = await CreateDatabaseAsync();

            var existingProduct = new Product
            {
                Id = Guid.NewGuid(),
                Code = "PROD-001",
                Description = "Notebook Dell",
                StockQuantity = 10,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            database.DbContext.Products.Add(existingProduct);

            await database.DbContext.SaveChangesAsync();

            var handler = new CreateProductHandler(database.DbContext);

            var request = new CreateProductRequest
            {
                Code = "PROD-001",
                Description = "Outro Produto",
                StockQuantity = 5
            };

            // Act
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => handler.HandleAsync(request));

            // Assert
            Assert.Contains("PROD-001", exception.Message);

            var productCount = await database.DbContext.Products.CountAsync();

            Assert.Equal(1, productCount);
        }
    }
}