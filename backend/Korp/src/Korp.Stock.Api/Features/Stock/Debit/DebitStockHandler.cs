using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Korp.Stock.Api.Features.Stock.Debit
{
    public class DebitStockHandler
    {
        private readonly StockDbContext _dbContext;

        public DebitStockHandler(StockDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DebitStockResponse> HandleAsync(DebitStockRequest request, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            var productIds = request.Items
                .Select(item => item.ProductId)
                .ToList();

            var products = await _dbContext.Products
                .Where(product => productIds.Contains(product.Id))
                .ToListAsync(cancellationToken);

            foreach (var requestItem in request.Items)
            {
                var product = products.FirstOrDefault(
                    product => product.Id == requestItem.ProductId);

                if (product is null)
                {
                    throw new NotFoundException(
                        $"Product with id '{requestItem.ProductId}' was not found.");
                }

                if (product.StockQuantity < requestItem.Quantity)
                {
                    throw new ConflictException(
                        $"Product '{product.Code}' has insufficient stock.");
                }
            }

            foreach (var requestItem in request.Items)
            {
                var product = products.Single(
                    product => product.Id == requestItem.ProductId);

                product.StockQuantity -= requestItem.Quantity;
                product.UpdatedAt = DateTimeOffset.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return new DebitStockResponse
            {
                Success = true
            };
        }
    }
}