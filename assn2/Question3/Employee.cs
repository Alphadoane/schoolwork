using System;

namespace FortuneBS
{
    /// <summary>
    /// Represents an employee of Fortune Business Systems Ltd.
    /// </summary>
    public class Employee
    {
        // ── Fields ──────────────────────────────────────────────────────────────
        public string EmployeeID     { get; set; }
        public string FirstName      { get; set; }
        public string SecondName     { get; set; }
        public string Surname        { get; set; }
        public string Gender         { get; set; }
        public string DateOfBirth    { get; set; }   // format: dd-mm-yyyy
        public double MonthlySalary  { get; set; }

        // ── Constructor ─────────────────────────────────────────────────────────
        public Employee(string employeeID, string firstName, string secondName,
                        string surname, string gender, string dateOfBirth,
                        double monthlySalary)
        {
            EmployeeID    = employeeID;
            FirstName     = firstName;
            SecondName    = secondName;
            Surname       = surname;
            Gender        = gender;
            DateOfBirth   = dateOfBirth;
            MonthlySalary = monthlySalary;
        }

        // ── Display Method ───────────────────────────────────────────────────────
        public void show_employee()
        {
            double pension = compute_pension(this);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.WriteLine("  ┃       FORTUNE BUSINESS SYSTEMS – EMPLOYEE DETAILS      ┃");
            Console.WriteLine("  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            Console.WriteLine(string.Format("  ┃  Employee ID      : {0,-37}┃", EmployeeID));
            Console.WriteLine(string.Format("  ┃  First Name       : {0,-37}┃", FirstName));
            Console.WriteLine(string.Format("  ┃  Second Name      : {0,-37}┃", SecondName));
            Console.WriteLine(string.Format("  ┃  Surname          : {0,-37}┃", Surname));
            Console.WriteLine(string.Format("  ┃  Gender           : {0,-37}┃", Gender));
            Console.WriteLine(string.Format("  ┃  Date of Birth    : {0,-37}┃", DateOfBirth));
            Console.WriteLine(string.Format("  ┃  Monthly Salary   : Ksh. {0,-33:F2}┃", MonthlySalary));
            Console.WriteLine(string.Format("  ┃  Pension (5%)     : Ksh. {0,-33:F2}┃", pension));
            Console.WriteLine("  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.ResetColor();
        }

        // ── Friend-style Static Pension Function ─────────────────────────────────
        /// <summary>
        /// Computes 5% pension contribution for the given employee.
        /// Implemented as a static method (C# equivalent of a C++ friend function).
        /// </summary>
        public static double compute_pension(Employee emp)
        {
            return emp.MonthlySalary * 0.05;
        }
    }
}
