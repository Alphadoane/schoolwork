using Microsoft.AspNetCore.Mvc;
using AtmSystem.Data;
using AtmSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AtmSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AtmDbContext _context;

        public AccountController(AtmDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string cardNumber, string pin)
        {
            string pinHash = HashPin(pin);
            var card = await _context.Cards
                .Include(c => c.Account)
                .FirstOrDefaultAsync(c => c.CardNumber == cardNumber && c.PinHash == pinHash && c.IsActive);

            if (card != null && card.Account != null)
            {
                HttpContext.Session.SetInt32("AccountId", card.AccountId);
                HttpContext.Session.SetString("AccountName", card.Account.HolderName);
                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Invalid Card Number or PIN";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string HashPin(string pin)
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
