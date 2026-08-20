using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Domain.Enums;
using Korp.Billing.Api.Features.Invoices.Create;
using Korp.Billing.Api.Infrastructure.Clients.Stock;
using Korp.Billing.Api.Infrastructure.Persistence;
using Korp.Billing.Tests.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Korp.Billing.Tests.Features.Invoices.Create
{
    [Collection("BillingDatabase")]
    public class CreateInvoiceHandlerTests : IAsyncLifetime
    {
        private readonly BillingDatabaseFixture _fixture;

        public CreateInvoiceHandlerTests(
            BillingDatabaseFixture fixture)
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
        public async Task HandleAsync_ShouldCreateInvoice_WhenRequestIsValid()
        {
            await using var dbContext = _fixture.CreateDbContext();

            var productId = Guid.NewGuid();

            var stockServiceClient = new Mock<IStockServiceClient>();

            stockServiceClient
                .Setup(client => client.GetProductByIdAsync(
                    productId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StockProductResponse
                {
                    Id = productId,
                    Code = "PROD-001",
                    Description = "Notebook Dell",
                    StockQuantity = 10
                });

            var handler = new CreateInvoiceHandler(
                dbContext,
                stockServiceClient.Object);

            var request = new CreateInvoiceRequest
            {
                Items =
                [
                    new CreateInvoiceItemRequest
                    {
                        ProductId = productId,
                        Quantity = 2
                    }
                ]
            };

            var response = await handler.HandleAsync(request);

            Assert.NotEqual(Guid.Empty, response.Id);
            Assert.Equal(InvoiceStatus.Open, response.Status);
            Assert.Single(response.Items);

            var invoice = await dbContext.Invoices
                .Include(invoice => invoice.Items)
                .SingleAsync();

            Assert.Equal(InvoiceStatus.Open, invoice.Status);
            Assert.Single(invoice.Items);

            var item = invoice.Items.Single();

            Assert.Equal(productId, item.ProductId);
            Assert.Equal("PROD-001", item.ProductCode);
            Assert.Equal("Notebook Dell", item.ProductDescription);
            Assert.Equal(2, item.Quantity);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            await using var dbContext = _fixture.CreateDbContext();

            var productId = Guid.NewGuid();

            var stockServiceClient = new Mock<IStockServiceClient>();

            stockServiceClient
                .Setup(client => client.GetProductByIdAsync(
                    productId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((StockProductResponse?)null);

            var handler = new CreateInvoiceHandler(
                dbContext,
                stockServiceClient.Object);

            var request = new CreateInvoiceRequest
            {
                Items =
                [
                    new CreateInvoiceItemRequest
            {
                ProductId = productId,
                Quantity = 2
            }
                ]
            };

            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.HandleAsync(request));

            var invoiceCount = await dbContext.Invoices.CountAsync();

            Assert.Equal(0, invoiceCount);
        }
    }
}