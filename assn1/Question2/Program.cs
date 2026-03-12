using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;

namespace Question2
{
    class Vehicle
    {
        private const double PROFIT_RATE = 0.15;
        private static string _connectionString = "Data Source=vehicles.db;Version=3;";

        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string EngineNumber { get; set; } = string.Empty;
        public double SalePrice { get; set; }

        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Vehicles (
                        EngineNumber TEXT PRIMARY KEY,
                        Make TEXT NOT NULL,
                        Model TEXT NOT NULL,
                        SalePrice REAL NOT NULL
                    );";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        public void SetVehicle()
        {
            Console.WriteLine("\n=== Enter Vehicle Details ===");
            Console.Write("  Make          : ");
            Make = Console.ReadLine() ?? string.Empty;
            Console.Write("  Model         : ");
            Model = Console.ReadLine() ?? string.Empty;
            Console.Write("  Engine Number : ");
            EngineNumber = Console.ReadLine() ?? string.Empty;
            Console.Write("  Sale Price    : ");
            if (double.TryParse(Console.ReadLine(), out double sp)) SalePrice = sp;
        }

        public double GetProfit()
        {
            return SalePrice * PROFIT_RATE;
        }

        public void SaveToDatabase()
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "INSERT OR REPLACE INTO Vehicles (EngineNumber, Make, Model, SalePrice) VALUES (@id, @make, @model, @price)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", EngineNumber);
                        cmd.Parameters.AddWithValue("@make", Make);
                        cmd.Parameters.AddWithValue("@model", Model);
                        cmd.Parameters.AddWithValue("@price", SalePrice);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("  [OK] Vehicle record saved to SQLite database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Could not save record: {ex.Message}");
            }
        }

        public static void DisplayAll()
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Vehicles";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n=== DT Dobie (K) Ltd – Vehicle Sales Database (SQLite) ===");
                        Console.WriteLine($"{"Make",-15} {"Model",-15} {"Engine No.",-20} {"Sale Price (KES)",-18} {"Profit (KES)",-18}");
                        Console.WriteLine(new string('-', 86));

                        int count = 0;
                        while (reader.Read())
                        {
                            string engineNumber = reader["EngineNumber"].ToString()?.Trim() ?? "";
                            string make = reader["Make"].ToString()?.Trim() ?? "";
                            string model = reader["Model"].ToString()?.Trim() ?? "";
                            double salePrice = Convert.ToDouble(reader["SalePrice"]);
                            double profit = salePrice * PROFIT_RATE;

                            Console.WriteLine($"{make,-15} {model,-15} {engineNumber,-20} {salePrice,-18:F2} {profit,-18:F2}");
                            count++;
                        }

                        if (count == 0)
                            Console.WriteLine("  [INFO] No records found.");
                        else
                        {
                            Console.WriteLine(new string('-', 86));
                            Console.WriteLine($"  Total vehicles: {count}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Could not read database: {ex.Message}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Vehicle.InitializeDatabase();

            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║   DT Dobie (K) Ltd – Vehicle Sales   ║");
            Console.WriteLine("╚══════════════════════════════════════╝");

            int choice;
            do
            {
                Console.WriteLine("\n--- MENU ---");
                Console.WriteLine("  1. Add a new vehicle");
                Console.WriteLine("  2. Display all vehicles");
                Console.WriteLine("  0. Exit");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    choice = -1;
                }

                switch (choice)
                {
                    case 1:
                        Vehicle v = new Vehicle();
                        v.SetVehicle();
                        Console.WriteLine("\n--- Vehicle Summary ---");
                        Console.WriteLine($"  Make          : {v.Make}");
                        Console.WriteLine($"  Model         : {v.Model}");
                        Console.WriteLine($"  Engine Number : {v.EngineNumber}");
                        Console.WriteLine($"  Sale Price    : KES {v.SalePrice:F2}");
                        Console.WriteLine($"  Profit (15%)  : KES {v.GetProfit():F2}");
                        v.SaveToDatabase();
                        break;
                    case 2:
                        Vehicle.DisplayAll();
                        break;
                    case 0:
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("  Invalid option. Try again.");
                        break;
                }
            } while (choice != 0);
        }
    }
}
