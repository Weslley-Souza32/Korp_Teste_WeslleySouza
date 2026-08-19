using Korp.Stock.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Korp.Stock.Api.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(product => product.Id);

            builder.Property(product => product.Id)
                .HasColumnName("id");

            builder.Property(product => product.Code)
                .HasColumnName("code")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(product => product.Code)
                .IsUnique();

            builder.Property(product => product.Description)
                .HasColumnName("description")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(product => product.StockQuantity)
                .HasColumnName("stock_quantity")
                .IsRequired();

            builder.Property(product => product.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(product => product.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(product => product.Version)
                .IsRowVersion();
        }
    }
}
