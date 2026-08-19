using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Domain.Enums;
using Korp.Billing.Api.Infrastructure.Clients.Stock;
using Korp.Billing.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Billing.Api.Features.Invoices.Print
{
    public class PrintInvoiceHandler
    {
        private readonly BillingDbContext _dbContext;
        private readonly IStockServiceClient _stockServiceClient;

        public PrintInvoiceHandler(
            BillingDbContext dbContext,
            IStockServiceClient stockServiceClient)
        {
            _dbContext = dbContext;
            _stockServiceClient = stockServiceClient;
        }

        public async Task<PrintInvoiceResponse> HandleAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            var invoice = await _dbContext.Invoices
                .Include(invoice => invoice.Items)
                .FirstOrDefaultAsync(
                    invoice => invoice.Id == invoiceId,
                    cancellationToken);

            if (invoice is null)
            {
                throw new NotFoundException(
                    $"Invoice with id '{invoiceId}' was not found.");
            }

            if (invoice.Status != InvoiceStatus.Open)
            {
                throw new ConflictException(
                    $"Invoice number '{invoice.Number}' is already closed.");
            }

            var debitRequest = new DebitStockRequest
            {
                InvoiceId = invoice.Id,
                Items = invoice.Items
                    .Select(item => new DebitStockItemRequest
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };

            await _stockServiceClient.DebitStockAsync(
                debitRequest,
                cancellationToken);

            invoice.Status = InvoiceStatus.Closed;
            invoice.ClosedAt = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new PrintInvoiceResponse
            {
                Id = invoice.Id,
                Number = invoice.Number,
                Status = invoice.Status,
                ClosedAt = invoice.ClosedAt
            };
        }
    }
}