namespace Korp.Billing.Api.Infrastructure.Clients.Stock
{
    public interface IStockServiceClient
    {
        Task<StockProductResponse?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<DebitStockResponse> DebitStockAsync(DebitStockRequest request, CancellationToken cancellationToken = default);
    }
}