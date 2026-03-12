using System;

namespace EVMS
{
    /// <summary>
    /// Represents a registered voter in the IIEC Electronic Voting Management System.
    /// </summary>
    public class Voter
    {
        // ── Fields ──────────────────────────────────────────────────────────────
        public string VoterCardID     { get; set; }
        public string NationalID      { get; set; }
        public string FirstName       { get; set; }
        public string MiddleName      { get; set; }
        public string Surname         { get; set; }
        public string PollingStation  { get; set; }
        public string DateOfBirth     { get; set; }   // format: dd-mm-yyyy
        public string Gender          { get; set; }   // Male / Female / Other

        // ── Constructor ─────────────────────────────────────────────────────────
        public Voter(string voterCardID, string nationalID, string firstName,
                     string middleName, string surname, string pollingStation,
                     string dateOfBirth, string gender)
        {
            VoterCardID    = voterCardID;
            NationalID     = nationalID;
            FirstName      = firstName;
            MiddleName     = middleName;
            Surname        = surname;
            PollingStation = pollingStation;
            DateOfBirth    = dateOfBirth;
            Gender         = gender;
        }

        // ── Display Method ───────────────────────────────────────────────────────
        public void DisplayVoter()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("  ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┓");
            Console.WriteLine("  ┃          VOTER DETAILS – IIEC EVMS               ┃");
            Console.WriteLine("  ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┫");
            Console.WriteLine(string.Format("  ┃  Voter Card ID    : {0,-29}┃", VoterCardID));
            Console.WriteLine(string.Format("  ┃  National ID      : {0,-29}┃", NationalID));
            Console.WriteLine(string.Format("  ┃  First Name       : {0,-29}┃", FirstName));
            Console.WriteLine(string.Format("  ┃  Middle Name      : {0,-29}┃", MiddleName));
            Console.WriteLine(string.Format("  ┃  Surname          : {0,-29}┃", Surname));
            Console.WriteLine(string.Format("  ┃  Polling Station  : {0,-29}┃", PollingStation));
            Console.WriteLine(string.Format("  ┃  Date of Birth    : {0,-29}┃", DateOfBirth));
            Console.WriteLine(string.Format("  ┃  Gender           : {0,-29}┃", Gender));
            Console.WriteLine("  ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┛");
            Console.ResetColor();
        }
    }
}
