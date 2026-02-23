from django.urls import path
from . import views

app_name = 'results_system'

urlpatterns = [
    path('', views.dashboard, name='dashboard'),
    path('students/', views.all_students, name='all_students'),
    path('results/<path:reg_number>/', views.result_slip, name='result_slip'),
]
