"""
Seed script for KCA University Results System.
Run with: py manage.py shell < seed_data.py
"""
import os
import django
os.environ.setdefault('DJANGO_SETTINGS_MODULE', 'kca_results.settings')
django.setup()

from datetime import date
from results_system.models import Faculty, Program, Student, Unit, ExamSession, Result

print("Seeding database...")

# --- Faculties ---
fict, _ = Faculty.objects.get_or_create(code='FICT', defaults={'name': 'Faculty of Information & Communication Technology'})
fob, _  = Faculty.objects.get_or_create(code='FOB',  defaults={'name': 'Faculty of Business'})
fos, _  = Faculty.objects.get_or_create(code='FOS',  defaults={'name': 'Faculty of Science'})

# --- Programs ---
bscs, _ = Program.objects.get_or_create(code='BSCS', defaults={'name': 'Bachelor of Science in Computer Science', 'faculty': fict})
bbit, _ = Program.objects.get_or_create(code='BBIT', defaults={'name': 'Bachelor of Business Information Technology', 'faculty': fict})
bba,  _ = Program.objects.get_or_create(code='BBA',  defaults={'name': 'Bachelor of Business Administration', 'faculty': fob})

# --- Exam Session ---
session, _ = ExamSession.objects.get_or_create(
    year=2024, semester=2,
    defaults={'name': 'September – December 2024', 'is_active': True}
)

# --- Units for BSCS Year 2, Sem 2 ---
units_bscs = [
    ('CSC201', 'Data Structures & Algorithms', 3),
    ('CSC202', 'Object Oriented Programming', 3),
    ('CSC203', 'Database Systems', 3),
    ('CSC204', 'Computer Networks', 3),
    ('CSC205', 'Operating Systems', 3),
]
unit_objs = []
for code, name, credits in units_bscs:
    u, _ = Unit.objects.get_or_create(code=code, defaults={
        'name': name, 'credit_hours': credits,
        'program': bscs, 'year': 2, 'semester': 2
    })
    unit_objs.append(u)

# --- Units for BBIT Year 1, Sem 2 ---
units_bbit = [
    ('BIT101', 'Introduction to Programming', 3),
    ('BIT102', 'Business Communication', 2),
    ('BIT103', 'Mathematics for IT', 3),
    ('BIT104', 'Introduction to Databases', 3),
]
unit_objs_bbit = []
for code, name, credits in units_bbit:
    u, _ = Unit.objects.get_or_create(code=code, defaults={
        'name': name, 'credit_hours': credits,
        'program': bbit, 'year': 1, 'semester': 2
    })
    unit_objs_bbit.append(u)

# --- Students ---
students_data = [
    ('KCA/2022/001', 'Alice', 'Wanjiku',  'alice.wanjiku@students.kca.ac.ke',  bscs, 2),
    ('KCA/2022/002', 'Brian', 'Otieno',   'brian.otieno@students.kca.ac.ke',   bscs, 2),
    ('KCA/2022/003', 'Carol', 'Muthoni',  'carol.muthoni@students.kca.ac.ke',  bscs, 2),
    ('KCA/2023/001', 'David', 'Kamau',    'david.kamau@students.kca.ac.ke',    bbit, 1),
    ('KCA/2023/002', 'Eva',   'Njeri',    'eva.njeri@students.kca.ac.ke',      bbit, 1),
    ('KCA/2022/004', 'Frank', 'Ochieng',  'frank.ochieng@students.kca.ac.ke',  bscs, 2),
]

student_objs = []
for reg, first, last, email, prog, yr in students_data:
    s, _ = Student.objects.get_or_create(registration_number=reg, defaults={
        'first_name': first, 'last_name': last, 'email': email,
        'program': prog, 'year_of_study': yr, 'date_of_admission': date(2022 if yr == 2 else 2023, 9, 1)
    })
    student_objs.append(s)

# --- Results for BSCS students ---
results_data = {
    'KCA/2022/001': [(28, 65), (25, 58), (27, 62), (29, 70), (24, 55)],  # Alice
    'KCA/2022/002': [(20, 45), (18, 40), (22, 50), (15, 35), (19, 42)],  # Brian
    'KCA/2022/003': [(30, 68), (28, 65), (29, 70), (27, 63), (30, 69)],  # Carol
    'KCA/2022/004': [(10, 25), (12, 28), (15, 30), (8,  20), (11, 26)],  # Frank
}

for reg, scores in results_data.items():
    student = Student.objects.get(registration_number=reg)
    for unit, (cat, exam) in zip(unit_objs, scores):
        Result.objects.get_or_create(
            student=student, unit=unit, exam_session=session,
            defaults={'cat_score': cat, 'exam_score': exam}
        )

# --- Results for BBIT students ---
bbit_results = {
    'KCA/2023/001': [(25, 60), (20, 55), (22, 58), (24, 62)],  # David
    'KCA/2023/002': [(28, 65), (22, 60), (26, 63), (27, 68)],  # Eva
}

for reg, scores in bbit_results.items():
    student = Student.objects.get(registration_number=reg)
    for unit, (cat, exam) in zip(unit_objs_bbit, scores):
        Result.objects.get_or_create(
            student=student, unit=unit, exam_session=session,
            defaults={'cat_score': cat, 'exam_score': exam}
        )

print("✅ Seed complete!")
print(f"   Faculties:  {Faculty.objects.count()}")
print(f"   Programs:   {Program.objects.count()}")
print(f"   Students:   {Student.objects.count()}")
print(f"   Units:      {Unit.objects.count()}")
print(f"   Sessions:   {ExamSession.objects.count()}")
print(f"   Results:    {Result.objects.count()}")
