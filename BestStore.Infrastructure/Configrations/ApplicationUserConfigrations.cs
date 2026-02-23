using BestStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BestStore.Infrastructure.Configrations
{
    internal class ApplicationUserConfigrations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(p => p.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.LastName)
                .HasMaxLength(100)
               .IsRequired();

            builder.Property(p => p.Address)
                .HasMaxLength(250);
            builder.Property(p => p.AcceptTerms)
                .HasDefaultValue(false)
                .IsRequired();
        }
    }
}
