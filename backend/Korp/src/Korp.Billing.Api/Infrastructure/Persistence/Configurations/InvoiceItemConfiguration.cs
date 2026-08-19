using Korp.Billing.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korp.Billing.Api.Infrastructure.Persistence.Configurations
{
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable("invoice_items");

            builder.HasKey(invoiceItem => invoiceItem.Id);

            builder.Property(invoiceItem => invoiceItem.Id)
                .HasColumnName("id");

            builder.Property(invoiceItem => invoiceItem.InvoiceId)
                .HasColumnName("invoice_id")
                .IsRequired();

            builder.Property(invoiceItem => invoiceItem.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.Property(invoiceItem => invoiceItem.ProductCode)
                .HasColumnName("product_code")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(invoiceItem => invoiceItem.ProductDescription)
                .HasColumnName("product_description")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(invoiceItem => invoiceItem.Quantity)
                .HasColumnName("quantity")
                .IsRequired();
        }
    }
}
