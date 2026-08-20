using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Domain.Entities;
using Korp.Billing.Api.Domain.Enums;
using Korp.Billing.Api.Features.Invoices.GetById;
using Korp.Billing.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Korp.Billing.Tests.Features.Invoices.GetById
{
    [Collection("BillingDatabase")]
    public class GetInvoiceByIdHandlerTests : IAsyncLifetime
    {
        private readonly BillingDatabaseFixture _fixture;

        public GetInvoiceByIdHandlerTests(
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
        public async Task HandleAsync_ShouldReturnInvoice_WhenInvoiceExists()
        {
            await using var dbContext = _fixture.CreateDbContext();

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
                        ProductId = Guid.NewGuid(),
                        ProductCode = "PROD-001",
                        ProductDescription = "Notebook Dell",
                        Quantity = 2
                    }
                ]
            };

            dbContext.Invoices.Add(invoice);

            await dbContext.SaveChangesAsync();

            var handler = new GetInvoiceByIdHandler(dbContext);

            var response = await handler.HandleAsync(invoice.Id);

            Assert.Equal(invoice.Id, response.Id);
            Assert.Equal(invoice.Number, response.Number);
            Assert.Equal(InvoiceStatus.Open, response.Status);

            Assert.Single(response.Items);

            var item = response.Items.Single();

            Assert.Equal("PROD-001", item.ProductCode);
            Assert.Equal("Notebook Dell", item.ProductDescription);
            Assert.Equal(2, item.Quantity);
        }

        [Fact]
        public async Task HandleAsync_ShouldThrowNotFoundException_WhenInvoiceDoesNotExist()
        {
            await using var dbContext = _fixture.CreateDbContext();

            var handler = new GetInvoiceByIdHandler(dbContext);

            var invoiceId = Guid.NewGuid();

            await Assert.ThrowsAsync<NotFoundException>(
                () => handler.HandleAsync(invoiceId));
        }
    }
}