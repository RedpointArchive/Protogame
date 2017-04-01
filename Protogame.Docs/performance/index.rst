Performance Profiling
================================

.. note::

    This documentation is a work-in-progress.
    
The performance profiling module allows you to profile events and method
calls in Protogame-based games.

This API provides both a general API for measuring the length of events
explicitly, and an automatic profiling system for desktop platforms.

.. warning::

    The automatic profiling mechanism is only available on desktop
    platforms, but works without requiring developers to explicitly
    scope measured events.