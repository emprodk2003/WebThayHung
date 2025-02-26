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
    public class TotalRevenueConfiguration : IEntityTypeConfiguration<TotalRevenue>
    {
        public void Configure(EntityTypeBuilder<TotalRevenue> builder)
        {
            builder.ToTable("TotalRevenues");
            builder.HasKey(x => x.doanhthu_id);
            builder.Property(x => x.doanhthu_id).UseIdentityColumn();
            builder.Property(x => x.date).IsRequired();
            builder.Property(x => x.tongdoanhthu).HasPrecision(18, 0).IsRequired();
        }
    }
}
