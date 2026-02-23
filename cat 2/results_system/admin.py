from django.contrib import admin
from .models import Faculty, Program, Student, Unit, ExamSession, Result


@admin.register(Faculty)
class FacultyAdmin(admin.ModelAdmin):
    list_display = ('code', 'name')
    search_fields = ('code', 'name')


@admin.register(Program)
class ProgramAdmin(admin.ModelAdmin):
    list_display = ('code', 'name', 'faculty')
    list_filter = ('faculty',)
    search_fields = ('code', 'name')


class ResultInline(admin.TabularInline):
    model = Result
    extra = 0
    readonly_fields = ('grade', 'grade_points', 'total_score', 'status')
    fields = ('unit', 'exam_session', 'cat_score', 'exam_score', 'total_score', 'grade', 'grade_points', 'status')


@admin.register(Student)
class StudentAdmin(admin.ModelAdmin):
    list_display = ('registration_number', 'full_name', 'program', 'year_of_study', 'email')
    list_filter = ('program__faculty', 'year_of_study')
    search_fields = ('registration_number', 'first_name', 'last_name', 'email')
    inlines = [ResultInline]
    fieldsets = (
        ('Personal Information', {
            'fields': ('first_name', 'last_name', 'email', 'photo')
        }),
        ('Academic Information', {
            'fields': ('registration_number', 'program', 'year_of_study', 'date_of_admission')
        }),
    )


@admin.register(Unit)
class UnitAdmin(admin.ModelAdmin):
    list_display = ('code', 'name', 'program', 'year', 'semester', 'credit_hours')
    list_filter = ('program', 'year', 'semester')
    search_fields = ('code', 'name')


@admin.register(ExamSession)
class ExamSessionAdmin(admin.ModelAdmin):
    list_display = ('name', 'year', 'semester', 'is_active')
    list_filter = ('year', 'semester', 'is_active')


@admin.register(Result)
class ResultAdmin(admin.ModelAdmin):
    list_display = ('student', 'unit', 'exam_session', 'cat_score', 'exam_score', 'total_score', 'grade', 'status')
    list_filter = ('exam_session', 'unit__program')
    search_fields = ('student__registration_number', 'student__first_name', 'student__last_name', 'unit__code')
    readonly_fields = ('grade', 'grade_points', 'total_score', 'status', 'weighted_points')


# Customize admin site branding
admin.site.site_header = "KCA University Results Administration"
admin.site.site_title = "KCA Results Admin"
admin.site.index_title = "Results Management Portal"
