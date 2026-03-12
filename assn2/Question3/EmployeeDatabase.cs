using System;
using System.Data.SQLite;
using System.IO;

namespace FortuneBS
{
    /// <summary>
    /// Handles all SQLite database operations for Fortune Business Systems employee records.
    /// </summary>
    public class EmployeeDatabase
    {
        private readonly string _connectionString;

        public EmployeeDatabase(string dbName = "employees.db")
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbName);
            _connectionString = "Data Source=" + dbPath + ";Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Employees (
                        EmployeeID    TEXT PRIMARY KEY,
                        FirstName     TEXT NOT NULL,
                        SecondName    TEXT,
                        Surname       TEXT NOT NULL,
                        Gender        TEXT NOT NULL,
                        DateOfBirth   TEXT NOT NULL,
                        MonthlySalary REAL NOT NULL,
                        Pension       REAL NOT NULL
                    );";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        public bool AddEmployee(Employee e)
        {
            try
            {
                double pension = Employee.compute_pension(e);
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO Employees
                            (EmployeeID, FirstName, SecondName, Surname,
                             Gender, DateOfBirth, MonthlySalary, Pension)
                        VALUES
                            (@id, @fn, @sn2, @sn, @gen, @dob, @sal, @pen);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id",  e.EmployeeID);
                        cmd.Parameters.AddWithValue("@fn",  e.FirstName);
                        cmd.Parameters.AddWithValue("@sn2", e.SecondName);
                        cmd.Parameters.AddWithValue("@sn",  e.Surname);
                        cmd.Parameters.AddWithValue("@gen", e.Gender);
                        cmd.Parameters.AddWithValue("@dob", e.DateOfBirth);
                        cmd.Parameters.AddWithValue("@sal", e.MonthlySalary);
                        cmd.Parameters.AddWithValue("@pen", pension);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("\n  ✔ Employee '" + e.FirstName + " " + e.Surname + "' saved to database.");
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("\n  ✘ Error: " + ex.Message);
                return false;
            }
        }

        public void DisplayAllEmployees()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Employees ORDER BY Surname;";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    bool any = false;
                    while (reader.Read())
                    {
                        any = true;
                        var emp = new Employee(
                            reader["EmployeeID"].ToString(),
                            reader["FirstName"].ToString(),
                            reader["SecondName"].ToString(),
                            reader["Surname"].ToString(),
                            reader["Gender"].ToString(),
                            reader["DateOfBirth"].ToString(),
                            Convert.ToDouble(reader["MonthlySalary"])
                        );
                        emp.show_employee();
                    }
                    if (!any)
                        Console.WriteLine("\n  No employees recorded yet.");
                }
            }
        }
    }
}
