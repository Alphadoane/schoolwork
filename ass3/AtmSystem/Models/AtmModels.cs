using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtmSystem.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required]
        public string AccountNumber { get; set; } = string.Empty;
        [Required]
        public string HolderName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string AccountType { get; set; } = "Savings";
        public string Status { get; set; } = "Active";

        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class Card
    {
        public int Id { get; set; }
        [Required]
        public string CardNumber { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        [Required]
        public string PinHash { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public string Type { get; set; } = string.Empty; // Deposit, Withdrawal, Transfer
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    public class ATMMachine
    {
        public int Id { get; set; }
        [Required]
        public string MachineId { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal CashAvailable { get; set; }
        public string Status { get; set; } = "Online";
        public DateTime LastServiced { get; set; } = DateTime.Now;
    }

    public class Admin
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
