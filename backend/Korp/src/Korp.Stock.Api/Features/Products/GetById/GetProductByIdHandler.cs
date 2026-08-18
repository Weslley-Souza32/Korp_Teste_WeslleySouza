using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Api.Features.Products.GetById
{
    public class GetProductByIdHandler
    {
        private readonly StockDbContext _dbContext;

        public GetProductByIdHandler(StockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetProductByIdResponse> HandleAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await _dbContext.Products
                .AsNoTracking()
                .Where(product => product.Id == id)
                .Select(product => new GetProductByIdResponse
                {
                    Id = product.Id,
                    Code = product.Code,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null)
            {
                throw new NotFoundException($"Product with ID '{id}' was not found.");
            }

            return product;
        }
    }
}
