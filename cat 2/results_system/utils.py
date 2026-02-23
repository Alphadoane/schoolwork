"""
Utility functions for GPA calculation and result slip generation.
"""


def calculate_gpa(results):
    """
    Calculate the GPA for a list of Result objects.
    GPA = Sum(Grade Points * Credit Hours) / Sum(Credit Hours)
    """
    total_weighted = sum(r.weighted_points for r in results)
    total_credits = sum(r.unit.credit_hours for r in results)
    if total_credits == 0:
        return 0.0
    return round(total_weighted / total_credits, 2)


def get_academic_standing(gpa):
    """Return academic standing label based on GPA."""
    if gpa >= 3.6:
        return ('First Class Honours', 'standing-first')
    elif gpa >= 3.0:
        return ('Second Class Honours (Upper)', 'standing-upper')
    elif gpa >= 2.0:
        return ('Second Class Honours (Lower)', 'standing-lower')
    elif gpa >= 1.0:
        return ('Pass', 'standing-pass')
    else:
        return ('Fail', 'standing-fail')


def get_gpa_color(gpa):
    """Return a CSS class for GPA display."""
    if gpa >= 3.6:
        return 'gpa-excellent'
    elif gpa >= 3.0:
        return 'gpa-good'
    elif gpa >= 2.0:
        return 'gpa-average'
    elif gpa >= 1.0:
        return 'gpa-pass'
    else:
        return 'gpa-fail'
