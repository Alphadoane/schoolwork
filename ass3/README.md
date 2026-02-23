# ATM Management System

A comprehensive ATM Management System built with Python (Flask) and SQLite. This system provides both customer and administrator functionalities, allowing for secure transactions and efficient management of accounts, cards, and ATM machines.

## 🚀 Features

### Customer Functionalities
- **Secure Login**: Access account using card number and PIN.
- **Balance Inquiry**: Check current account balance.
- **Deposits & Withdrawals**: Perform cash transactions with real-time balance updates.
- **Funds Transfer**: Transfer money between accounts securely.
- **Transaction History**: View a detailed log of all account activities.

### Administrator Functionalities
- **Admin Login**: Secure access for authorized personnel.
- **Account Management**: Create, Read, Update, and Delete (CRUD) customer accounts.
- **Card Management**: Manage ATM cards associated with accounts.
- **ATM Machine Management**: CRUD operations for physical ATM machine records.
- **Full Transaction Audit**: View all transactions across the entire system.

## 🛠 Tech Stack
- **Frontend**: HTML5, Vanilla CSS, JavaScript
- **Backend**: Python, Flask
- **Database**: SQLite
- **Architecture**: Modular Blueprint-based Flask application

## 📋 Project Structure
```text
ass3/
├── app.py              # Flask entry point & main application configuration
├── database.py         # Database initialization and connection logic
├── models.py           # Database schema definitions
├── atm.db              # SQLite database file
├── routes/             # Modular route handlers
│   ├── account_routes.py # Customer-facing endpoints
│   └── admin_routes.py   # Administrator-facing endpoints
├── static/             # Static assets (CSS, JS, Images)
└── templates/          # HTML templates
    ├── index.html      # Landing & Login page
    ├── dashboard.html  # Customer dashboard
    └── admin.html      # Administrator panel
```

## ⚙️ Setup and Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd ass3
   ```

2. **Install dependencies**:
   Ensure you have Python installed. You can install the required packages using pip:
   ```bash
   pip install flask
   ```

3. **Run the application**:
   ```bash
   python app.py
   ```
   The application will be available at `http://127.0.0.1:5000`.

## 🔐 Demo Credentials

### Customer Login
- **Card Number**: `1234567890123456` | **PIN**: `1234` (Alice Johnson)
- **Card Number**: `2345678901234567` | **PIN**: `2345` (Bob Smith)
- **Card Number**: `3456789012345678` | **PIN**: `3456` (Carol Williams)

### Administrator Login
- **Username**: `admin`
- **Password**: `admin123`

---
*Developed as part of the Advance App Course Work (Assignment 3).*
