using System.Security.Cryptography;
using System.Text;
using Cat2System.Data;
using Cat2System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat2System
{
    class Program
    {
        private static AppUser? CurrentUser = null;

        static void Main(string[] args)
        {
            Console.Title = "KCA University - Student Results Portal";
            using (var db = new Cat2DbContext())
            {
                SeedData.Initialize(db);
                RunMainMenu(db);
            }
        }

        static void RunMainMenu(Cat2DbContext db)
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("============================================================");
                Console.WriteLine("           KCA UNIVERSITY - STUDENT RESULTS PORTAL          ");
                Console.WriteLine("============================================================");
                Console.ResetColor();

                if (CurrentUser == null)
                {
                    Console.WriteLine("\n  1. 👤 Login");
                    Console.WriteLine("  2. 📝 Register");
                    Console.WriteLine("  0. ❌ Exit");
                }
                else
                {
                    Console.WriteLine($"\n  Logged in as: {CurrentUser.FullName} ({CurrentUser.Role})");
                    Console.WriteLine("  3. 🔍 Search Student & View Result Slip");
                    Console.WriteLine("  4. 📋 View All Students");
                    Console.WriteLine("  9. ⬅️ Logout");
                    Console.WriteLine("  0. ❌ Exit");
                }
                Console.Write("\n  Select Option: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": if (CurrentUser == null) Login(db); break;
                    case "2": if (CurrentUser == null) Register(db); break;
                    case "3": if (CurrentUser != null) SearchStudent(db); break;
                    case "4": if (CurrentUser != null) ViewAllStudents(db); break;
                    case "9": CurrentUser = null; break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("\n  ✘ Invalid choice!"); Console.ReadKey(); break;
                }
            }
        }

        static void ViewAllStudents(Cat2DbContext db)
        {
            Console.Clear();
            Console.WriteLine("── ALL REGISTERED STUDENTS ──");
            var students = db.Students.Include(s => s.Program).ToList();
            Console.WriteLine("{0,-15} {1,-25} {2}", "Reg No", "Name", "Program");
            Console.WriteLine(new string('-', 60));
            foreach (var s in students)
            {
                Console.WriteLine("{0,-15} {1,-25} {2}", s.RegistrationNumber, s.FullName, s.Program?.Code ?? "N/A");
            }
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        static void SearchStudent(Cat2DbContext db)
        {
            Console.Clear();
            Console.WriteLine("── STUDENT SEARCH ──");
            Console.Write("  Enter Reg Number or Name: ");
            string query = Console.ReadLine() ?? "";

            var student = db.Students
                .Include(s => s.Program).ThenInclude(p => p!.Faculty)
                .FirstOrDefault(s => s.RegistrationNumber.Contains(query) || s.FirstName.Contains(query) || s.LastName.Contains(query));

            if (student != null)
            {
                ViewResultSlip(db, student);
            }
            else
            {
                Console.WriteLine("\n  ✘ Student not found!");
                Console.ReadKey();
            }
        }

        static void ViewResultSlip(Cat2DbContext db, Student student)
        {
            var sessions = db.ExamSessions.ToList();
            var activeSession = sessions.FirstOrDefault(s => s.IsActive);

            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("── STUDENT RESULT SLIP ──");
                Console.ResetColor();
                Console.WriteLine($"  Name: {student.FullName}");
                Console.WriteLine($"  Reg : {student.RegistrationNumber}");
                Console.WriteLine($"  Prog: {student.Program?.Name} ({student.Program?.Faculty?.Code})");
                Console.WriteLine(new string('-', 60));

                Console.WriteLine("\n  Select Exam Session:");
                for (int i = 0; i < sessions.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {sessions[i].Name}{(sessions[i].IsActive ? " (Active)" : "")}");
                }
                Console.WriteLine("  0. ⬅️ Back");
                Console.Write("\n  Choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= sessions.Count)
                {
                    DisplayResults(db, student, sessions[choice - 1]);
                }
                else if (choice == 0) back = true;
            }
        }

        static void DisplayResults(Cat2DbContext db, Student student, ExamSession session)
        {
            Console.Clear();
            Console.WriteLine($"── RESULTS FOR: {session.Name} ──");
            var results = db.Results
                .Include(r => r.Unit)
                .Where(r => r.StudentId == student.Id && r.ExamSessionId == session.Id)
                .ToList();

            if (!results.Any())
            {
                Console.WriteLine("  No results recorded for this session.");
            }
            else
            {
                Console.WriteLine("{0,-10} {1,-30} {2,5} {3,5} {4,5} {5}", "Code", "Unit Name", "CAT", "Exam", "Total", "Grade");
                Console.WriteLine(new string('-', 70));
                foreach (var r in results)
                {
                    Console.WriteLine("{0,-10} {1,-30} {2,5} {3,5} {4,5} {5}", r.Unit?.Code, r.Unit?.Name, r.CatScore, r.ExamScore, r.TotalScore, r.Grade);
                }
                Console.WriteLine(new string('-', 70));
                Console.WriteLine($"  GPA: {CalculateGPA(results):F2}");
            }
            Console.WriteLine("\n  Press any key to return...");
            Console.ReadKey();
        }

        static double CalculateGPA(List<Result> results)
        {
            if (results.Count == 0) return 0;
            double totalPoints = 0;
            int totalCredits = 0;
            foreach (var r in results)
            {
                totalPoints += GetGradePoints(r.Grade) * r.Unit!.CreditHours;
                totalCredits += r.Unit.CreditHours;
            }
            return totalCredits > 0 ? totalPoints / totalCredits : 0;
        }

        static double GetGradePoints(string grade)
        {
            return grade switch
            {
                "A" => 4.0, "B+" => 3.5, "B" => 3.0, "C+" => 2.5, "C" => 2.0, "D+" => 1.5, "D" => 1.0, _ => 0.0
            };
        }

        static void Login(Cat2DbContext db)
        {
            Console.Clear();
            Console.WriteLine("── USER LOGIN ──");
            Console.Write("  Username: ");
            string username = Console.ReadLine() ?? "";
            Console.Write("  Password: ");
            string password = ReadPassword();

            string passHash = HashString(password);
            var user = db.AppUsers.FirstOrDefault(u => u.Username == username && u.PasswordHash == passHash);

            if (user != null)
            {
                CurrentUser = user;
                Console.WriteLine($"\n  ✔ Welcome, {user.FullName}!");
            }
            else
            {
                Console.WriteLine("\n  ✘ Invalid credentials!");
            }
            Console.ReadKey();
        }

        static void Register(Cat2DbContext db)
        {
            Console.Clear();
            Console.WriteLine("── USER REGISTRATION ──");
            Console.Write("  Full Name: ");
            string name = Console.ReadLine() ?? "";
            Console.Write("  Username : ");
            string username = Console.ReadLine() ?? "";

            if (db.AppUsers.Any(u => u.Username == username))
            {
                Console.WriteLine("\n  ✘ Username already exists!");
                Console.ReadKey();
                return;
            }

            Console.Write("  Password : ");
            string password = ReadPassword();

            db.AppUsers.Add(new AppUser
            {
                FullName = name,
                Username = username,
                PasswordHash = HashString(password),
                Role = "Student"
            });
            db.SaveChanges();

            Console.WriteLine("\n  ✔ Registration successful! You can now login.");
            Console.ReadKey();
        }

        static string HashString(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        static string ReadPassword()
        {
            string pass = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    pass += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                    {
                        pass = pass.Substring(0, (pass.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }
            } while (true);
            Console.WriteLine();
            return pass;
        }
    }
}
