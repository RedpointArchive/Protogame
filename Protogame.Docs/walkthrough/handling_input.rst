.. _walkthrough-handling-input:

Handling input
======================================

.. danger::

  These walkthroughs currently need to be updated to reflect the new
  render pipeline functionality in Protogame.

.. warning::

  For this walkthrough, we are assuming you are following on
  from the :ref:`walkthrough-add-a-player` walkthrough.

In the "add a player" tutorial, we introduced the concepts of defining and
creating entities, as well as performing rendering and updating for them.
However, we are still missing one crucial component left for a game; that is,
the ability to accept input from the player and react to it.

Traditional XNA or MonoGame games directly access the abstracted hardware APIs
such as ``MouseState`` and ``KeyboardState`` to determine what input the player
is provided.  While this works for simple scenarios, it quickly becomes
unfeasible when you have the potential for multiple entities or UI elements
to be accepting input and you only want one entity to react to the player's
actions.

To this end, Protogame provides an event system which uses bindings to
map player inputs to actions that the game should take.  The event system
is the recommended way of accepting input in a Protogame-based game (but
you do not have to use it).

Adding the module
--------------------

In order to use the events system, we have to load it's module into the
dependency injection kernel.  Modify the contents of your `Program.cs` file
so that the initialization of the kernel looks like the following:

.. literalinclude:: snippet/handling_input_load_module.cs
    :language: csharp

We can now use the events system.

Creating an event binder
------------------------------

The events system allows multiple event binders to be registered (through
dependency injection).  The most commonly used binder is the ``StaticEventBinder``,
which uses a fluent syntax for allowing the developer to map input to actions.

That said, you can derive from the ``IEventBinder`` interface and implement
another mechanism for mapping inputs to actions.  You will most likely want
to do this when you want to allow the player to remap inputs.

However, implementation of a custom ``IEventBinder`` is outside the scope of
this walkthrough, so we will be deriving from ``StaticEventBinder`` and registering 
it.

First, we need to create a new class called ``MyProjectEventBinder`` (adjusting
the project name appropriately), with the following content:

.. literalinclude:: snippet/handling_input_event_binder_base.cs
    :language: csharp

Now let's register it in our dependency injection module.  Modify the ``Load``
method of ``MyProjectModule`` such that it looks like:

.. literalinclude:: snippet/handling_input_module.cs
    :language: csharp

Creating actions
---------------------

We will now define an action that the player can take.  This is seperate
from the actual input the player will provide, but indicates what should occur.

Event bindings can target one of multiple things; you can have bindings map
inputs to **actions**, **entity actions**, **commands** or **listeners**.

In this case we want to perform an action against a type of entity, where
we are expecting only one entity of this type to be in the world.  Thus we
can use an **entity action** and simplify our logic drastically, since the
event binder will take care of locating the entity and passing it in as a
parameter.

To start, we are going to define four actions for moving our entity.  We have
placed these four actions in one file for the walkthrough's simplicity, although
the normal convention is to have one class per file.  Add the following
content to a file called ``MoveActions.cs``:

.. literalinclude:: snippet/handling_input_move_actions.cs
    :language: csharp

Binding input to actions
---------------------------

Now that we have defined both our event binder and our actions, we can create
our input bindings.

Modify the event binder so that the ``Configure`` method looks like the
following:

.. literalinclude:: snippet/handling_input_event_binder_bound.cs
    :language: csharp

In the code above, we bind various "key held events", (that is, they fire while the player holds a key down)
onto the ``PlayerEntity`` instance in the world, with the specified action being invoked against
the instance of the ``PlayerEntity``.

Modify the player entity
-----------------------------

Before we go any further, we need to remove the modifications to the player
entity that cause it's position to be updated automatically every step.

.. danger::

  If you leave this code in, you won't see the
  effects of your keyboard presses, as the result will be immediately reset when
  the entity updates.

If you followed the previous tutorial, your player entity might have it's
Update method defined as:

.. literalinclude:: snippet/add_a_player_update_entity.cs
    :language: csharp

Remove the above code so that it becomes it's original form:

.. literalinclude:: snippet/handling_input_update_entity.cs
    :language: csharp
    
Testing the result
----------------------

With the event bindings now set up, you should be able to run the game and
see something similar to the following video:

.. raw:: html

    <iframe width="600" height="360" src="//www.youtube.com/embed/yAj7-TCuN7Q" frameborder="0" allowfullscreen></iframe>
    <br/><br/>
    
Conclusion
-------------

For now, this ends the series of basic walkthroughs on getting started with Protogame.  We've
covered the basics, such as rendering graphics, entity management and handling input.

.. note::
  Is there something we've missed or didn't cover? `Please let me know!`_

.. _Please let me know!: http://twitter.com/hachque