namespace Korp.Billing.Api.Features.Invoices.Create
{
    public class CreateInvoiceItemResponse
    {
        public Guid ProductId { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
