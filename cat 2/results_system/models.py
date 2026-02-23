from django.db import models
from django.core.validators import MinValueValidator, MaxValueValidator


class Faculty(models.Model):
    name = models.CharField(max_length=200, unique=True)
    code = models.CharField(max_length=10, unique=True)

    class Meta:
        verbose_name_plural = "Faculties"
        ordering = ['name']

    def __str__(self):
        return f"{self.code} - {self.name}"


class Program(models.Model):
    faculty = models.ForeignKey(Faculty, on_delete=models.CASCADE, related_name='programs')
    name = models.CharField(max_length=200)
    code = models.CharField(max_length=20, unique=True)

    class Meta:
        ordering = ['name']

    def __str__(self):
        return f"{self.code} - {self.name}"


class Student(models.Model):
    YEAR_CHOICES = [(i, f"Year {i}") for i in range(1, 5)]
    SEMESTER_CHOICES = [(1, "Semester 1"), (2, "Semester 2"), (3, "Semester 3")]

    registration_number = models.CharField(max_length=30, unique=True)
    first_name = models.CharField(max_length=100)
    last_name = models.CharField(max_length=100)
    email = models.EmailField(unique=True)
    program = models.ForeignKey(Program, on_delete=models.SET_NULL, null=True, related_name='students')
    year_of_study = models.IntegerField(choices=YEAR_CHOICES, default=1)
    date_of_admission = models.DateField()
    photo = models.ImageField(upload_to='student_photos/', null=True, blank=True)

    class Meta:
        ordering = ['last_name', 'first_name']

    def __str__(self):
        return f"{self.registration_number} - {self.full_name}"

    @property
    def full_name(self):
        return f"{self.first_name} {self.last_name}"


class Unit(models.Model):
    code = models.CharField(max_length=20, unique=True)
    name = models.CharField(max_length=200)
    credit_hours = models.IntegerField(default=3, validators=[MinValueValidator(1), MaxValueValidator(6)])
    program = models.ForeignKey(Program, on_delete=models.CASCADE, related_name='units')
    year = models.IntegerField(choices=Student.YEAR_CHOICES, default=1)
    semester = models.IntegerField(choices=Student.SEMESTER_CHOICES, default=1)

    class Meta:
        ordering = ['code']

    def __str__(self):
        return f"{self.code} - {self.name}"


class ExamSession(models.Model):
    SEMESTER_CHOICES = Student.SEMESTER_CHOICES
    name = models.CharField(max_length=100)  # e.g. "January - April 2025"
    year = models.IntegerField()
    semester = models.IntegerField(choices=SEMESTER_CHOICES)
    is_active = models.BooleanField(default=False)

    class Meta:
        ordering = ['-year', '-semester']
        unique_together = ('year', 'semester')

    def __str__(self):
        return f"{self.name} (Sem {self.semester}, {self.year})"


class Result(models.Model):
    student = models.ForeignKey(Student, on_delete=models.CASCADE, related_name='results')
    unit = models.ForeignKey(Unit, on_delete=models.CASCADE, related_name='results')
    exam_session = models.ForeignKey(ExamSession, on_delete=models.CASCADE, related_name='results')
    cat_score = models.FloatField(
        verbose_name="CAT Score (30%)",
        validators=[MinValueValidator(0), MaxValueValidator(30)],
        null=True, blank=True
    )
    exam_score = models.FloatField(
        verbose_name="Exam Score (70%)",
        validators=[MinValueValidator(0), MaxValueValidator(70)],
        null=True, blank=True
    )

    class Meta:
        unique_together = ('student', 'unit', 'exam_session')
        ordering = ['unit__code']

    def __str__(self):
        return f"{self.student.registration_number} - {self.unit.code} ({self.exam_session})"

    @property
    def total_score(self):
        cat = self.cat_score or 0
        exam = self.exam_score or 0
        return round(cat + exam, 2)

    @property
    def grade(self):
        score = self.total_score
        if score >= 70:
            return 'A'
        elif score >= 60:
            return 'B+'
        elif score >= 55:
            return 'B'
        elif score >= 50:
            return 'C+'
        elif score >= 45:
            return 'C'
        elif score >= 40:
            return 'D+'
        elif score >= 35:
            return 'D'
        else:
            return 'E'

    @property
    def grade_points(self):
        grade_map = {
            'A': 4.0, 'B+': 3.5, 'B': 3.0,
            'C+': 2.5, 'C': 2.0, 'D+': 1.5,
            'D': 1.0, 'E': 0.0
        }
        return grade_map.get(self.grade, 0.0)

    @property
    def status(self):
        return 'PASS' if self.total_score >= 40 else 'FAIL'

    @property
    def weighted_points(self):
        return self.grade_points * self.unit.credit_hours
