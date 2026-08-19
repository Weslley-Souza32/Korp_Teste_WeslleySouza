namespace Korp.Billing.Api.Infrastructure.Clients.Stock
{
    public class StockProductResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
