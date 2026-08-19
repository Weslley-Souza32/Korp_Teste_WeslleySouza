using Korp.Stock.Api.Common.Exceptions;
using Korp.Stock.Api.Domain.Entities;
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

        public async Task<DebitStockResponse> HandleAsync(
            DebitStockRequest request,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            var alreadyProcessed = await _dbContext.StockDebitOperations
                .AnyAsync(
                    operation => operation.InvoiceId == request.InvoiceId,
                    cancellationToken);

            if (alreadyProcessed)
            {
                return new DebitStockResponse
                {
                    Success = true
                };
            }

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

            var debitOperation = new StockDebitOperation
            {
                Id = Guid.NewGuid(),
                InvoiceId = request.InvoiceId,
                ProcessedAt = DateTimeOffset.UtcNow
            };

            _dbContext.StockDebitOperations.Add(debitOperation);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {

                throw new ConflictException("Stock was changed by another operation. Please try again.");
            }

            return new DebitStockResponse
            {
                Success = true
            };
        }
    }
}