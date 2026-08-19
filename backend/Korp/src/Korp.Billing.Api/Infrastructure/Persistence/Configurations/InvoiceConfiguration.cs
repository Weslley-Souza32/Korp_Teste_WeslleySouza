using Korp.Billing.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korp.Billing.Api.Infrastructure.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("invoices");

            builder.HasKey(invoice => invoice.Id);

            builder.Property(invoice => invoice.Id)
                .HasColumnName("id");

            builder.Property(invoice => invoice.Number)
                .HasColumnName("number")
                .ValueGeneratedOnAdd();

            builder.HasIndex(invoice => invoice.Number)
                .IsUnique();

            builder.Property(invoice => invoice.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(invoice => invoice.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(invoice => invoice.ClosedAt)
                .HasColumnName("closed_at");

            builder.HasMany(invoice => invoice.Items)
                .WithOne(item => item.Invoice)
                .HasForeignKey(item => item.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
