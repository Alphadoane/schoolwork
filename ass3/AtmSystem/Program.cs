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
                Console.WriteLine("  3. 🆕 Create Account");
                Console.WriteLine("  0. ❌ Exit");
                Console.Write("\n  Select Option: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": UserLogin(db); break;
                    case "2": AdminLogin(db); break;
                    case "3": CreateAccount(db); break;
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

        static void CreateAccount(AtmDbContext db)
        {
            Console.Clear();
            Console.WriteLine("── CREATE NEW ACCOUNT ──");
            Console.Write("  Enter Holder Name : ");
            string name = Console.ReadLine() ?? "";
            Console.Write("  Enter Initial Deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal balance) || balance < 0)
            {
                Console.WriteLine("  ✘ Invalid amount!");
                Console.ReadKey();
                return;
            }

            Console.Write("  Set 4-Digit PIN   : ");
            string pin = Console.ReadLine() ?? "";
            if (pin.Length != 4 || !pin.All(char.IsDigit))
            {
                Console.WriteLine("  ✘ PIN must be 4 digits!");
                Console.ReadKey();
                return;
            }

            string accNo = GenerateAccountNumber(db);
            string cardNo = GenerateCardNumber(db);
            string pinHash = HashString(pin);

            var account = new Account
            {
                AccountNumber = accNo,
                HolderName = name,
                Balance = balance,
                AccountType = "Savings",
                Status = "Active"
            };

            db.Accounts.Add(account);
            db.SaveChanges();

            var card = new Card
            {
                AccountId = account.Id,
                CardNumber = cardNo,
                PinHash = pinHash,
                ExpiryDate = DateTime.Now.AddYears(5).ToString("MM/yy"),
                IsActive = true
            };

            db.Cards.Add(card);
            db.SaveChanges();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  ✔ Account Created Successfully!");
            Console.ResetColor();
            Console.WriteLine($"  Account No: {accNo}");
            Console.WriteLine($"  Card No   : {cardNo}");
            Console.WriteLine("\n  Please note your Card Number for login.");
            Console.ReadKey();
        }

        static string GenerateAccountNumber(AtmDbContext db)
        {
            Random res = new Random();
            string accNo;
            do
            {
                accNo = res.Next(10000000, 99999999).ToString();
            } while (db.Accounts.Any(a => a.AccountNumber == accNo));
            return accNo;
        }

        static string GenerateCardNumber(AtmDbContext db)
        {
            Random res = new Random();
            string cardNo;
            do
            {
                cardNo = "4242" + res.Next(10000000, 99999999).ToString() + res.Next(1000, 9999).ToString();
            } while (db.Cards.Any(c => c.CardNumber == cardNo));
            return cardNo;
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
