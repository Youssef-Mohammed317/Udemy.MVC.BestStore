
using BestStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Infrastructure.Configrations
{
    internal class ProductConfigrations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(1000);



            builder.Property(p => p.ImageUrl)
              .HasMaxLength(100);

            builder.Property(p => p.Price)
                .IsRequired()
              .HasPrecision(16, 2);

            builder.Property(p => p.CreatedAt)
              .IsRequired();

            builder.Property(p => p.LastUpdatedAt)
              .IsRequired();

            builder.Property(p => p.Brand).IsRequired().HasMaxLength(100);

            builder.Property(p => p.CategoryId)
                .IsRequired();



            builder.HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.CategoryId);
        }
    }
}
