"""
routes/admin_routes.py – Admin CRUD endpoints for the ATM Management System.
"""

from flask import Blueprint, request, jsonify, session
from models import Account, Card, Transaction, ATMMachine, Admin

admin_bp = Blueprint("admin", __name__)


def _require_admin():
    return session.get("admin_id") is not None


# ── Admin Auth ────────────────────────────────────────────────────────────────

@admin_bp.route("/api/admin/login", methods=["POST"])
def admin_login():
    data = request.get_json()
    username = (data.get("username") or "").strip()
    password = (data.get("password") or "").strip()

    admin = Admin.verify(username, password)
    if not admin:
        return jsonify({"error": "Invalid credentials."}), 401

    session["admin_id"] = admin["id"]
    session["admin_username"] = admin["username"]
    return jsonify({"message": "Admin login successful.", "username": admin["username"]})


@admin_bp.route("/api/admin/logout", methods=["POST"])
def admin_logout():
    session.pop("admin_id", None)
    session.pop("admin_username", None)
    return jsonify({"message": "Logged out."})


# ── Accounts CRUD ─────────────────────────────────────────────────────────────

@admin_bp.route("/api/admin/accounts", methods=["GET"])
def list_accounts():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401
    return jsonify({"accounts": Account.get_all()})


@admin_bp.route("/api/admin/accounts", methods=["POST"])
def create_account():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    data = request.get_json()
    required = ["account_number", "holder_name", "balance", "account_type"]
    for field in required:
        if not data.get(field) and data.get(field) != 0:
            return jsonify({"error": f"'{field}' is required."}), 400

    try:
        balance = float(data["balance"])
    except (TypeError, ValueError):
        return jsonify({"error": "Invalid balance."}), 400

    # Check unique account number
    existing = Account.get_by_number(data["account_number"])
    if existing:
        return jsonify({"error": "Account number already exists."}), 409

    new_id = Account.create(
        data["account_number"],
        data["holder_name"],
        balance,
        data["account_type"],
        data.get("status", "Active")
    )
    return jsonify({"message": "Account created.", "id": new_id}), 201


@admin_bp.route("/api/admin/accounts/<int:account_id>", methods=["PUT"])
def update_account(account_id):
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    data = request.get_json()
    account = Account.get_by_id(account_id)
    if not account:
        return jsonify({"error": "Account not found."}), 404

    try:
        balance = float(data.get("balance", account["balance"]))
    except (TypeError, ValueError):
        return jsonify({"error": "Invalid balance."}), 400

    Account.update(
        account_id,
        data.get("holder_name", account["holder_name"]),
        balance,
        data.get("account_type", account["account_type"]),
        data.get("status", account["status"])
    )
    return jsonify({"message": "Account updated."})


@admin_bp.route("/api/admin/accounts/<int:account_id>", methods=["DELETE"])
def delete_account(account_id):
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    account = Account.get_by_id(account_id)
    if not account:
        return jsonify({"error": "Account not found."}), 404

    Account.delete(account_id)
    return jsonify({"message": "Account deleted."})


# ── Cards CRUD ────────────────────────────────────────────────────────────────

@admin_bp.route("/api/admin/cards", methods=["GET"])
def list_cards():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401
    return jsonify({"cards": Card.get_all()})


@admin_bp.route("/api/admin/cards", methods=["POST"])
def create_card():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    data = request.get_json()
    required = ["card_number", "account_id", "pin", "expiry_date"]
    for field in required:
        if not data.get(field):
            return jsonify({"error": f"'{field}' is required."}), 400

    account = Account.get_by_id(data["account_id"])
    if not account:
        return jsonify({"error": "Account not found."}), 404

    new_id = Card.create(data["card_number"], data["account_id"], data["pin"], data["expiry_date"])
    return jsonify({"message": "Card created.", "id": new_id}), 201


@admin_bp.route("/api/admin/cards/<int:card_id>", methods=["PUT"])
def update_card(card_id):
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    data = request.get_json()
    Card.update(card_id, data.get("expiry_date"), int(data.get("is_active", 1)))
    return jsonify({"message": "Card updated."})


@admin_bp.route("/api/admin/cards/<int:card_id>", methods=["DELETE"])
def delete_card(card_id):
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401
    Card.delete(card_id)
    return jsonify({"message": "Card deleted."})


# ── ATM Machines CRUD ─────────────────────────────────────────────────────────

@admin_bp.route("/api/admin/atms", methods=["GET"])
def list_atms():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401
    return jsonify({"atms": ATMMachine.get_all()})


@admin_bp.route("/api/admin/atms", methods=["POST"])
def create_atm():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    data = request.get_json()
    required = ["machine_id", "location", "cash_available"]
    for field in required:
        if not data.get(field) and data.get(field) != 0:
            return jsonify({"error": f"'{field}' is required."}), 400

    new_id = ATMMachine.create(
        data["machine_id"],
        data["location"],
        float(data["cash_available"]),
        data.get("status", "Online")
    )
    return jsonify({"message": "ATM machine created.", "id": new_id}), 201


@admin_bp.route("/api/admin/atms/<int:atm_id>", methods=["PUT"])
def update_atm(atm_id):
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    data = request.get_json()
    atm = ATMMachine.get_by_id(atm_id)
    if not atm:
        return jsonify({"error": "ATM not found."}), 404

    ATMMachine.update(
        atm_id,
        data.get("location", atm["location"]),
        float(data.get("cash_available", atm["cash_available"])),
        data.get("status", atm["status"]),
        data.get("last_serviced", atm["last_serviced"])
    )
    return jsonify({"message": "ATM updated."})


@admin_bp.route("/api/admin/atms/<int:atm_id>", methods=["DELETE"])
def delete_atm(atm_id):
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401
    ATMMachine.delete(atm_id)
    return jsonify({"message": "ATM deleted."})


# ── All Transactions ──────────────────────────────────────────────────────────

@admin_bp.route("/api/admin/transactions", methods=["GET"])
def all_transactions():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401
    return jsonify({"transactions": Transaction.get_all()})


# ── Dashboard Stats ───────────────────────────────────────────────────────────

@admin_bp.route("/api/admin/stats", methods=["GET"])
def stats():
    if not _require_admin():
        return jsonify({"error": "Unauthorized."}), 401

    from database import query_one
    total_accounts = query_one("SELECT COUNT(*) as c FROM accounts")["c"]
    total_cards    = query_one("SELECT COUNT(*) as c FROM cards")["c"]
    total_atms     = query_one("SELECT COUNT(*) as c FROM atm_machines")["c"]
    total_tx       = query_one("SELECT COUNT(*) as c FROM transactions")["c"]
    total_balance  = query_one("SELECT COALESCE(SUM(balance),0) as s FROM accounts")["s"]
    online_atms    = query_one("SELECT COUNT(*) as c FROM atm_machines WHERE status='Online'")["c"]

    return jsonify({
        "total_accounts": total_accounts,
        "total_cards": total_cards,
        "total_atms": total_atms,
        "total_transactions": total_tx,
        "total_balance": total_balance,
        "online_atms": online_atms,
    })
