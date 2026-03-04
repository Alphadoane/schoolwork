using Microsoft.AspNetCore.Mvc;
using Cat2System.Data;
using Cat2System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat2System.Controllers
{
    public class StudentController : Controller
    {
        private readonly Cat2DbContext _context;

        public StudentController(Cat2DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard(string q)
        {
            var students = _context.Students.Include(s => s.Program).ThenInclude(p => p!.Faculty).AsQueryable();
            if (!string.IsNullOrEmpty(q))
            {
                students = students.Where(s => s.RegistrationNumber.Contains(q) || s.FirstName.Contains(q) || s.LastName.Contains(q));
            }

            ViewBag.Sessions = await _context.ExamSessions.ToListAsync();
            ViewBag.Query = q;
            return View(await students.ToListAsync());
        }

        public async Task<IActionResult> ResultSlip(string regNumber, int? sessionId)
        {
            var student = await _context.Students
                .Include(s => s.Program).ThenInclude(p => p!.Faculty)
                .FirstOrDefaultAsync(s => s.RegistrationNumber == regNumber);

            if (student == null) return NotFound();

            var sessions = await _context.ExamSessions.ToListAsync();
            ViewBag.Sessions = sessions;

            ExamSession? selectedSession = null;
            if (sessionId.HasValue)
            {
                selectedSession = sessions.FirstOrDefault(s => s.Id == sessionId);
            }
            else
            {
                selectedSession = sessions.FirstOrDefault(s => s.IsActive);
            }

            List<Result> results = new List<Result>();
            if (selectedSession != null)
            {
                results = await _context.Results
                    .Include(r => r.Unit)
                    .Where(r => r.StudentId == student.Id && r.ExamSessionId == selectedSession.Id)
                    .ToListAsync();
            }

            ViewBag.SelectedSession = selectedSession;
            ViewBag.Results = results;
            ViewBag.GPA = CalculateGPA(results);

            return View(student);
        }

        private double CalculateGPA(List<Result> results)
        {
            if (results == null || results.Count == 0) return 0;
            double totalPoints = 0;
            int totalCredits = 0;
            foreach (var r in results)
            {
                totalPoints += GetGradePoints(r.Grade) * r.Unit!.CreditHours;
                totalCredits += r.Unit.CreditHours;
            }
            return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
        }

        private double GetGradePoints(string grade)
        {
            return grade switch
            {
                "A" => 4.0,
                "B+" => 3.5,
                "B" => 3.0,
                "C+" => 2.5,
                "C" => 2.0,
                "D+" => 1.5,
                "D" => 1.0,
                _ => 0.0
            };
        }
    }
}
