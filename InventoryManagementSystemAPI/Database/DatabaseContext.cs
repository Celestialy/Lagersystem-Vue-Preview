using InventoryManagementSystemAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Database
{
    public class DatabaseContext : IdentityDbContext<UserModel, RoleModel, string, IdentityUserClaim<string>, UserRoleModel, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<DepartmentModel> Departments { get; set; }

        public DbSet<InventoryModel> Inventories { get; set; }

        public DbSet<ImageModel> Images { get; set; }

        public DbSet<CategoryModel> Categories { get; set; }

        public DbSet<BarcodeModel> Barcodes { get; set; }

        public DbSet<LoanItemModel> LoanItems { get; set; }

        public DbSet<ConsumptionItemModel> ConsumptionItems { get; set; }

        public DbSet<UserLoanModel> UserLoans { get; set; }

        public DbSet<UserConsumptionsModel> UserConsumptions { get; set; }

        public DbSet<ItemModel> Items { get; set; }
        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserModel>(x =>
            {
                x.HasMany(e => e.Roles).WithOne(e => e.User).HasForeignKey(ur => ur.UserId).IsRequired();
            });

            builder.Entity<RoleModel>(b =>
            {
                // Each Role can have many entries in the UserRole join table
                b.HasMany(e => e.Users)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            builder.Entity<ItemModel>().ToTable("ItemModel");

            builder.Entity<RoleModel>().HasData(new RoleModel { Id = "1", Name = "Admin", NormalizedName = "ADMIN", CreatedAt = DateTime.Now });
            builder.Entity<RoleModel>().HasData(new RoleModel { Id = "2", Name = "Manager", NormalizedName = "MANAGER", CreatedAt = DateTime.Now });
            builder.Entity<RoleModel>().HasData(new RoleModel { Id = "3", Name = "InventoryManager", NormalizedName = "INVENTORYMANAGER", CreatedAt = DateTime.Now });
            builder.Entity<RoleModel>().HasData(new RoleModel { Id = "4", Name = "User", NormalizedName = "USER", CreatedAt = DateTime.Now });

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}