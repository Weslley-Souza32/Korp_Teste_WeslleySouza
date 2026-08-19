namespace Korp.Billing.Api.Features.Invoices.Create
{
    public class CreateInvoiceItemRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
