using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Domain.Entities;
using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Api.Features.Products.Create
{
    public class CreateProductHandler
    {
        private readonly StockDbContext _dbContext;

        public CreateProductHandler(StockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreateProductResponse> HandleAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
        {
            var productCodeAlreadyExists = await _dbContext.Products.AnyAsync(product => product.Code == request.Code, cancellationToken);

            if (productCodeAlreadyExists)
            {
                throw new ConflictException($"A product with code '{request.Code}' already exists.");
            }

            var now = DateTimeOffset.UtcNow;

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Code = request.Code,
                Description = request.Description,
                StockQuantity = request.StockQuantity,
                CreatedAt = now,
                UpdatedAt = now
            };

            _dbContext.Products.Add(product);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateProductResponse
            {
                Id = product.Id,
                Code = product.Code,
                Description = product.Description,
                StockQuantity = product.StockQuantity
            };
        }
    }
}
