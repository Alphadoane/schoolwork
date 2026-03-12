using System;

namespace EVMS
{
    /// <summary>
    /// Interactive driver program for the IIEC Electronic Voting Management System.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "IIEC – Electronic Voting Management System (EVMS)";
            VoterDatabase db = new VoterDatabase("voters.db");

            bool running = true;
            while (running)
            {
                PrintMenu();
                string input = Console.ReadLine();
                string choice = (input != null) ? input.Trim() : "";

                switch (choice)
                {
                    case "1":
                        AddVoter(db);
                        break;
                    case "2":
                        DeleteVoter(db);
                        break;
                    case "3":
                        DisplayVoter(db);
                        break;
                    case "4":
                        db.DisplayAllVoters();
                        break;
                    case "5":
                        Console.WriteLine("\n  Exiting EVMS. Goodbye!\n");
                        running = false;
                        break;
                    default:
                        Console.WriteLine("\n  ✘ Invalid option. Please enter 1–5.");
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
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.WriteLine("  ┃   IIEC – ELECTRONIC VOTING MANAGEMENT SYSTEM     ┃");
            Console.WriteLine("  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            Console.WriteLine("  ┃  1. 👤 Add New Voter                            ┃");
            Console.WriteLine("  ┃  2. 🗑️ Delete Voter                             ┃");
            Console.WriteLine("  ┃  3. 🔍 Display Voter Details                    ┃");
            Console.WriteLine("  ┃  4. 📋 Display All Voters                       ┃");
            Console.WriteLine("  ┃  5. ❌ Exit                                     ┃");
            Console.WriteLine("  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.ResetColor();
            Console.Write("  Select Option [1-5]: ");
        }

        static void AddVoter(VoterDatabase db)
        {
            Console.WriteLine("\n  ── ADD NEW VOTER ──────────────────────────────────");
            Console.Write("  Voter Card ID      : "); 
            string inputVid = Console.ReadLine();
            string vid = (inputVid != null) ? inputVid.Trim() : "";

            Console.Write("  National ID Number : "); 
            string inputNid = Console.ReadLine();
            string nid = (inputNid != null) ? inputNid.Trim() : "";

            Console.Write("  First Name         : "); 
            string inputFn = Console.ReadLine();
            string fn = (inputFn != null) ? inputFn.Trim() : "";

            Console.Write("  Middle Name        : "); 
            string inputMn = Console.ReadLine();
            string mn = (inputMn != null) ? inputMn.Trim() : "";

            Console.Write("  Surname            : "); 
            string inputSn = Console.ReadLine();
            string sn = (inputSn != null) ? inputSn.Trim() : "";

            Console.Write("  Polling Station    : "); 
            string inputPs = Console.ReadLine();
            string ps = (inputPs != null) ? inputPs.Trim() : "";

            Console.Write("  Date of Birth (dd-mm-yyyy): "); 
            string inputDob = Console.ReadLine();
            string dob = (inputDob != null) ? inputDob.Trim() : "";

            Console.Write("  Gender (Male/Female/Other): "); 
            string inputGen = Console.ReadLine();
            string gen = (inputGen != null) ? inputGen.Trim() : "";

            if (string.IsNullOrEmpty(vid) || string.IsNullOrEmpty(nid) ||
                string.IsNullOrEmpty(fn)  || string.IsNullOrEmpty(sn)  ||
                string.IsNullOrEmpty(ps)  || string.IsNullOrEmpty(dob) ||
                string.IsNullOrEmpty(gen))
            {
                Console.WriteLine("\n  ✘ All fields except Middle Name are required.");
                return;
            }

            Voter voter = new Voter(vid, nid, fn, mn, sn, ps, dob, gen);
            db.AddVoter(voter);
            voter.DisplayVoter();
        }

        static void DeleteVoter(VoterDatabase db)
        {
            Console.WriteLine("\n  ── DELETE VOTER ───────────────────────────────────");
            Console.Write("  Enter Voter Card ID to delete: ");
            string input = Console.ReadLine();
            string vid = (input != null) ? input.Trim() : "";
            if (!string.IsNullOrEmpty(vid))
                db.DeleteVoter(vid);
            else
                Console.WriteLine("\n  ✘ Voter Card ID cannot be empty.");
        }

        static void DisplayVoter(VoterDatabase db)
        {
            Console.WriteLine("\n  ── DISPLAY VOTER DETAILS ──────────────────────────");
            Console.Write("  Enter Voter Card ID: ");
            string input = Console.ReadLine();
            string vid = (input != null) ? input.Trim() : "";
            if (!string.IsNullOrEmpty(vid))
                db.DisplayVoter(vid);
            else
                Console.WriteLine("\n  ✘ Voter Card ID cannot be empty.");
        }
    }
}
