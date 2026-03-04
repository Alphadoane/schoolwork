using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Question2
{
    class Vehicle
    {
        private const double PROFIT_RATE = 0.15;

        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string EngineNumber { get; set; } = string.Empty;
        public double SalePrice { get; set; }

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
                using (FileStream fs = new FileStream("vehicles.dat", FileMode.Append, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(Make.PadRight(30).ToCharArray());
                    writer.Write(Model.PadRight(30).ToCharArray());
                    writer.Write(EngineNumber.PadRight(20).ToCharArray());
                    writer.Write(SalePrice);
                }
                Console.WriteLine("  [OK] Vehicle record saved to database.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Could not save record: {ex.Message}");
            }
        }

        public static void DisplayAll()
        {
            if (!File.Exists("vehicles.dat"))
            {
                Console.WriteLine("\n  [INFO] No records found. Database is empty.");
                return;
            }

            try
            {
                using (FileStream fs = new FileStream("vehicles.dat", FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    Console.WriteLine("\n=== DT Dobie (K) Ltd – Vehicle Sales Database ===");
                    Console.WriteLine($"{"Make",-15} {"Model",-15} {"Engine No.",-20} {"Sale Price (KES)",-18} {"Profit (KES)",-18}");
                    Console.WriteLine(new string('-', 86));

                    int count = 0;
                    while (fs.Position < fs.Length)
                    {
                        string make = new string(reader.ReadChars(30)).TrimEnd();
                        string model = new string(reader.ReadChars(30)).TrimEnd();
                        string engineNumber = new string(reader.ReadChars(20)).TrimEnd();
                        double salePrice = reader.ReadDouble();
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
