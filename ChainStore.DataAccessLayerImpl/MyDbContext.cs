﻿using ChainStore.DataAccessLayerImpl.DbModels;
using ChainStore.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.DataAccessLayerImpl
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        internal DbSet<StoreProductDbModel> StoreProductRelation { get; set; }
        internal DbSet<StoreCategoryDbModel> StoreCategoryRelation { get; set; }
        internal DbSet<ProductDbModel> Products { get; set; }
        internal DbSet<ClientDbModel> Clients { get; set; }
        internal DbSet<PurchaseDbModel> Purchases { get; set; }
        internal DbSet<BookDbModel> Books { get; set; }
        internal DbSet<CategoryDbModel> Categories { get; set; }
        internal DbSet<StoreDbModel> Stores { get; set; }
        internal DbSet<User> Users { get; set; }
        internal DbSet<Role> Roles { get; set; }
        internal DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserRole>().HasKey(e => new { e.UserId, e.RoleId });
            modelBuilder.Entity<StoreProductDbModel>().HasKey(e => new { e.StoreDbModelId, e.ProductDbModelId });
            modelBuilder.Entity<StoreCategoryDbModel>().HasKey(e => new { e.StoreDbModelId, e.CategoryDbModelId });
            modelBuilder.Entity<StoreProductDbModel>().HasOne(e => e.StoreDbModel)
                .WithMany(e => e.StoreProductRelation);
            modelBuilder.Entity<StoreProductDbModel>().HasOne(e => e.ProductDbModel)
                .WithMany(e => e.StoreProductRelation);
            modelBuilder.Entity<StoreCategoryDbModel>().HasOne(e => e.StoreDbModel)
                .WithMany(e => e.StoreCategoryRelation);
            modelBuilder.Entity<StoreCategoryDbModel>().HasOne(e => e.CategoryDbModel)
                .WithMany(e => e.StoreCategoryRelation);
            modelBuilder.Entity<CategoryDbModel>().HasMany(cat => cat.ProductDbModels)
                .WithOne(pr => pr.CategoryDbModel);
            modelBuilder.Seed();
        }
    }
}