using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Domain.Entities;
using Korp.Stock.Api.Features.Products.GetById;
using Korp.Stock.Api.Infrastructure.Persistence;
using Korp.Stock.Tests.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Tests.Products.GetById
{
    [Collection("StockDatabase")]
    public class GetProductByIdHandlerTests : IAsyncLifetime
    {
        private readonly StockDatabaseFixture _fixture;

        public GetProductByIdHandlerTests(
            StockDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task HandleAsync_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            await using var dbContext = _fixture.CreateDbContext();

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = "PROD-001",
                Description = "Notebook Dell",
                StockQuantity = 10,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync();

            var handler = new GetProductByIdHandler(dbContext);

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
            await using var dbContext = _fixture.CreateDbContext();

            var handler = new GetProductByIdHandler(dbContext);

            var productId = Guid.NewGuid();

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                () => handler.HandleAsync(productId));

            // Assert
            Assert.Contains(productId.ToString(), exception.Message);
        }
    }
}