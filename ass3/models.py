"""
models.py – Model classes wrapping SQLite rows for the ATM Management System.
"""

from database import query_one, query_all, execute, hash_pin
from datetime import datetime


class Account:
    @staticmethod
    def get_all():
        return query_all("SELECT * FROM accounts ORDER BY id")

    @staticmethod
    def get_by_id(account_id):
        return query_one("SELECT * FROM accounts WHERE id = ?", (account_id,))

    @staticmethod
    def get_by_number(account_number):
        return query_one("SELECT * FROM accounts WHERE account_number = ?", (account_number,))

    @staticmethod
    def create(account_number, holder_name, balance, account_type, status="Active"):
        return execute(
            "INSERT INTO accounts (account_number, holder_name, balance, account_type, status) VALUES (?,?,?,?,?)",
            (account_number, holder_name, balance, account_type, status)
        )

    @staticmethod
    def update(account_id, holder_name, balance, account_type, status):
        execute(
            "UPDATE accounts SET holder_name=?, balance=?, account_type=?, status=? WHERE id=?",
            (holder_name, balance, account_type, status, account_id)
        )

    @staticmethod
    def delete(account_id):
        execute("DELETE FROM accounts WHERE id = ?", (account_id,))

    @staticmethod
    def update_balance(account_id, new_balance):
        execute("UPDATE accounts SET balance=? WHERE id=?", (new_balance, account_id))


class Card:
    @staticmethod
    def get_all():
        return query_all("""
            SELECT c.*, a.account_number, a.holder_name
            FROM cards c JOIN accounts a ON c.account_id = a.id
            ORDER BY c.id
        """)

    @staticmethod
    def get_by_number(card_number):
        return query_one("""
            SELECT c.*, a.account_number, a.holder_name, a.balance, a.status as account_status
            FROM cards c JOIN accounts a ON c.account_id = a.id
            WHERE c.card_number = ?
        """, (card_number,))

    @staticmethod
    def verify_pin(card_number, pin):
        card = query_one(
            "SELECT * FROM cards WHERE card_number = ? AND pin_hash = ? AND is_active = 1",
            (card_number, hash_pin(pin))
        )
        return card

    @staticmethod
    def create(card_number, account_id, pin, expiry_date):
        return execute(
            "INSERT INTO cards (card_number, account_id, pin_hash, expiry_date, is_active) VALUES (?,?,?,?,1)",
            (card_number, account_id, hash_pin(pin), expiry_date)
        )

    @staticmethod
    def update(card_id, expiry_date, is_active):
        execute(
            "UPDATE cards SET expiry_date=?, is_active=? WHERE id=?",
            (expiry_date, is_active, card_id)
        )

    @staticmethod
    def delete(card_id):
        execute("DELETE FROM cards WHERE id = ?", (card_id,))


class Transaction:
    @staticmethod
    def get_by_account(account_id, limit=20):
        return query_all(
            "SELECT * FROM transactions WHERE account_id=? ORDER BY timestamp DESC LIMIT ?",
            (account_id, limit)
        )

    @staticmethod
    def get_all(limit=100):
        return query_all("""
            SELECT t.*, a.account_number, a.holder_name
            FROM transactions t JOIN accounts a ON t.account_id = a.id
            ORDER BY t.timestamp DESC LIMIT ?
        """, (limit,))

    @staticmethod
    def record(account_id, tx_type, amount, balance_after, description=""):
        return execute(
            "INSERT INTO transactions (account_id, type, amount, balance_after, description) VALUES (?,?,?,?,?)",
            (account_id, tx_type, amount, balance_after, description)
        )


class ATMMachine:
    @staticmethod
    def get_all():
        return query_all("SELECT * FROM atm_machines ORDER BY id")

    @staticmethod
    def get_by_id(atm_id):
        return query_one("SELECT * FROM atm_machines WHERE id = ?", (atm_id,))

    @staticmethod
    def get_online():
        return query_one("SELECT * FROM atm_machines WHERE status='Online' AND cash_available > 0 LIMIT 1")

    @staticmethod
    def create(machine_id, location, cash_available, status):
        return execute(
            "INSERT INTO atm_machines (machine_id, location, cash_available, status) VALUES (?,?,?,?)",
            (machine_id, location, cash_available, status)
        )

    @staticmethod
    def update(atm_id, location, cash_available, status, last_serviced):
        execute(
            "UPDATE atm_machines SET location=?, cash_available=?, status=?, last_serviced=? WHERE id=?",
            (location, cash_available, status, last_serviced, atm_id)
        )

    @staticmethod
    def delete(atm_id):
        execute("DELETE FROM atm_machines WHERE id = ?", (atm_id,))

    @staticmethod
    def deduct_cash(atm_id, amount):
        execute(
            "UPDATE atm_machines SET cash_available = cash_available - ? WHERE id=?",
            (amount, atm_id)
        )


class Admin:
    @staticmethod
    def verify(username, password):
        return query_one(
            "SELECT * FROM admins WHERE username=? AND password_hash=?",
            (username, hash_pin(password))
        )
