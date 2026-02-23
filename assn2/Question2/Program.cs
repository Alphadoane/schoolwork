using System;

namespace BongaPoints
{
    /// <summary>
    /// Interactive driver program for the Safaricom Electronic Reward System.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Safaricom – Electronic Reward System (Bonga Points)";
            SubscriberDatabase db = new SubscriberDatabase("subscribers.db");

            bool running = true;
            while (running)
            {
                PrintMenu();
                string input = Console.ReadLine();
                string choice = (input != null) ? input.Trim() : "";

                switch (choice)
                {
                    case "1":
                        AddSubscriber(db);
                        break;
                    case "2":
                        db.DisplayAllSubscribers();
                        break;
                    case "3":
                        Console.WriteLine("\n  Exiting. Thank you for using Safaricom!\n");
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
            Console.WriteLine();
            Console.WriteLine("  ╔══════════════════════════════════════════════════╗");
            Console.WriteLine("  ║   SAFARICOM – Electronic Reward System           ║");
            Console.WriteLine("  ╠══════════════════════════════════════════════════╣");
            Console.WriteLine("  ║  1. Add Subscriber & Compute Bonga Points        ║");
            Console.WriteLine("  ║  2. Display All Subscribers                      ║");
            Console.WriteLine("  ║  3. Exit                                         ║");
            Console.WriteLine("  ╚══════════════════════════════════════════════════╝");
            Console.Write("  Enter option: ");
        }

        static void AddSubscriber(SubscriberDatabase db)
        {
            Console.WriteLine("\n  ── ADD SUBSCRIBER ─────────────────────────────────");
            Console.Write("  Full Name        : "); 
            string inputName = Console.ReadLine();
            string name = (inputName != null) ? inputName.Trim() : "";

            Console.Write("  Phone Number     : "); 
            string inputPhone = Console.ReadLine();
            string phone = (inputPhone != null) ? inputPhone.Trim() : "";

            Console.Write("  Airtime Amount (Ksh.): ");
            string inputAirtime = Console.ReadLine();
            double airtime = 0;
            if (!double.TryParse((inputAirtime != null) ? inputAirtime.Trim() : "", out airtime))
            {
                Console.WriteLine("\n  ✘ Invalid airtime amount. Please enter a number.");
                return;
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
            {
                Console.WriteLine("\n  ✘ Name and Phone Number are required.");
                return;
            }

            Subscriber sub = new Subscriber(name, phone, airtime);
            sub.DisplayInfo();
            db.AddSubscriber(sub);
        }
    }
}
