.. _asset-management:

Asset Management
====================================================

Protogame uses the concept of assets to distinguish between logic / code and 
data / assets.  Unlike other engines, assets are intended to be used to define
not only low-level data such as textures, but also types of things in your
game, such as different types or enemies.

Examples of assets common to all Protogame games are:

  * Textures
  * Sounds
  * 3D models
  * Effects (shaders)
  * AI logic
  * Ogmo Editor levels

Examples of custom assets that you might define for your game are:

  * Enemy types
  * Spells
  * Weapons
  
.. toctree::
    :maxdepth: 2
    :caption: Table of contents
    :glob:
    
    conceptual_model
    using_assets
    types_of_assets
    compiling_assets
    remote_compilation
    apiindex