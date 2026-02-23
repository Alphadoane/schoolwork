"""
database.py – SQLite database layer for the ATM Management System.
Handles schema creation, connection management, and seed data.
"""

import sqlite3
import hashlib
import os
from datetime import datetime, date

DB_PATH = os.path.join(os.path.dirname(__file__), "atm.db")


def get_connection():
    """Return a new SQLite connection with row_factory set."""
    conn = sqlite3.connect(DB_PATH)
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA foreign_keys = ON")
    return conn


def hash_pin(pin: str) -> str:
    return hashlib.sha256(pin.encode()).hexdigest()


def init_db():
    """Create all tables if they don't exist, then seed demo data."""
    conn = get_connection()
    cur = conn.cursor()

    # ── accounts ──────────────────────────────────────────────────────────────
    cur.execute("""
        CREATE TABLE IF NOT EXISTS accounts (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            account_number  TEXT    NOT NULL UNIQUE,
            holder_name     TEXT    NOT NULL,
            balance         REAL    NOT NULL DEFAULT 0.0,
            account_type    TEXT    NOT NULL DEFAULT 'Savings',
            status          TEXT    NOT NULL DEFAULT 'Active',
            created_at      TEXT    NOT NULL DEFAULT (datetime('now'))
        )
    """)

    # ── cards ─────────────────────────────────────────────────────────────────
    cur.execute("""
        CREATE TABLE IF NOT EXISTS cards (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            card_number TEXT    NOT NULL UNIQUE,
            account_id  INTEGER NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
            pin_hash    TEXT    NOT NULL,
            expiry_date TEXT    NOT NULL,
            is_active   INTEGER NOT NULL DEFAULT 1,
            created_at  TEXT    NOT NULL DEFAULT (datetime('now'))
        )
    """)

    # ── transactions ──────────────────────────────────────────────────────────
    cur.execute("""
        CREATE TABLE IF NOT EXISTS transactions (
            id            INTEGER PRIMARY KEY AUTOINCREMENT,
            account_id    INTEGER NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
            type          TEXT    NOT NULL,
            amount        REAL    NOT NULL,
            balance_after REAL    NOT NULL,
            description   TEXT,
            timestamp     TEXT    NOT NULL DEFAULT (datetime('now'))
        )
    """)

    # ── atm_machines ──────────────────────────────────────────────────────────
    cur.execute("""
        CREATE TABLE IF NOT EXISTS atm_machines (
            id             INTEGER PRIMARY KEY AUTOINCREMENT,
            machine_id     TEXT    NOT NULL UNIQUE,
            location       TEXT    NOT NULL,
            cash_available REAL    NOT NULL DEFAULT 0.0,
            status         TEXT    NOT NULL DEFAULT 'Online',
            last_serviced  TEXT
        )
    """)

    # ── admins ────────────────────────────────────────────────────────────────
    cur.execute("""
        CREATE TABLE IF NOT EXISTS admins (
            id            INTEGER PRIMARY KEY AUTOINCREMENT,
            username      TEXT    NOT NULL UNIQUE,
            password_hash TEXT    NOT NULL,
            created_at    TEXT    NOT NULL DEFAULT (datetime('now'))
        )
    """)

    conn.commit()
    _seed(conn)
    conn.close()


def _seed(conn):
    """Insert demo data only if tables are empty."""
    cur = conn.cursor()

    # Skip if already seeded
    if cur.execute("SELECT COUNT(*) FROM accounts").fetchone()[0] > 0:
        return

    # ── Demo accounts ─────────────────────────────────────────────────────────
    accounts = [
        ("ACC001", "Alice Johnson",   15000.00, "Savings",  "Active"),
        ("ACC002", "Bob Smith",        8500.50, "Checking", "Active"),
        ("ACC003", "Carol Williams",  32000.75, "Savings",  "Active"),
        ("ACC004", "David Brown",      1200.00, "Checking", "Suspended"),
        ("ACC005", "Eva Martinez",    50000.00, "Savings",  "Active"),
    ]
    cur.executemany(
        "INSERT INTO accounts (account_number, holder_name, balance, account_type, status) VALUES (?,?,?,?,?)",
        accounts
    )

    # ── Demo cards ────────────────────────────────────────────────────────────
    cards = [
        ("1234567890123456", 1, hash_pin("1234"), "2028-12-31", 1),
        ("2345678901234567", 2, hash_pin("2345"), "2027-06-30", 1),
        ("3456789012345678", 3, hash_pin("3456"), "2029-03-31", 1),
        ("4567890123456789", 4, hash_pin("4567"), "2026-09-30", 0),
        ("5678901234567890", 5, hash_pin("5678"), "2030-01-31", 1),
    ]
    cur.executemany(
        "INSERT INTO cards (card_number, account_id, pin_hash, expiry_date, is_active) VALUES (?,?,?,?,?)",
        cards
    )

    # ── Demo transactions ─────────────────────────────────────────────────────
    transactions = [
        (1, "Deposit",    5000.00, 15000.00, "Initial deposit",          "2026-01-10 09:00:00"),
        (1, "Withdrawal", 2000.00, 13000.00, "ATM withdrawal",           "2026-01-15 14:30:00"),
        (1, "Deposit",    4000.00, 17000.00, "Salary credit",            "2026-02-01 08:00:00"),
        (1, "Withdrawal", 2000.00, 15000.00, "Bill payment",             "2026-02-10 11:00:00"),
        (2, "Deposit",    8500.50, 8500.50,  "Account opening deposit",  "2026-01-20 10:00:00"),
        (3, "Deposit",   32000.75, 32000.75, "Transfer from savings",    "2026-01-05 09:00:00"),
        (5, "Deposit",   50000.00, 50000.00, "Fixed deposit maturity",   "2026-01-01 00:00:00"),
    ]
    cur.executemany(
        "INSERT INTO transactions (account_id, type, amount, balance_after, description, timestamp) VALUES (?,?,?,?,?,?)",
        transactions
    )

    # ── Demo ATM machines ─────────────────────────────────────────────────────
    atms = [
        ("ATM-001", "Main Branch – Ground Floor",  250000.00, "Online",  "2026-02-01"),
        ("ATM-002", "City Mall – Level 2",          180000.00, "Online",  "2026-01-28"),
        ("ATM-003", "Airport Terminal 1",           320000.00, "Online",  "2026-02-10"),
        ("ATM-004", "University Campus",             50000.00, "Offline", "2026-01-15"),
        ("ATM-005", "Hospital Entrance",            120000.00, "Online",  "2026-02-05"),
    ]
    cur.executemany(
        "INSERT INTO atm_machines (machine_id, location, cash_available, status, last_serviced) VALUES (?,?,?,?,?)",
        atms
    )

    # ── Default admin ─────────────────────────────────────────────────────────
    cur.execute(
        "INSERT INTO admins (username, password_hash) VALUES (?, ?)",
        ("admin", hash_pin("admin123"))
    )

    conn.commit()
    print("[DB] Demo data seeded successfully.")


# ── Query helpers ─────────────────────────────────────────────────────────────

def query_one(sql, params=()):
    conn = get_connection()
    row = conn.execute(sql, params).fetchone()
    conn.close()
    return dict(row) if row else None


def query_all(sql, params=()):
    conn = get_connection()
    rows = conn.execute(sql, params).fetchall()
    conn.close()
    return [dict(r) for r in rows]


def execute(sql, params=()):
    conn = get_connection()
    cur = conn.execute(sql, params)
    conn.commit()
    last_id = cur.lastrowid
    conn.close()
    return last_id
