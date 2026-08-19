namespace Korp.Billing.Api.Features.Invoices.Create
{
    public class CreateInvoiceRequest
    {
        public List<CreateInvoiceItemRequest> Items { get; set; } = [];
    }
}
