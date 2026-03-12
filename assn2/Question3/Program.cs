using System;

namespace FortuneBS
{
    /// <summary>
    /// Interactive driver program for the Fortune Business Systems Employee Management System.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Fortune Business Systems Ltd – Employee Management";
            EmployeeDatabase db = new EmployeeDatabase("employees.db");

            bool running = true;
            while (running)
            {
                PrintMenu();
                string inputStr = Console.ReadLine();
                string choice = (inputStr != null) ? inputStr.Trim() : "";

                switch (choice)
                {
                    case "1":
                        AddEmployee(db);
                        break;
                    case "2":
                        db.DisplayAllEmployees();
                        break;
                    case "3":
                        Console.WriteLine("\n  Exiting Fortune Business Systems. Goodbye!\n");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("\n  ✘ Invalid option. Please enter 1–3.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine("\n  Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void PrintMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine();
            Console.WriteLine("  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.WriteLine("  ┃   FORTUNE BUSINESS SYSTEMS – EMPLOYEE MANAGEMENT       ┃");
            Console.WriteLine("  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            Console.WriteLine("  ┃  1. 👤 Add New Employee                               ┃");
            Console.WriteLine("  ┃  2. 📋 Display All Employees                          ┃");
            Console.WriteLine("  ┃  3. ❌ Exit                                           ┃");
            Console.WriteLine("  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.ResetColor();
            Console.Write("  Select Action: ");
        }

        static void AddEmployee(EmployeeDatabase db)
        {
            Console.WriteLine("\n  ── ADD NEW EMPLOYEE ───────────────────────────────────────");
            Console.Write("  Employee ID            : "); 
            string inputId = Console.ReadLine();
            string id = (inputId != null) ? inputId.Trim() : "";

            Console.Write("  First Name             : "); 
            string inputFn = Console.ReadLine();
            string fn = (inputFn != null) ? inputFn.Trim() : "";

            Console.Write("  Second Name            : "); 
            string inputSn2 = Console.ReadLine();
            string sn2 = (inputSn2 != null) ? inputSn2.Trim() : "";

            Console.Write("  Surname                : "); 
            string inputSn = Console.ReadLine();
            string sn = (inputSn != null) ? inputSn.Trim() : "";

            Console.Write("  Gender (Male/Female)   : "); 
            string inputGen = Console.ReadLine();
            string gen = (inputGen != null) ? inputGen.Trim() : "";

            Console.Write("  Date of Birth (dd-mm-yyyy): "); 
            string inputDob = Console.ReadLine();
            string dob = (inputDob != null) ? inputDob.Trim() : "";

            Console.Write("  Monthly Basic Salary (Ksh.): ");
            string inputSal = Console.ReadLine();
            double salary = 0;
            if (!double.TryParse((inputSal != null) ? inputSal.Trim() : "", out salary))
            {
                Console.WriteLine("\n  ✘ Invalid salary. Please enter a numeric value.");
                return;
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(fn) ||
                string.IsNullOrEmpty(sn) || string.IsNullOrEmpty(gen) ||
                string.IsNullOrEmpty(dob))
            {
                Console.WriteLine("\n  ✘ All fields except Second Name are required.");
                return;
            }

            // Create employee object using constructor (as required by 3a(i))
            Employee emp_obj = new Employee(id, fn, sn2, sn, gen, dob, salary);

            // Display employee info using show_employee()
            emp_obj.show_employee();

            // Compute pension using the friend-style static function
            double pension = Employee.compute_pension(emp_obj);
            Console.WriteLine(string.Format("\n  Pension Contribution (5%): Ksh. {0:F2}", pension));

            // Save to database
            db.AddEmployee(emp_obj);
        }
    }
}
