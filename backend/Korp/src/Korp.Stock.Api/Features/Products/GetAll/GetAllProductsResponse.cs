namespace Korp.Stock.Api.Features.Products.GetAll
{
    public class GetAllProductsResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
