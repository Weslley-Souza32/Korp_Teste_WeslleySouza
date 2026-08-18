namespace Korp.Stock.Api.Features.Products.Create
{
    public class CreateProductRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
