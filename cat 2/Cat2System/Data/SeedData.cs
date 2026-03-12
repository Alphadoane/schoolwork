using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cat2System.Data;
using Cat2System.Models;

namespace Cat2System.Data
{
    public static class SeedData
    {
        public static void Initialize(Cat2DbContext context)
        {
            context.Database.EnsureCreated();

                if (context.Faculties.Any()) return;

                var faculty = new Faculty { Name = "Science and Technology", Code = "FST" };
                context.Faculties.Add(faculty);
                context.SaveChanges();

                var program = new AppProgram { FacultyId = faculty.Id, Name = "Bachelor of Science in Information Technology", Code = "BSIT" };
                context.Programs.Add(program);
                context.SaveChanges();

                var student = new Student 
                { 
                    RegistrationNumber = "BIT/001/2022", 
                    FirstName = "John", 
                    LastName = "Doe", 
                    Email = "john.doe@example.com", 
                    ProgramId = program.Id, 
                    DateOfAdmission = DateTime.Now.AddYears(-2) 
                };
                context.Students.Add(student);
                context.SaveChanges();

                var session = new ExamSession { Name = "January - April 2026", Year = 2026, Semester = 1, IsActive = true };
                context.ExamSessions.Add(session);
                context.SaveChanges();

                var unit = new Unit { Code = "IT301", Name = "Advanced Application Development", CreditHours = 3, ProgramId = program.Id, Year = 3, Semester = 1 };
                context.Units.Add(unit);
                context.SaveChanges();

                context.Results.Add(new Result { StudentId = student.Id, UnitId = unit.Id, ExamSessionId = session.Id, CatScore = 25, ExamScore = 65 });
                context.SaveChanges();
        }
    }
}
