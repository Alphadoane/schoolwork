using Microsoft.AspNetCore.Mvc;
using AtmSystem.Models;
using AtmSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace AtmSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AtmDbContext _context;

        public HomeController(AtmDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Dashboard()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null) return RedirectToAction("Login", "Account");

            var account = await _context.Accounts
                .Include(a => a.Transactions)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            return View(account);
        }

        public IActionResult Privacy() => View();
    }
}
