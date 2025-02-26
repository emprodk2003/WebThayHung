using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configurations
{
    public class Size_ProductConfiguration : IEntityTypeConfiguration<Size_Product>
    {
        public void Configure(EntityTypeBuilder<Size_Product> builder)
        {
            builder.ToTable("Size_Product");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Name).HasMaxLength(10);
            builder.Property(x=>x.quantity).IsRequired();
            builder.HasOne(x => x.variants_product).WithMany(x => x.Size).HasForeignKey(x => x.variants_product_id);
        }
    }
}
