using Microsoft.EntityFrameworkCore;
using Cat2System.Models;

namespace Cat2System.Data
{
    public class Cat2DbContext : DbContext
    {
        public Cat2DbContext(DbContextOptions<Cat2DbContext> options) : base(options) { }

        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<AppProgram> Programs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Faculty>().HasIndex(f => f.Code).IsUnique();
            modelBuilder.Entity<AppProgram>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<Student>().HasIndex(s => s.RegistrationNumber).IsUnique();
            modelBuilder.Entity<Unit>().HasIndex(u => u.Code).IsUnique();
            modelBuilder.Entity<ExamSession>().HasIndex(e => new { e.Year, e.Semester }).IsUnique();
        }
    }
}
