using Korp.Billing.Api.Domain.Enums;

namespace Korp.Billing.Api.Features.Invoices.Create
{
    public class CreateInvoiceResponse
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<CreateInvoiceItemResponse> Items { get; set; } = [];
    }
}
