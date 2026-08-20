using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Domain.Entities;
using Korp.Billing.Api.Domain.Enums;
using Korp.Billing.Api.Features.Invoices.Print;
using Korp.Billing.Api.Infrastructure.Clients.Stock;
using Korp.Billing.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Korp.Billing.Tests.Features.Invoices.Print
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
        public async Task HandleAsync_ShouldCloseInvoice_WhenStockDebitSucceeds()
        {
            await using var dbContext = _fixture.CreateDbContext();

            var productId = Guid.NewGuid();

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Status = InvoiceStatus.Open,
                CreatedAt = DateTimeOffset.UtcNow,
                Items =
                [
                    new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = productId,
                        ProductCode = "PROD-001",
                        ProductDescription = "Notebook Dell",
                        Quantity = 2
                    }
                ]
            };

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            var stockServiceClient = new Mock<IStockServiceClient>();

            stockServiceClient
                .Setup(client => client.DebitStockAsync(
                    It.IsAny<DebitStockRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DebitStockResponse
                {
                    Success = true
                });

            var handler = new PrintInvoiceHandler(
                dbContext,
                stockServiceClient.Object);

            var response = await handler.HandleAsync(invoice.Id);

            Assert.Equal(InvoiceStatus.Closed, response.Status);
            Assert.NotNull(response.ClosedAt);

            var persistedInvoice = await dbContext.Invoices
                .SingleAsync(current => current.Id == invoice.Id);

            Assert.Equal(InvoiceStatus.Closed, persistedInvoice.Status);
            Assert.NotNull(persistedInvoice.ClosedAt);

            stockServiceClient.Verify(
                client => client.DebitStockAsync(
                    It.Is<DebitStockRequest>(
                        request =>
                            request.InvoiceId == invoice.Id &&
                            request.Items.Count == 1 &&
                            request.Items[0].ProductId == productId &&
                            request.Items[0].Quantity == 2),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowConflictException_WhenInvoiceIsAlreadyClosed()
        {
            await using var dbContext = _fixture.CreateDbContext();

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Status = InvoiceStatus.Closed,
                CreatedAt = DateTimeOffset.UtcNow,
                ClosedAt = DateTimeOffset.UtcNow,
                Items = []
            };

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            var stockServiceClient = new Mock<IStockServiceClient>();

            var handler = new PrintInvoiceHandler(
                dbContext,
                stockServiceClient.Object);

            await Assert.ThrowsAsync<ConflictException>(
                () => handler.HandleAsync(invoice.Id));

            stockServiceClient.Verify(
                client => client.DebitStockAsync(
                    It.IsAny<DebitStockRequest>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleAsync_ShouldKeepInvoiceOpen_WhenStockDebitFails()
        {
            await using var dbContext = _fixture.CreateDbContext();

            var productId = Guid.NewGuid();

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Status = InvoiceStatus.Open,
                CreatedAt = DateTimeOffset.UtcNow,
                Items =
                [
                    new InvoiceItem
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                ProductCode = "PROD-001",
                ProductDescription = "Notebook Dell",
                Quantity = 2
            }
                ]
            };

            dbContext.Invoices.Add(invoice);
            await dbContext.SaveChangesAsync();

            var stockServiceClient = new Mock<IStockServiceClient>();

            stockServiceClient
                .Setup(client => client.DebitStockAsync(
                    It.IsAny<DebitStockRequest>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(
                    new ConflictException(
                        "Product 'PROD-001' has insufficient stock."));

            var handler = new PrintInvoiceHandler(
                dbContext,
                stockServiceClient.Object);

            await Assert.ThrowsAsync<ConflictException>(
                () => handler.HandleAsync(invoice.Id));

            var persistedInvoice = await dbContext.Invoices
                .SingleAsync(current => current.Id == invoice.Id);

            Assert.Equal(InvoiceStatus.Open, persistedInvoice.Status);
            Assert.Null(persistedInvoice.ClosedAt);
        }
    }
}