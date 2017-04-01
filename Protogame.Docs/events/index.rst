.. _events:

Events (Input)
================================

.. note::

    This documentation is a work-in-progress.
    
The event system provides a general mechanism for propagating events from
one part of your game to another.  As opposed to the .NET events framework,
the Protogame events system allows for events to be consumed by handlers,
filtered at runtime, and bound with a fluent syntax.

The event system is primarily used to handle input in Protogame; this allows
entities in a game to handle input events, without multiple entities receiving
the same event.