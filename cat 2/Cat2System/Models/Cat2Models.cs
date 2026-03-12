using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat2System.Models
{
    public class Faculty
    {
        public int Id { get; set; }
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(10)]
        public string Code { get; set; } = string.Empty;

        public ICollection<AppProgram> Programs { get; set; } = new List<AppProgram>();
    }

    public class AppProgram
    {
        public int Id { get; set; }
        public int FacultyId { get; set; }
        public Faculty? Faculty { get; set; }
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(20)]
        public string Code { get; set; } = string.Empty;

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Unit> Units { get; set; } = new List<Unit>();
    }

    public class Student
    {
        public int Id { get; set; }
        [Required, StringLength(30)]
        public string RegistrationNumber { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        public int? ProgramId { get; set; }
        public AppProgram? Program { get; set; }
        public int YearOfStudy { get; set; } = 1;
        public DateTime DateOfAdmission { get; set; }
        public string? PhotoPath { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }

    public class Unit
    {
        public int Id { get; set; }
        [Required, StringLength(20)]
        public string Code { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string Name { get; set; } = string.Empty;
        [Range(1, 6)]
        public int CreditHours { get; set; } = 3;
        public int ProgramId { get; set; }
        public AppProgram? Program { get; set; }
        public int Year { get; set; } = 1;
        public int Semester { get; set; } = 1;

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }

    public class ExamSession
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Semester { get; set; }
        public bool IsActive { get; set; } = false;

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }

    public class Result
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int UnitId { get; set; }
        public Unit? Unit { get; set; }
        public int ExamSessionId { get; set; }
        public ExamSession? ExamSession { get; set; }
        [Range(0, 30)]
        public double? CatScore { get; set; }
        [Range(0, 70)]
        public double? ExamScore { get; set; }

        public double TotalScore => (CatScore ?? 0) + (ExamScore ?? 0);

        public string Grade
        {
            get
            {
                var score = TotalScore;
                if (score >= 70) return "A";
                if (score >= 60) return "B+";
                if (score >= 55) return "B";
                if (score >= 50) return "C+";
                if (score >= 45) return "C";
                if (score >= 40) return "D+";
                if (score >= 35) return "D";
                return "E";
            }
        }

        public string Status => TotalScore >= 40 ? "PASS" : "FAIL";
    }

    public class AppUser
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Student"; // Admin, Student
    }
}
