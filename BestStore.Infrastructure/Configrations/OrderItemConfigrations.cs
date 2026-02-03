using BestStore.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BestStore.Infrastructure.Configrations
{
    internal class OrderItemConfigrations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder.Property(p => p.UnitPrice).HasPrecision(16, 2);
            builder.HasOne(oi => oi.Product)
         .WithMany()
         .HasForeignKey(oi => oi.ProductId)
         .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
