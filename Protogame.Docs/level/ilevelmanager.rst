ILevelManager
===================

The level manager service provides a simple mechanism for loading a 
level into a world.  It has one method, ``Load``, which accepts a reference
to the current world and the name of the level asset.  In the context
of a world, it can be used like so:

.. literalinclude:: snippet/loading_level.cs
    :language: csharp
    
The default level manager implementation uses the ``ILevelReader`` service
to load entities, before it adds them to the world.

Please note that calling ``Load`` does not remove previous entities from the
world, so if you are loading a different level, remember to remove all
entities from the world that you do not want to have persist between levels.