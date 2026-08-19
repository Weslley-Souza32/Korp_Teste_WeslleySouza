using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Domain.Entities;
using Korp.Billing.Api.Domain.Enums;
using Korp.Billing.Api.Infrastructure.Clients.Stock;
using Korp.Billing.Api.Infrastructure.Persistence;

namespace Korp.Billing.Api.Features.Invoices.Create
{
    public class CreateInvoiceHandler
    {
        private readonly BillingDbContext _dbContext;
        private readonly IStockServiceClient _stockServiceClient;

        public CreateInvoiceHandler(BillingDbContext dbContext, IStockServiceClient stockServiceClient)
        {
            _dbContext = dbContext;
            _stockServiceClient = stockServiceClient;
        }

        public async Task<CreateInvoiceResponse> HandleAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
        {
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                Status = InvoiceStatus.Open,
                CreatedAt = DateTimeOffset.UtcNow
            };

            foreach (var requestItem in request.Items)
            {
                var product = await _stockServiceClient.GetProductByIdAsync(
                    requestItem.ProductId,
                    cancellationToken);

                if (product is null)
                {
                    throw new NotFoundException(
                        $"Product with id '{requestItem.ProductId}' was not found.");
                }

                invoice.Items.Add(new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductCode = product.Code,
                    ProductDescription = product.Description,
                    Quantity = requestItem.Quantity
                });
            }

            _dbContext.Invoices.Add(invoice);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateInvoiceResponse
            {
                Id = invoice.Id,
                Number = invoice.Number,
                Status = invoice.Status,
                CreatedAt = invoice.CreatedAt,
                Items = invoice.Items
                    .Select(item => new CreateInvoiceItemResponse
                    {
                        ProductId = item.ProductId,
                        ProductCode = item.ProductCode,
                        ProductDescription = item.ProductDescription,
                        Quantity = item.Quantity
                    })
                    .ToList()
            };
        }
    }
}