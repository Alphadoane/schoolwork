using System;

namespace BongaPoints
{
    /// <summary>
    /// Represents a Safaricom subscriber in the Electronic Reward System.
    /// </summary>
    public class Subscriber
    {
        // ── Fields ──────────────────────────────────────────────────────────────
        public string Name          { get; private set; }
        public string PhoneNumber   { get; private set; }
        public double AirtimeAmount { get; private set; }

        // ── Constructor ─────────────────────────────────────────────────────────
        public Subscriber(string name, string phoneNumber, double airtimeAmount)
        {
            Name          = name;
            PhoneNumber   = phoneNumber;
            AirtimeAmount = airtimeAmount;
        }

        // ── Bonus Points Calculation ─────────────────────────────────────────────
        /// <summary>
        /// Calculates and returns the Bonga bonus points based on airtime amount.
        /// </summary>
        public int compute_bonuspoints()
        {
            if (AirtimeAmount >= 2000.00)
                return 500;
            else if (AirtimeAmount >= 1000.00)
                return 300;
            else if (AirtimeAmount >= 500.00)
                return 100;
            else if (AirtimeAmount >= 100.00)
                return 50;
            else
                return 0;
        }

        // ── Display Method ───────────────────────────────────────────────────────
        public void DisplayInfo()
        {
            int points = compute_bonuspoints();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.WriteLine("  ┃        SAFARICOM BONGA POINTS RECORD             ┃");
            Console.WriteLine("  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            Console.WriteLine(string.Format("  ┃  Name   : {0,-35}┃", Name.ToUpper()));
            Console.WriteLine(string.Format("  ┃  Phone  : {0,-35}┃", PhoneNumber));
            Console.WriteLine(string.Format("  ┃  Points : {0,-35}┃", points));
            Console.WriteLine("  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            Console.WriteLine("  ┃  STAY WITH SAFARICOM. THE BETTER OPTION!         ┃");
            Console.WriteLine("  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
