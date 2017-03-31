ILevelReader
===============

The level reader service provides a low-level mechanism for converting a
stream into an enumerable of ``IEntity``.

By default, the implementation bound to this service reads Ogmo Editor levels,
although you could rebind this service to a different implementation, and
the level manager service would thus be able to load a level format of your
choice.

In order to load levels with the default Ogmo Editor reader implementation,
you must bind an entity class to the ``ISolidEntity`` interface.  The reader
will instantiate new objects of the bound type for the "solids" layer
in the Ogmo Editor level.

.. literalinclude:: snippet/ogmo_solid_entity_binding.cs
    :language: csharp

On construction of the bound type, it is provided with 4 arguments: the X
position ``x``, Y position ``y``, width ``width`` and height ``height`` of the solid tile.
As with automatic factories, the names of the parameters in the constructor
must match those listed.

When using tiles in the Ogmo Editor, you must bind tile entities with the
appropriate name in the dependency injection kernel so that they can be
resolved as the level is read.  For example, if you use a dirt tileset in
Ogmo Editor that has a name "dirt", then you need to create the following
binding.

.. literalinclude:: snippet/ogmo_tile_entity_binding.cs
    :language: csharp

On construction of DirtTileEntity, it is provided with 4 arguments; the pixel
X position ``x``, pixel Y position ``y``, tile X position ``tx``, and tile Y position
``ty``.  As with automatic factories, the names of the parameters in the constructor
must match those listed.