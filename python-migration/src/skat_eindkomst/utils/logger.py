"""
Logging utilities

Provides a simple logger interface compatible with both Python logging
and the C# ILog interface pattern.
"""
import logging
import sys


def get_logger(name: str, level=logging.INFO):
    """
    Create and configure logger

    Args:
        name: Logger name (typically __name__)
        level: Logging level (default: INFO)

    Returns:
        Configured logger instance
    """
    logger = logging.getLogger(name)
    logger.setLevel(level)

    # Only add handler if none exists (avoid duplicates)
    if not logger.handlers:
        handler = logging.StreamHandler(sys.stdout)
        handler.setLevel(level)

        formatter = logging.Formatter(
            '%(asctime)s - %(name)s - %(levelname)s - %(message)s',
            datefmt='%Y-%m-%d %H:%M:%S'
        )
        handler.setFormatter(formatter)

        logger.addHandler(handler)

    return logger


class ILog:
    """
    Interface compatible with C# ILog pattern

    This class provides Info() and Error() methods (capital I and E)
    to match the C# interface, while internally using Python's logging.
    """

    def __init__(self, name: str = "skat_eindkomst", level=logging.INFO):
        """Initialize ILog-compatible logger"""
        self.logger = get_logger(name, level)

    def Info(self, message: str):
        """Log info message (C# style)"""
        self.logger.info(message)

    def Error(self, message: str):
        """Log error message (C# style)"""
        self.logger.error(message)

    # Python-style aliases
    def info(self, message: str):
        """Log info message (Python style)"""
        self.logger.info(message)

    def error(self, message: str):
        """Log error message (Python style)"""
        self.logger.error(message)
