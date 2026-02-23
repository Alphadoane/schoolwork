from django.shortcuts import render, get_object_or_404
from django.db.models import Q
from .models import Student, ExamSession, Result
from .utils import calculate_gpa, get_academic_standing, get_gpa_color


def dashboard(request):
    """Main dashboard - search for students."""
    query = request.GET.get('q', '').strip()
    students = []
    if query:
        students = Student.objects.filter(
            Q(registration_number__icontains=query) |
            Q(first_name__icontains=query) |
            Q(last_name__icontains=query)
        ).select_related('program__faculty')
    sessions = ExamSession.objects.all()
    return render(request, 'results_system/dashboard.html', {
        'students': students,
        'query': query,
        'sessions': sessions,
    })


def result_slip(request, reg_number):
    """Display the result slip for a specific student and session."""
    student = get_object_or_404(Student, registration_number=reg_number)
    sessions = ExamSession.objects.all()

    session_id = request.GET.get('session')
    selected_session = None
    results = []
    gpa = 0.0
    standing_label = ''
    standing_class = ''
    gpa_class = ''
    total_credits = 0
    passed_units = 0
    failed_units = 0

    if session_id:
        selected_session = get_object_or_404(ExamSession, pk=session_id)
        results = Result.objects.filter(
            student=student, exam_session=selected_session
        ).select_related('unit')
        if results.exists():
            gpa = calculate_gpa(results)
            standing_label, standing_class = get_academic_standing(gpa)
            gpa_class = get_gpa_color(gpa)
            total_credits = sum(r.unit.credit_hours for r in results)
            passed_units = sum(1 for r in results if r.status == 'PASS')
            failed_units = sum(1 for r in results if r.status == 'FAIL')
    else:
        # Default to active session
        try:
            selected_session = ExamSession.objects.get(is_active=True)
            results = Result.objects.filter(
                student=student, exam_session=selected_session
            ).select_related('unit')
            if results.exists():
                gpa = calculate_gpa(results)
                standing_label, standing_class = get_academic_standing(gpa)
                gpa_class = get_gpa_color(gpa)
                total_credits = sum(r.unit.credit_hours for r in results)
                passed_units = sum(1 for r in results if r.status == 'PASS')
                failed_units = sum(1 for r in results if r.status == 'FAIL')
        except ExamSession.DoesNotExist:
            pass

    return render(request, 'results_system/result_slip.html', {
        'student': student,
        'sessions': sessions,
        'selected_session': selected_session,
        'results': results,
        'gpa': gpa,
        'standing_label': standing_label,
        'standing_class': standing_class,
        'gpa_class': gpa_class,
        'total_credits': total_credits,
        'passed_units': passed_units,
        'failed_units': failed_units,
    })


def all_students(request):
    """List all students."""
    students = Student.objects.select_related('program__faculty').all()
    return render(request, 'results_system/all_students.html', {
        'students': students,
    })
