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
                .OnDelete(DeleteBehavior.Cascade); // Adjust the delete behavior based on your requirements
        }
    }
}