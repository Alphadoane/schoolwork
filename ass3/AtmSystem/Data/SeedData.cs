using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AtmSystem.Data;
using AtmSystem.Models;
using System.Security.Cryptography;
using System.Text;

namespace AtmSystem.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AtmDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<AtmDbContext>>()))
            {
                context.Database.EnsureCreated();

                if (context.Accounts.Any())
                {
                    return;   // DB has been seeded
                }

                var alice = new Account { AccountNumber = "1001", HolderName = "Alice Johnson", Balance = 50000, AccountType = "Savings" };
                var bob = new Account { AccountNumber = "1002", HolderName = "Bob Smith", Balance = 25000, AccountType = "Current" };
                var carol = new Account { AccountNumber = "1003", HolderName = "Carol Williams", Balance = 75000, AccountType = "Savings" };

                context.Accounts.AddRange(alice, bob, carol);
                context.SaveChanges();

                context.Cards.AddRange(
                    new Card { CardNumber = "1234567890123456", AccountId = alice.Id, PinHash = HashPin("1234"), ExpiryDate = "12/28" },
                    new Card { CardNumber = "2345678901234567", AccountId = bob.Id, PinHash = HashPin("2345"), ExpiryDate = "12/28" },
                    new Card { CardNumber = "3456789012345678", AccountId = carol.Id, PinHash = HashPin("3456"), ExpiryDate = "12/28" }
                );

                context.Admins.Add(new Admin { Username = "admin", PasswordHash = HashPin("admin123") });

                context.AtmMachines.Add(new ATMMachine { MachineId = "ATM001", Location = "Main Campus", CashAvailable = 1000000, Status = "Online" });

                context.SaveChanges();
            }
        }

        private static string HashPin(string pin)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pin));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
