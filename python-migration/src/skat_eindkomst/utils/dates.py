"""
Date utilities

Helper functions for date formatting and parsing
"""
from datetime import datetime
from typing import Optional


def format_date(date: datetime, format_string: str = "%Y-%m-%d") -> str:
    """
    Format datetime to string

    Args:
        date: datetime object
        format_string: strftime format string (default: ISO format YYYY-MM-DD)

    Returns:
        Formatted date string
    """
    if date is None:
        return ""
    return date.strftime(format_string)


def parse_date(date_string: str, format_string: str = "%Y-%m-%d") -> Optional[datetime]:
    """
    Parse date string to datetime

    Args:
        date_string: Date string to parse
        format_string: strptime format string (default: ISO format YYYY-MM-DD)

    Returns:
        datetime object or None if parsing fails
    """
    if not date_string:
        return None

    try:
        return datetime.strptime(date_string, format_string)
    except ValueError:
        # Try alternative formats
        for fmt in ["%Y.%m.%d", "%d-%m-%Y", "%d/%m/%Y"]:
            try:
                return datetime.strptime(date_string, fmt)
            except ValueError:
                continue
        return None


def to_short_date_string(date: datetime) -> str:
    """
    Convert datetime to short date string (mirrors C# ToShortDateString())

    Args:
        date: datetime object

    Returns:
        Date string in format DD-MM-YYYY
    """
    return date.strftime("%d-%m-%Y")
