using Microsoft.EntityFrameworkCore;
using AtmSystem.Models;
using System.IO;

namespace AtmSystem.Data
{
    public class AtmDbContext : DbContext
    {
        public AtmDbContext() { }
        public AtmDbContext(DbContextOptions<AtmDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ATMMachine> AtmMachines { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "atm.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.AccountNumber)
                .IsUnique();

            modelBuilder.Entity<Card>()
                .HasIndex(c => c.CardNumber)
                .IsUnique();

            modelBuilder.Entity<ATMMachine>()
                .HasIndex(m => m.MachineId)
                .IsUnique();
        }
    }
}
