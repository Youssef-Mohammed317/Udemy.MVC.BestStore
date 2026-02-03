using BestStore.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Infrastructure.Configrations
{
    internal class OrderConfigrations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.ShippingFee).HasPrecision(16, 2);
        }
    }
    internal class OrderItemConfigrations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder.Property(p => p.UnitPrice).HasPrecision(16, 2);
        }
    }
}
