using Korp.Stock.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korp.Stock.Api.Infrastructure.Persistence.Configurations
{
    public class StockDebitOperationConfiguration
        : IEntityTypeConfiguration<StockDebitOperation>
    {
        public void Configure(
            EntityTypeBuilder<StockDebitOperation> builder)
        {
            builder.ToTable("stock_debit_operations");

            builder.HasKey(operation => operation.Id);

            builder.Property(operation => operation.Id)
                .HasColumnName("id");

            builder.Property(operation => operation.InvoiceId)
                .HasColumnName("invoice_id")
                .IsRequired();

            builder.Property(operation => operation.ProcessedAt)
                .HasColumnName("processed_at")
                .IsRequired();

            builder.HasIndex(operation => operation.InvoiceId)
                .IsUnique();
        }
    }
}