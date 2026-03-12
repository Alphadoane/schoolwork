using System;
using System.Collections.Generic;
using System.Linq;
using Cat2System.Data;
using Cat2System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cat2System
{
    class Program
    {
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
                Console.WriteLine("\n  1. 🔍 Search Student & View Result Slip");
                Console.WriteLine("  2. 📋 View All Students");
                Console.WriteLine("  0. ❌ Exit");
                Console.Write("\n  Select Option: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1": SearchStudent(db); break;
                    case "2": ViewAllStudents(db); break;
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
    }
}
