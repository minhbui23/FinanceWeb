using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Finance.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace Finance.DataAccess.DBContext
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<SpendingCategory> SpendingCategories { get; set; }
        public DbSet<IncomeCategory> IncomeCategories { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Spending> Spendings { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-many relationship between ApplicationUser and Wallet
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(user => user.Wallets)
                .WithOne(wallet => wallet.User)
                .HasForeignKey(wallet => wallet.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Adjust the delete behavior based on your requirements

            // Configure the one-to-many relationship between Wallet and Spending
            modelBuilder.Entity<Wallet>()
                .HasMany(wallet => wallet.Spendings)
                .WithOne(spending => spending.Wallet)
                .HasForeignKey(spending => spending.IdWallet)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Wallet>()
                .HasMany(wallet => wallet.Incomes)
                .WithOne(income => income.Wallet)
                .HasForeignKey(income => income.IdWallet)
                .OnDelete(DeleteBehavior.Cascade);

             // Configure the one-to-many relationship between SpendingCategory and Spending
            modelBuilder.Entity<Spending>()
                .HasOne(spending => spending.Spending_Category)
                .WithMany()
                .HasForeignKey(spending => spending.SpendingCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the one-to-many relationship between IncomeCategory and Income
            modelBuilder.Entity<Income>()
                .HasOne(income => income.Income_Category)
                .WithMany()
                .HasForeignKey(income => income.IncomeCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}