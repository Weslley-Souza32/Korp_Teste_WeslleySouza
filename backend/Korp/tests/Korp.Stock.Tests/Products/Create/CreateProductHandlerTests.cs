using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Domain.Entities;
using Korp.Stock.Api.Features.Products.Create;
using Korp.Stock.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Tests.Products.Create
{
    [Collection("StockDatabase")]
    public class CreateProductHandlerTests : IAsyncLifetime
    {
        private readonly StockDatabaseFixture _fixture;

        public CreateProductHandlerTests(
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
        public async Task HandleAsync_WhenRequestIsValid_ShouldCreateProduct()
        {
            // Arrange
            await using var dbContext = _fixture.CreateDbContext();

            var handler = new CreateProductHandler(dbContext);

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

            var product = await dbContext.Products
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
            await using var dbContext = _fixture.CreateDbContext();

            var existingProduct = new Product
            {
                Id = Guid.NewGuid(),
                Code = "PROD-001",
                Description = "Notebook Dell",
                StockQuantity = 10,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            dbContext.Products.Add(existingProduct);

            await dbContext.SaveChangesAsync();

            var handler = new CreateProductHandler(dbContext);

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

            var productCount = await dbContext.Products.CountAsync();

            Assert.Equal(1, productCount);
        }
    }
}