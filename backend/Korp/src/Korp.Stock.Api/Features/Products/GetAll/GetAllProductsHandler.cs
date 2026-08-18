using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Api.Features.Products.GetAll
{
    public class GetAllProductsHandler
    {
        private readonly StockDbContext _dbContext;

        public GetAllProductsHandler(StockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<GetAllProductsResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var products = await _dbContext.Products
                .AsNoTracking()
                .OrderBy(product => product.Code)
                .Select(p => new GetAllProductsResponse
                {
                    Id = p.Id,
                    Code = p.Code,
                    Description = p.Description,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync(cancellationToken);
            return products;
        }
    }
}
