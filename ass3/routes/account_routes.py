"""
routes/account_routes.py – Customer-facing ATM operations.
"""

from flask import Blueprint, request, jsonify, session
from models import Account, Card, Transaction, ATMMachine

account_bp = Blueprint("account", __name__)


def _require_session():
    """Return account dict if logged in, else None."""
    account_id = session.get("account_id")
    if not account_id:
        return None
    return Account.get_by_id(account_id)


# ── Login ─────────────────────────────────────────────────────────────────────

@account_bp.route("/api/login", methods=["POST"])
def login():
    data = request.get_json()
    card_number = (data.get("card_number") or "").strip()
    pin = (data.get("pin") or "").strip()

    if not card_number or not pin:
        return jsonify({"error": "Card number and PIN are required."}), 400

    card = Card.verify_pin(card_number, pin)
    if not card:
        return jsonify({"error": "Invalid card number or PIN."}), 401

    account = Account.get_by_id(card["account_id"])
    if not account:
        return jsonify({"error": "Account not found."}), 404

    if account["status"] != "Active":
        return jsonify({"error": f"Account is {account['status']}. Please contact support."}), 403

    session["account_id"] = account["id"]
    session["card_number"] = card_number
    return jsonify({
        "message": "Login successful.",
        "account": {
            "id": account["id"],
            "account_number": account["account_number"],
            "holder_name": account["holder_name"],
            "balance": account["balance"],
            "account_type": account["account_type"],
        }
    })


@account_bp.route("/api/logout", methods=["POST"])
def logout():
    session.clear()
    return jsonify({"message": "Logged out."})


# ── Balance ───────────────────────────────────────────────────────────────────

@account_bp.route("/api/balance", methods=["GET"])
def balance():
    account = _require_session()
    if not account:
        return jsonify({"error": "Not authenticated."}), 401
    return jsonify({"balance": account["balance"], "account_number": account["account_number"]})


# ── Deposit ───────────────────────────────────────────────────────────────────

@account_bp.route("/api/deposit", methods=["POST"])
def deposit():
    account = _require_session()
    if not account:
        return jsonify({"error": "Not authenticated."}), 401

    data = request.get_json()
    try:
        amount = float(data.get("amount", 0))
    except (TypeError, ValueError):
        return jsonify({"error": "Invalid amount."}), 400

    if amount <= 0:
        return jsonify({"error": "Deposit amount must be positive."}), 400
    if amount > 1_000_000:
        return jsonify({"error": "Maximum single deposit is 1,000,000."}), 400

    new_balance = account["balance"] + amount
    Account.update_balance(account["id"], new_balance)
    Transaction.record(account["id"], "Deposit", amount, new_balance, data.get("description", "ATM Deposit"))

    return jsonify({"message": f"Deposited {amount:,.2f} successfully.", "balance": new_balance})


# ── Withdrawal ────────────────────────────────────────────────────────────────

@account_bp.route("/api/withdraw", methods=["POST"])
def withdraw():
    account = _require_session()
    if not account:
        return jsonify({"error": "Not authenticated."}), 401

    data = request.get_json()
    try:
        amount = float(data.get("amount", 0))
    except (TypeError, ValueError):
        return jsonify({"error": "Invalid amount."}), 400

    if amount <= 0:
        return jsonify({"error": "Withdrawal amount must be positive."}), 400
    if amount > account["balance"]:
        return jsonify({"error": "Insufficient funds."}), 400

    # Check ATM has enough cash
    atm = ATMMachine.get_online()
    if not atm:
        return jsonify({"error": "No ATM machines are currently available."}), 503
    if atm["cash_available"] < amount:
        return jsonify({"error": "ATM does not have sufficient cash. Please try another machine."}), 400

    new_balance = account["balance"] - amount
    Account.update_balance(account["id"], new_balance)
    ATMMachine.deduct_cash(atm["id"], amount)
    Transaction.record(account["id"], "Withdrawal", amount, new_balance, data.get("description", "ATM Withdrawal"))

    return jsonify({"message": f"Withdrew {amount:,.2f} successfully.", "balance": new_balance})


# ── Transfer ──────────────────────────────────────────────────────────────────

@account_bp.route("/api/transfer", methods=["POST"])
def transfer():
    account = _require_session()
    if not account:
        return jsonify({"error": "Not authenticated."}), 401

    data = request.get_json()
    target_number = (data.get("target_account") or "").strip()
    try:
        amount = float(data.get("amount", 0))
    except (TypeError, ValueError):
        return jsonify({"error": "Invalid amount."}), 400

    if not target_number:
        return jsonify({"error": "Target account number is required."}), 400
    if amount <= 0:
        return jsonify({"error": "Transfer amount must be positive."}), 400
    if target_number == account["account_number"]:
        return jsonify({"error": "Cannot transfer to the same account."}), 400
    if amount > account["balance"]:
        return jsonify({"error": "Insufficient funds."}), 400

    target = Account.get_by_number(target_number)
    if not target:
        return jsonify({"error": "Target account not found."}), 404
    if target["status"] != "Active":
        return jsonify({"error": "Target account is not active."}), 400

    sender_balance = account["balance"] - amount
    receiver_balance = target["balance"] + amount

    Account.update_balance(account["id"], sender_balance)
    Account.update_balance(target["id"], receiver_balance)

    desc_out = f"Transfer to {target_number}"
    desc_in = f"Transfer from {account['account_number']}"
    Transaction.record(account["id"], "Transfer Out", amount, sender_balance, desc_out)
    Transaction.record(target["id"], "Transfer In", amount, receiver_balance, desc_in)

    return jsonify({
        "message": f"Transferred {amount:,.2f} to {target_number} successfully.",
        "balance": sender_balance
    })


# ── Transaction History ───────────────────────────────────────────────────────

@account_bp.route("/api/transactions", methods=["GET"])
def transactions():
    account = _require_session()
    if not account:
        return jsonify({"error": "Not authenticated."}), 401

    txs = Transaction.get_by_account(account["id"])
    return jsonify({"transactions": txs})
