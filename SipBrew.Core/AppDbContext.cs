using Microsoft.EntityFrameworkCore;
using SipBrew.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SipBrew.Core
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminModel> Admins => Set<AdminModel>();
        public DbSet<ProductsModel> Products => Set<ProductsModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //AdminModel
            modelBuilder.Entity<AdminModel>()
                .HasIndex(x => x.Email)
                .IsUnique();

            //ProductsModel
            modelBuilder.Entity<ProductsModel>()
                .Property(x => x.ProductName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<ProductsModel>()
                .Property(x => x.Price)
                .HasPrecision(18, 2);


            base.OnModelCreating(modelBuilder);
        }
    }
}
