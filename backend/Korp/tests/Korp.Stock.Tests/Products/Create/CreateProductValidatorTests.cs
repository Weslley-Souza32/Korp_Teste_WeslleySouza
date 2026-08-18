using Korp.Stock.Api.Features.Products.Create;

namespace Korp.Stock.Tests.Products.Create
{
    public class CreateProductValidatorTests
    {
        private readonly CreateProductValidator _validator;

        public CreateProductValidatorTests()
        {
            _validator = new CreateProductValidator();
        }

        [Fact]
        public async Task ValidateAsync_WhenRequestIsValid_ShouldBeValid()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Code = "P001",
                Description = "Notebook Dell",
                StockQuantity = 10
            };

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_WhenCodeIsEmpty_ShouldBeInvalid()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Code = string.Empty,
                Description = "Notebook Dell",
                StockQuantity = 10
            };

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                error => error.PropertyName == nameof(CreateProductRequest.Code));
        }

        [Fact]
        public async Task ValidateAsync_WhenDescriptionIsEmpty_ShouldBeInvalid()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Code = "PROD-001",
                Description = string.Empty,
                StockQuantity = 10
            };

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                error => error.PropertyName == nameof(CreateProductRequest.Description));
        }

        [Fact]
        public async Task ValidateAsync_WhenStockQuantityIsNegative_ShouldBeInvalid()
        {
            // Arrange
            var request = new CreateProductRequest
            {
                Code = "PROD-001",
                Description = "Notebook Dell",
                StockQuantity = -1
            };

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                error => error.PropertyName == nameof(CreateProductRequest.StockQuantity));
        }
    }
}
