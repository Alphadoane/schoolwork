using Microsoft.AspNetCore.Mvc;
using AtmSystem.Data;
using AtmSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AtmSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AtmDbContext _context;

        public AdminController(AtmDbContext context)
        {
            _context = context;
        }

        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            string passwordHash = HashPin(password);
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Username == username && a.PasswordHash == passwordHash);

            if (admin != null)
            {
                HttpContext.Session.SetInt32("AdminId", admin.Id);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid Admin Credentials";
            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null) return RedirectToAction("Login");

            var data = new AdminDashboardViewModel
            {
                Accounts = await _context.Accounts.ToListAsync(),
                Cards = await _context.Cards.Include(c => c.Account).ToListAsync(),
                AtmMachines = await _context.AtmMachines.ToListAsync()
            };

            return View(data);
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

    public class AdminDashboardViewModel
    {
        public List<Account>? Accounts { get; set; }
        public List<Card>? Cards { get; set; }
        public List<ATMMachine>? AtmMachines { get; set; }
    }
}
