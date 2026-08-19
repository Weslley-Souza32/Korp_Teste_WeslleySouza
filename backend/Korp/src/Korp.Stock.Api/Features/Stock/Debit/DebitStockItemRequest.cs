namespace Korp.Stock.Api.Features.Stock.Debit
{
    public class DebitStockItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}