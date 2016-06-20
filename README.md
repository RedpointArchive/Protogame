Protogame v4 Beta
===================

[![Join the chat at https://gitter.im/RedpointGames/Protogame](https://badges.gitter.im/RedpointGames/Protogame.svg)](https://gitter.im/RedpointGames/Protogame?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

**NOTE:** Version 4 is still a work-in-progress!

* The latest version (version 4) is available on the `master` branch.
* The last version of version 3 is available at the `v3.0` tag.
* The last version of version 2 is available at the `v2.0` tag.
* The last version of version 1 is available at the `v1.0` tag.

New features in Version 4
-----------------------------

Version 4 breaks compatibility with version 3, in order to introduce the following features:

 * Entities now have transforms instead of explicit X, Y and Z properties.  This is necessary to
   support hierarchical transform combination.
 * Worlds no longer have an `Entities` property.  Instead the dependency injection hierarchy is
   used to track entities in the world.  This means entities you inject in the constructor will
   automatically be added to the world.  To add entities to the world, you now need to inject
   `IHierarchy` and `INode` and call `_hierarchy.AddChild(_hierarchy.CreateNodeForObject(entity))`.
   We'll have a cleaner API for entity management in the future that wraps this call.
 * We expect to significantly rework the platforming and bounding box APIs to make them work cleaner
   with the new transform composition system.

Protogame is an open source game engine (MIT licensed) written on top of MonoGame and C#, designed to allow game developers to rapidly produce games within 48 hours.

Read more about Protogame at the website: http://protogame.org/
