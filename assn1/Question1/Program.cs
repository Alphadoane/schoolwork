using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Question1
{
    class Book
    {
        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string BookNumber { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Copies { get; set; }

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

            // Append record to binary file
            try
            {
                using (FileStream fs = new FileStream("books.dat", FileMode.Append, FileAccess.Write))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(Author.PadRight(50).ToCharArray());
                    writer.Write(Title.PadRight(100).ToCharArray());
                    writer.Write(BookNumber.PadRight(20).ToCharArray());
                    writer.Write(Price);
                    writer.Write(Copies);
                }
                Console.WriteLine("  [OK] Book record saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [ERROR] Could not save record: {ex.Message}");
            }
        }

        public static void Display()
        {
            if (!File.Exists("books.dat"))
            {
                Console.WriteLine("\n  [INFO] No records found. Database is empty.");
                return;
            }

            try
            {
                using (FileStream fs = new FileStream("books.dat", FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    Console.WriteLine("\n=== Booker University Library – Book Inventory ===");
                    Console.WriteLine($"{"Book No.",-12} {"Title",-40} {"Author",-25} {"Price(KES)",-12} {"Copies",-8}");
                    Console.WriteLine(new string('-', 97));

                    int count = 0;
                    while (fs.Position < fs.Length)
                    {
                        string author = new string(reader.ReadChars(50)).TrimEnd();
                        string title = new string(reader.ReadChars(100)).TrimEnd();
                        string bookNumber = new string(reader.ReadChars(20)).TrimEnd();
                        double price = reader.ReadDouble();
                        int copies = reader.ReadInt32();

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
