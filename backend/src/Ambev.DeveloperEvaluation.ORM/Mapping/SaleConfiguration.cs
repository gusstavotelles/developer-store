using System;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.SaleNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Date)
                .IsRequired();

            builder.Property(s => s.CustomerId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.BranchId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.IsCancelled)
                .IsRequired();

            builder.Property(s => s.Total)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Map items as a separate table linked by SaleId.
            // Use OwnsMany to keep mapping simple within the same configuration.
            builder.OwnsMany(s => s.Items, ib =>
            {
                ib.ToTable("SaleItems");

                // Use a shadow key property if SaleItem.Id is not recognized as key in owned mapping.
                // Also map SaleItem properties explicitly.
                ib.Property<Guid>("Id");
                ib.HasKey("Id");

                ib.Property(i => i.ProductId)
                    .IsRequired()
                    .HasMaxLength(100);

                ib.Property(i => i.ProductName)
                    .IsRequired()
                    .HasMaxLength(250);

                ib.Property(i => i.UnitPrice)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                ib.Property(i => i.Quantity)
                    .IsRequired();

                ib.Property(i => i.DiscountPercent)
                    .HasColumnType("decimal(5,2)")
                    .IsRequired();

                ib.Property(i => i.Total)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                ib.Property(i => i.IsCancelled)
                    .IsRequired();

                // Configure foreign key (shadow) to parent Sale
                ib.WithOwner().HasForeignKey("SaleId");
                ib.Ignore(i => i.SaleId); // SaleId is represented by shadow FK
            });
        }
    }
}
