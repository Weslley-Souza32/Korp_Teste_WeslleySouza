using Korp.Billing.Api.Domain.Enums;

namespace Korp.Billing.Api.Features.Invoices.GetAll
{
    public class GetAllInvoicesResponse
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
    }
}