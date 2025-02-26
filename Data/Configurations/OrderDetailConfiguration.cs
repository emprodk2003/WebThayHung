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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
           
            builder.ToTable("OrderDetails");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseIdentityColumn();
            builder.Property(x => x.quantity).IsRequired();
            builder.Property(x => x.Price).HasPrecision(18, 0).IsRequired();
            builder.HasOne(t => t.Order).WithMany(dr => dr.OrderDetails)
                .HasForeignKey(dr => dr.bill_id);
            builder.HasOne(t => t.Product).WithMany(dr => dr.OrderDetails)
               .HasForeignKey(dr => dr.product_id);
        }
    }
}
