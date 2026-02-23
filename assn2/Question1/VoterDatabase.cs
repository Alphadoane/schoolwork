using System;
using System.Data.SQLite;
using System.IO;

namespace EVMS
{
    /// <summary>
    /// Handles all SQLite database operations for the EVMS voter records.
    /// </summary>
    public class VoterDatabase
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public VoterDatabase(string dbPath = "voters.db")
        {
            _dbPath = dbPath;
            _connectionString = "Data Source=" + _dbPath + ";Version=3;";
            InitializeDatabase();
        }

        // ── Create table if not exists ───────────────────────────────────────────
        private void InitializeDatabase()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Voters (
                        VoterCardID    TEXT PRIMARY KEY,
                        NationalID     TEXT NOT NULL UNIQUE,
                        FirstName      TEXT NOT NULL,
                        MiddleName     TEXT,
                        Surname        TEXT NOT NULL,
                        PollingStation TEXT NOT NULL,
                        DateOfBirth    TEXT NOT NULL,
                        Gender         TEXT NOT NULL
                    );";
                using (var cmd = new SQLiteCommand(sql, conn))
                    cmd.ExecuteNonQuery();
            }
        }

        // ── Add a voter ──────────────────────────────────────────────────────────
        public bool AddVoter(Voter v)
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO Voters
                            (VoterCardID, NationalID, FirstName, MiddleName,
                             Surname, PollingStation, DateOfBirth, Gender)
                        VALUES
                            (@vid, @nid, @fn, @mn, @sn, @ps, @dob, @gen);";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@vid", v.VoterCardID);
                        cmd.Parameters.AddWithValue("@nid", v.NationalID);
                        cmd.Parameters.AddWithValue("@fn",  v.FirstName);
                        cmd.Parameters.AddWithValue("@mn",  v.MiddleName);
                        cmd.Parameters.AddWithValue("@sn",  v.Surname);
                        cmd.Parameters.AddWithValue("@ps",  v.PollingStation);
                        cmd.Parameters.AddWithValue("@dob", v.DateOfBirth);
                        cmd.Parameters.AddWithValue("@gen", v.Gender);
                        cmd.ExecuteNonQuery();
                    }
                }
                Console.WriteLine("\n  ✔ Voter '" + v.FirstName + " " + v.Surname + "' added successfully.");
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("\n  ✘ Error adding voter: " + ex.Message);
                return false;
            }
        }

        // ── Delete a voter ───────────────────────────────────────────────────────
        public bool DeleteVoter(string voterCardID)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Voters WHERE VoterCardID = @vid;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@vid", voterCardID);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        Console.WriteLine("\n  ✔ Voter with Card ID '" + voterCardID + "' deleted.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("\n  ✘ No voter found with Card ID '" + voterCardID + "'.");
                        return false;
                    }
                }
            }
        }

        // ── Display a single voter ───────────────────────────────────────────────
        public void DisplayVoter(string voterCardID)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Voters WHERE VoterCardID = @vid;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@vid", voterCardID);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var v = new Voter(
                                reader["VoterCardID"].ToString(),
                                reader["NationalID"].ToString(),
                                reader["FirstName"].ToString(),
                                reader["MiddleName"].ToString(),
                                reader["Surname"].ToString(),
                                reader["PollingStation"].ToString(),
                                reader["DateOfBirth"].ToString(),
                                reader["Gender"].ToString()
                            );
                            v.DisplayVoter();
                        }
                        else
                        {
                            Console.WriteLine("\n  ✘ No voter found with Card ID '" + voterCardID + "'.");
                        }
                    }
                }
            }
        }

        // ── Display all voters ───────────────────────────────────────────────────
        public void DisplayAllVoters()
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM Voters ORDER BY Surname;";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    bool any = false;
                    while (reader.Read())
                    {
                        any = true;
                        var v = new Voter(
                            reader["VoterCardID"].ToString(),
                            reader["NationalID"].ToString(),
                            reader["FirstName"].ToString(),
                            reader["MiddleName"].ToString(),
                            reader["Surname"].ToString(),
                            reader["PollingStation"].ToString(),
                            reader["DateOfBirth"].ToString(),
                            reader["Gender"].ToString()
                        );
                        v.DisplayVoter();
                    }
                    if (!any)
                        Console.WriteLine("\n  No voters registered yet.");
                }
            }
        }
    }
}
