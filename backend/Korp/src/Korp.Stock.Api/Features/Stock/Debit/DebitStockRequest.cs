namespace Korp.Stock.Api.Features.Stock.Debit
{
    public class DebitStockRequest
    {
        public Guid InvoiceId { get; set; }
        public List<DebitStockItemRequest> Items { get; set; } = [];
    }
}