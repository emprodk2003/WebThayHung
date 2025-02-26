using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Configurations;



namespace Data.EF
{
    public class WebDbContext : IdentityDbContext<User,Role,Guid>
    {
        public WebDbContext(DbContextOptions<WebDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(user =>
            {
                user.ToTable("Users");

                user.HasKey(u => u.Id);

                user.HasMany(u => u.UserRoles)
                       .WithOne(ur => ur.User)
                       .HasForeignKey(ur => ur.UserId)
                       .IsRequired();

                user.HasMany(u => u.UserClaims)
                       .WithOne(uc => uc.User)
                       .HasForeignKey(uc => uc.UserId)
                       .IsRequired();

                user.HasMany(u => u.UserLogins)
                       .WithOne(ul => ul.User)
                       .HasForeignKey(ul => ul.UserId)
                       .IsRequired();

                user.HasMany(u => u.UserTokens)
                       .WithOne(ut => ut.User)
                       .HasForeignKey(ut => ut.UserId)
                       .IsRequired();
            });
            // Cấu hình cho Role
            modelBuilder.Entity<Role>(role =>
            {
                role.ToTable("Roles");

                role.Property(r => r.Description)
                    .IsUnicode(true)
                    .IsRequired();

                // Quan hệ giữa Role và UserRole
                role.HasMany(r => r.UserRoles)
                     .WithOne(ur => ur.Role)
                     .HasForeignKey(ur => ur.RoleId)
                     .IsRequired();

            });
            // Cấu hình cho ApplicationUserRole
            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.ToTable("UserRoles");

                userRole.HasOne(ur => ur.User)
                       .WithMany(u => u.UserRoles)
                       .HasForeignKey(ur => ur.UserId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.NoAction); // 🔥 Thêm dòng này

                userRole.HasOne(ur => ur.Role)
                       .WithMany(r => r.UserRoles)
                       .HasForeignKey(ur => ur.RoleId)
                       .IsRequired()
                       .OnDelete(DeleteBehavior.NoAction); // 🔥 Thêm dòng này
            });


            // Cấu hình cho UserToken
            modelBuilder.Entity<UserToken>(userToken =>
            {
                userToken.ToTable("UserTokens");

                userToken.HasOne(ut => ut.User)
                         .WithMany(u => u.UserTokens)
                         .HasForeignKey(ut => ut.UserId)
                         .IsRequired()
                         .OnDelete(DeleteBehavior.NoAction); // 🔥 Thêm dòng này
            });

            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new TotalRevenueConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
            modelBuilder.ApplyConfiguration(new Cata_ProductConfiguration());
            modelBuilder.ApplyConfiguration(new Size_ProductConfiguration());
            modelBuilder.ApplyConfiguration(new AdvertisementConfiguration());
        }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TotalRevenue> TotalRevenue { get; set; }
        public DbSet<Variants_product> Variants_product { get; set; }
        public DbSet<Size_Product> Size_Product {  get; set; }
        public DbSet<Advertisement> Advertisements { get; set;}
    }
}
