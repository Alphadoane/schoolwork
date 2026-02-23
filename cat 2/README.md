# KCA Student Result System

A comprehensive Django-based application for managing student academic records, results, and GPA calculations at KCA University.

## 🚀 Features

- **Student Management:** track student details, registration, and admission information.
- **Academic Structure:** Organize data by Faculty, Program, Unit, and Exam Session.
- **Result System:** Record CAT and Exam scores with automatic grade calculation.
- **GPA calculation:** Automated weighted point and GPA tracking.
- **Result Slips:** Generate and view student result slips via a web interface.
- **Dashboard:** At-a-glance overview of students and systems.

## 🛠️ Tech Stack

- **Backend:** Python 3.x, Django 5.x
- **Database:** SQLite (default)
- **Frontend:** HTML, CSS (Django Templates)

## ⚙️ Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd "cat 2"
   ```

2. **Create and activate a virtual environment:**
   ```bash
   python -m venv venv
   .\venv\Scripts\activate  # Windows
   # source venv/bin/activate # Linux/Mac
   ```

3. **Install dependencies:**
   ```bash
   pip install django
   ```

4. **Run Migrations:**
   ```bash
   python manage.py migrate
   ```

5. **Seed the Database (Optional):**
   ```bash
   python seed_data.py
   ```

6. **Start the Development Server:**
   ```bash
   python manage.py runserver
   ```

## 📖 Usage

Access the following routes after starting the server:
- **Dashboard:** [http://127.0.0.1:8000/](http://127.0.0.1:8000/)
- **Student List:** [http://127.0.0.1:8000/students/](http://127.0.0.1:8000/students/)
- **Admin Panel:** [http://127.0.0.1:8000/admin/](http://127.0.0.1:8000/admin/)

## 📂 Project Structure

- `kca_results/`: Main Django project configuration.
- `results_system/`: Core application containing models, views, and templates.
- `media/`: Storage for student photos.
- `static/`: Frontend assets (CSS/JS).
- `seed_data.py`: Script to populate the database with initial data.
