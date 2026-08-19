using Korp.Billing.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Billing.Api.Features.Invoices.GetAll
{
    public class GetAllInvoicesHandler
    {
        private readonly BillingDbContext _dbContext;

        public GetAllInvoicesHandler(BillingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<GetAllInvoicesResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Invoices
                .AsNoTracking()
                .OrderByDescending(invoice => invoice.Number)
                .Select(invoice => new GetAllInvoicesResponse
                {
                    Id = invoice.Id,
                    Number = invoice.Number,
                    Status = invoice.Status,
                    CreatedAt = invoice.CreatedAt,
                    ClosedAt = invoice.ClosedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}