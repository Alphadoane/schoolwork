using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AtmSystem.Data;
using AtmSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace AtmSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "KCA University - ATM Management System";
            using (var db = new AtmDbContext())
            {
                SeedData.Initialize(db);
                RunMainMenu(db);
            }
        }

        static void RunMainMenu(AtmDbContext db)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("============================================================");
                Console.WriteLine("           KCA UNIVERSITY - ATM MANAGEMENT SYSTEM           ");
                Console.WriteLine("============================================================");
                Console.ResetColor();
                Console.WriteLine("\n  1. 👤 Account Login");
                Console.WriteLine("  2. 🔐 Admin Login");
                Console.WriteLine("  0. ❌ Exit");
                Console.Write("\n  Select Option: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": UserLogin(db); break;
                    case "2": AdminLogin(db); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("\n  ✘ Invalid choice!"); Console.ReadKey(); break;
                }
            }
        }

        static void UserLogin(AtmDbContext db)
        {
            Console.Clear();
            Console.WriteLine("── ACCOUNT LOGIN ──");
            Console.Write("  Enter Card Number : ");
            string cardNumber = Console.ReadLine() ?? "";
            Console.Write("  Enter PIN         : ");
            string pin = Console.ReadLine() ?? "";

            string pinHash = HashString(pin);
            var card = db.Cards.Include(c => c.Account)
                .FirstOrDefault(c => c.CardNumber == cardNumber && c.PinHash == pinHash && c.IsActive);

            if (card != null && card.Account != null)
            {
                UserMenu(db, card.Account);
            }
            else
            {
                Console.WriteLine("\n  ✘ Invalid Credentials or Inactive Card!");
                Console.ReadKey();
            }
        }

        static void UserMenu(AtmDbContext db, Account account)
        {
            bool logout = false;
            while (!logout)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"── WELCOME, {account.HolderName.ToUpper()} ──");
                Console.ResetColor();
                Console.WriteLine($"  Account: {account.AccountNumber} | Type: {account.AccountType}");
                Console.WriteLine($"  Balance: KES {account.Balance:N2}");
                Console.WriteLine("\n  1. 📋 View Statement");
                Console.WriteLine("  2. 💵 Withdraw Cash");
                Console.WriteLine("  3. 📥 Deposit Cash");
                Console.WriteLine("  0. ⬅️ Logout");
                Console.Write("\n  Select Option: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": ViewStatement(db, account); break;
                    case "2": Withdraw(db, account); break;
                    case "3": Deposit(db, account); break;
                    case "0": logout = true; break;
                }
            }
        }

        static void ViewStatement(AtmDbContext db, Account account)
        {
            Console.Clear();
            Console.WriteLine($"── TRANSACTION STATEMENT: {account.AccountNumber} ──");
            var txs = db.Transactions.Where(t => t.AccountId == account.Id).OrderByDescending(t => t.Timestamp).Take(10).ToList();
            if (!txs.Any()) Console.WriteLine("  No transactions yet.");
            else
            {
                Console.WriteLine("{0,-20} {1,-15} {2,12} {3}", "Date", "Type", "Amount", "Description");
                Console.WriteLine(new string('-', 60));
                foreach (var tx in txs)
                {
                    Console.WriteLine("{0,-20} {1,-15} {2,12:N2} {3}", tx.Timestamp, tx.Type, tx.Amount, tx.Description);
                }
            }
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        static void Withdraw(AtmDbContext db, Account account)
        {
            Console.Write("\n  Enter withdrawal amount: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                if (account.Balance >= amount)
                {
                    account.Balance -= amount;
                    db.Transactions.Add(new Transaction { AccountId = account.Id, Type = "Withdrawal", Amount = amount, BalanceAfter = account.Balance, Description = "ATM Withdrawal" });
                    db.SaveChanges();
                    Console.WriteLine($"\n  ✔ Successfully withdrawn KES {amount:N2}");
                }
                else Console.WriteLine("\n  ✘ Insufficient balance!");
            }
            else Console.WriteLine("\n  ✘ Invalid amount!");
            Console.ReadKey();
        }

        static void Deposit(AtmDbContext db, Account account)
        {
            Console.Write("\n  Enter deposit amount: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
            {
                account.Balance += amount;
                db.Transactions.Add(new Transaction { AccountId = account.Id, Type = "Deposit", Amount = amount, BalanceAfter = account.Balance, Description = "ATM Deposit" });
                db.SaveChanges();
                Console.WriteLine($"\n  ✔ Successfully deposited KES {amount:N2}");
            }
            else Console.WriteLine("\n  ✘ Invalid amount!");
            Console.ReadKey();
        }

        static void AdminLogin(AtmDbContext db)
        {
            Console.Clear();
            Console.WriteLine("── ADMIN LOGIN ──");
            Console.Write("  Username : ");
            string username = Console.ReadLine() ?? "";
            Console.Write("  Password : ");
            string password = Console.ReadLine() ?? "";

            string passHash = HashString(password);
            var admin = db.Admins.FirstOrDefault(a => a.Username == username && a.PasswordHash == passHash);

            if (admin != null) AdminMenu(db);
            else { Console.WriteLine("\n  ✘ Invalid Admin Credentials!"); Console.ReadKey(); }
        }

        static void AdminMenu(AtmDbContext db)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("── ADMIN DASHBOARD ──");
                Console.ResetColor();
                Console.WriteLine("  1. 👥 View All Accounts");
                Console.WriteLine("  2. 💳 View All Cards");
                Console.WriteLine("  3. 🏧 Manage Machines");
                Console.WriteLine("  0. ⬅️ Back");
                Console.Write("\n  Select Option: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        var accs = db.Accounts.ToList();
                        Console.WriteLine("{0,-10} {1,-20} {2,15}", "Acc No", "Holder", "Balance");
                        foreach (var a in accs) Console.WriteLine("{0,-10} {1,-20} {2,15:N2}", a.AccountNumber, a.HolderName, a.Balance);
                        Console.ReadKey();
                        break;
                    case "2":
                        Console.Clear();
                        var cards = db.Cards.Include(c => c.Account).ToList();
                        Console.WriteLine("{0,-18} {1,-15} {2}", "Card No", "Account", "Status");
                        foreach (var c in cards) Console.WriteLine("{0,-18} {1,-15} {2}", c.CardNumber, c.Account?.AccountNumber, c.IsActive ? "Active" : "Blocked");
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.Clear();
                        var machines = db.AtmMachines.ToList();
                        foreach (var m in machines) Console.WriteLine($"{m.MachineId} | {m.Location} | KES {m.CashAvailable:N2} | {m.Status}");
                        Console.ReadKey();
                        break;
                    case "0": exit = true; break;
                }
            }
        }

        static string HashString(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
