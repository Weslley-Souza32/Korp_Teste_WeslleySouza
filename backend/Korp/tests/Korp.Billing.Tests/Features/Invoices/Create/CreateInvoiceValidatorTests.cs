using Korp.Billing.Api.Features.Invoices.Create;

namespace Korp.Billing.Tests.Features.Invoices.Create
{
    public class CreateInvoiceValidatorTests
    {
        private readonly CreateInvoiceValidator _validator = new();

        [Fact]
        public async Task ValidateAsync_ShouldBeValid_WhenRequestIsValid()
        {
            var request = new CreateInvoiceRequest
            {
                Items =
                [
                    new CreateInvoiceItemRequest
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 2
                    }
                ]
            };

            var result = await _validator.ValidateAsync(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_ShouldBeInvalid_WhenItemsIsEmpty()
        {
            var request = new CreateInvoiceRequest
            {
                Items = []
            };

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_ShouldBeInvalid_WhenQuantityIsZero()
        {
            var request = new CreateInvoiceRequest
            {
                Items =
                [
                    new CreateInvoiceItemRequest
                    {
                        ProductId = Guid.NewGuid(),
                        Quantity = 0
                    }
                ]
            };

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_ShouldBeInvalid_WhenProductsAreDuplicated()
        {
            var productId = Guid.NewGuid();

            var request = new CreateInvoiceRequest
            {
                Items =
                [
                    new CreateInvoiceItemRequest
                    {
                        ProductId = productId,
                        Quantity = 1
                    },
                    new CreateInvoiceItemRequest
                    {
                        ProductId = productId,
                        Quantity = 2
                    }
                ]
            };

            var result = await _validator.ValidateAsync(request);

            Assert.False(result.IsValid);
        }
    }
}