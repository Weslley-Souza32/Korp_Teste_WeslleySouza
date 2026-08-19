namespace Korp.Billing.Api.Infrastructure.Clients.Stock
{
    public class DebitStockItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}