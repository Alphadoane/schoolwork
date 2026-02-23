"""
app.py – Flask entry point for the ATM Management System.
"""

import os
from flask import Flask, render_template, session, redirect, url_for
from database import init_db
from routes.account_routes import account_bp
from routes.admin_routes import admin_bp

app = Flask(__name__)
app.secret_key = "atm-secret-key-2026-change-in-production"
app.config["SESSION_COOKIE_HTTPONLY"] = True


# ── Register blueprints ───────────────────────────────────────────────────────
app.register_blueprint(account_bp)
app.register_blueprint(admin_bp)


# ── Page routes ───────────────────────────────────────────────────────────────

@app.route("/")
def index():
    return render_template("index.html")


@app.route("/dashboard")
def dashboard():
    if not session.get("account_id"):
        return redirect(url_for("index"))
    return render_template("dashboard.html")


@app.route("/admin")
def admin_panel():
    if not session.get("admin_id"):
        return redirect(url_for("index"))
    return render_template("admin.html")


# ── Init & run ────────────────────────────────────────────────────────────────

if __name__ == "__main__":
    init_db()
    print("\n" + "=" * 60)
    print("  ATM Management System")
    print("  Running at: http://127.0.0.1:5000")
    print("=" * 60)
    print("\n  Demo Customer Credentials:")
    print("    Card: 1234567890123456  PIN: 1234  (Alice Johnson)")
    print("    Card: 2345678901234567  PIN: 2345  (Bob Smith)")
    print("    Card: 3456789012345678  PIN: 3456  (Carol Williams)")
    print("\n  Demo Admin Credentials:")
    print("    Username: admin  Password: admin123")
    print("=" * 60 + "\n")
    app.run(debug=True, port=5000)
