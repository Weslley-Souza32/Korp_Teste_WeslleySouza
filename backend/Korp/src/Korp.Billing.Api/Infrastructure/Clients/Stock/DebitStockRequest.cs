namespace Korp.Billing.Api.Infrastructure.Clients.Stock
{
    public class DebitStockRequest
    {
        public Guid InvoiceId { get; set; }
        public List<DebitStockItemRequest> Items { get; set; } = [];
    }
}