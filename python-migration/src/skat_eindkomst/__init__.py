"""
SKAT eIndkomst Python Client
"""

__version__ = "2.0.1"

from .client import EIndkomst
from .config import ServiceConfig

__all__ = ["EIndkomst", "ServiceConfig"]
