using Korp.Billing.Api.Common.Exceptions;
using Korp.Billing.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Billing.Api.Features.Invoices.GetById
{
    public class GetInvoiceByIdHandler
    {
        private readonly BillingDbContext _dbContext;

        public GetInvoiceByIdHandler(BillingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetInvoiceByIdResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invoice = await _dbContext.Invoices
                .AsNoTracking()
                .Where(invoice => invoice.Id == id)
                .Select(invoice => new GetInvoiceByIdResponse
                {
                    Id = invoice.Id,
                    Number = invoice.Number,
                    Status = invoice.Status,
                    CreatedAt = invoice.CreatedAt,
                    ClosedAt = invoice.ClosedAt,
                    Items = invoice.Items
                        .Select(item => new GetInvoiceItemResponse
                        {
                            ProductId = item.ProductId,
                            ProductCode = item.ProductCode,
                            ProductDescription = item.ProductDescription,
                            Quantity = item.Quantity
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (invoice is null)
            {
                throw new NotFoundException(
                    $"Invoice with id '{id}' was not found.");
            }

            return invoice;
        }
    }
}