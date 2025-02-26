using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Oders");
            builder.HasKey(x => x.bill_id);
            builder.Property(x => x.bill_id).UseIdentityColumn();
            builder.Property(x => x.ShipEmail).IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(x => x.ShipName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ShipAddress).IsRequired().HasMaxLength(200);
            builder.Property(x => x.ShipPhoneNumber).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Totalprice).IsRequired().HasPrecision(18, 0);
            builder.Property(x => x.ShippingFee).IsRequired().HasPrecision(18, 0);
            builder.Property(x => x.Note).IsRequired(false);
            builder.HasOne(x=>x.AppUser).WithMany(x=>x.Orders).HasForeignKey(x=>x.UserId);
        }
    }
}
