using Korp.Billing.Api.Domain.Enums;

namespace Korp.Billing.Api.Domain.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public long Number { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ClosedAt { get; set; }
        public ICollection<InvoiceItem> Items { get; set; } = [];
    }
}
