using System;
using System.Data.SQLite;
using System.IO;

namespace BongaPoints
{
    /// <summary>
    /// Handles all SQLite database operations for the Bonga Points subscriber records.
    /// </summary>
    public class SubscriberDatabase
    {
        private readonly string _connectionString;

        public SubscriberDatabase(string dbName = "subscribers.db")
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
                    CREATE TABLE IF NOT EXISTS Subscribers (
                        Id            INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name          TEXT    NOT NULL,
                        PhoneNumber   TEXT    NOT NULL,
                        AirtimeAmount REAL    NOT NULL,
                        BonusPoints   INTEGER NOT NULL
                    );";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        public void AddSubscriber(Subscriber s)
        {
            int points = s.compute_bonuspoints();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    INSERT INTO Subscribers (Name, PhoneNumber, AirtimeAmount, BonusPoints)
                    VALUES (@name, @phone, @airtime, @points);";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name",    s.Name);
                    cmd.Parameters.AddWithValue("@phone",   s.PhoneNumber);
                    cmd.Parameters.AddWithValue("@airtime", s.AirtimeAmount);
                    cmd.Parameters.AddWithValue("@points",  points);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("\n  ✔ Subscriber '" + s.Name + "' saved to database.");
        }

        public void DisplayAllSubscribers()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Subscribers ORDER BY Id;";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    bool any = false;
                    Console.WriteLine();
                    Console.WriteLine("  ══════════════════════════════════════════════════════════════");
                    Console.WriteLine("  ALL SAFARICOM SUBSCRIBERS – BONGA POINTS RECORD");
                    Console.WriteLine("  ══════════════════════════════════════════════════════════════");
                    while (reader.Read())
                    {
                        any = true;
                        string name    = reader["Name"].ToString();
                        string phone   = reader["PhoneNumber"].ToString();
                        double airtime = Convert.ToDouble(reader["AirtimeAmount"]);
                        int    points  = Convert.ToInt32(reader["BonusPoints"]);
                        Console.WriteLine("  " + name.ToUpper() + " :(PHONE NO:" + phone + "): AWARDED " + points + " BONGA POINTS.");
                        Console.WriteLine("  STAY WITH SAFARICOM. THE BETTER OPTION!");
                        Console.WriteLine(string.Format("  [Airtime: Ksh. {0:F2}]", airtime));
                        Console.WriteLine();
                    }
                    if (!any)
                        Console.WriteLine("  No subscribers recorded yet.");
                    Console.WriteLine("  ══════════════════════════════════════════════════════════════");
                }
            }
        }
    }
}
