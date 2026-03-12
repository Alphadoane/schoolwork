using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;

namespace Question1
{
    class Book
    {
        private static string _connectionString = "Data Source=books.db;Version=3;";

        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string BookNumber { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Copies { get; set; }

        public static void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Books (
                        BookNumber TEXT PRIMARY KEY,
                        Author TEXT NOT NULL,
                        Title TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Copies INTEGER NOT NULL
                    );";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        public void Insert()
        {
            Console.WriteLine("\n=== Add New Book ===");
            Console.Write("  Author      : ");
            Author = Console.ReadLine() ?? string.Empty;
            Console.Write("  Title       : ");
            Title = Console.ReadLine() ?? string.Empty;
            Console.Write("  Book Number : ");
            BookNumber = Console.ReadLine() ?? string.Empty;
            Console.Write("  Price (KES) : ");
            if (double.TryParse(Console.ReadLine(), out double p)) Price = p;
            Console.Write("  Copies      : ");
            if (int.TryParse(Console.ReadLine(), out int c)) Copies = c;

            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "INSERT OR REPLACE INTO Books (BookNumber, Author, Title, Price, Copies) VALUES (@id, @author, @title, @price, @copies)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", BookNumber);
                        cmd.Parameters.AddWithValue("@author", Author);
                        cmd.Parameters.AddWithValue("@title", Title);
                        cmd.Parameters.AddWithValue("@price", Price);
                        cmd.Parameters.AddWithValue("@copies", Copies);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("  [OK] Book record saved successfully to SQLite.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Could not save record: {ex.Message}");
            }
        }

        public static void Display()
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Books";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n=== Booker University Library – Book Inventory (SQLite) ===");
                        Console.WriteLine($"{"Book No.",-12} {"Title",-40} {"Author",-25} {"Price(KES)",-12} {"Copies",-8}");
                        Console.WriteLine(new string('-', 97));

                        int count = 0;
                        while (reader.Read())
                        {
                            string bookNumber = reader["BookNumber"].ToString()?.Trim() ?? "";
                            string author = reader["Author"].ToString()?.Trim() ?? "";
                            string title = reader["Title"].ToString()?.Trim() ?? "";
                            double price = Convert.ToDouble(reader["Price"]);
                            int copies = Convert.ToInt32(reader["Copies"]);

                            Console.WriteLine($"{bookNumber,-12} {title,-40} {author,-25} {price,-12:F2} {copies,-8}");
                            count++;
                        }

                        if (count == 0)
                            Console.WriteLine("  [INFO] No records found.");
                        else
                        {
                            Console.WriteLine(new string('-', 97));
                            Console.WriteLine($"  Total books: {count}");
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
            Book.InitializeDatabase();

            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║  Booker University Library System    ║");
            Console.WriteLine("╚══════════════════════════════════════╝");

            int choice;
            do
            {
                Console.WriteLine("\n--- MENU ---");
                Console.WriteLine("  1. Add a new book");
                Console.WriteLine("  2. Display all books");
                Console.WriteLine("  0. Exit");
                Console.Write("Enter choice: ");

                if (!int.TryParse(Console.ReadLine(), out choice))
                {
                    choice = -1;
                }

                switch (choice)
                {
                    case 1:
                        new Book().Insert();
                        break;
                    case 2:
                        Book.Display();
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
