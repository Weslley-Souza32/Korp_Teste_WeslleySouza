namespace Korp.Stock.Api.Domain.Entities
{
    public class StockDebitOperation
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public DateTimeOffset ProcessedAt { get; set; }
    }
}
