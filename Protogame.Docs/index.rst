Welcome to Protogame
======================================

`Protogame`_ is a cross-platform .NET game engine.  It aims to allow developers
to prototype games quickly during the early stages of development, and then
scale those games into full projects.

Protogame is fully open source and available under an MIT license.

.. _Protogame: https://protogame.org/

This documentation is organised into several sections, depending on what you 
need to achieve:

 * :ref:`general-info`
 * :ref:`basic-walkthroughs`
 * :ref:`engine-api`
 * :ref:`engine-utilities`
 * :ref:`third-party`
 
..  * :ref:`advanced-walkthroughs`
..  * :ref:`ide`

.. raw:: html

    <style type="text/css">
      div.toctree-wrapper > p.caption > span.caption-text {
        display: none;
      }
    </style>

.. _general-info:
 
General Information
--------------------

.. toctree::
    :maxdepth: 2
    :caption: General Information

    general/faq

.. _basic-walkthroughs:
 
Basic Walkthroughs
--------------------

If you're just starting out with Protogame, it is **highly recommended** that
you follow the first walkthrough: :ref:`walkthrough-your-first-game`.  This
will walk you through creating your first game with absolutely nothing installed
or set up.

.. toctree::
    :maxdepth: 2
    :caption: Basic Walkthroughs

    walkthrough/your_first_game
    walkthrough/rendering_textures
    walkthrough/add_a_player
    walkthrough/handling_input

.. _advanced-walkthroughs:
 
.. Advanced Walkthroughs
.. --------------------
.. 
.. These walkthroughs cover advanced topics in Protogame.
.. 
.. .. toctree::
..     :maxdepth: 2
..     :caption: Advanced Walkthroughs

.. _`engine-api`:

Engine API
----------------------------

This documentation covers the core concepts of the engine, as well as the APIs
that are available for use.  It is organised into sections that roughly
correlate with various aspects of the engine; core structure, graphics APIs,
networking, etc.

.. toctree::
    :maxdepth: 2
    :caption: Engine APIs

    core_arch/index
    core_api/index
    assets/index
    audio/index
    graphics/index
    graphics_2d/index
    graphics_3d/index
    caching/index
    events/index
    physics/index
    level/index
    commands/index
    ai/index
    particles/index
    network/index
    server/index
    scripting/index
    sensor/index
    ui/index

.. _`engine-utilities`:

Engine Utilities
----------------------------

Protogame provides various smaller utility APIs that serve very specific purposes.
These APIs are often only used for specific games, or when targeting specific
platforms.

.. toctree::
    :maxdepth: 2
    :caption: Engine Utilities
    
    analytics/index
    performance/index
    compression/index
    collision/index
    image_processing/index
    platforming/index
    pooling/index
    structure/index
    noise/index
    math/index
    extensions/index

.. _`third-party`:

Third-Party APIs
----------------------------

Protogame makes wide usage of third-party libraries to provide functionality, most
notably MonoGame.  Classes and methods in those third-party libraries are documented
in the section below.

.. toctree::
    :maxdepth: 1
    :caption: Third-Party APIs
    
    monogame.framework/index
    monogame.framework.content.pipeline/index
    jitter/index
    protoinject/index
    newtonsoft.json/index
    ndesk.options/index

.. .. _ide:
.. 
.. MonoDevelop and Visual Studio Extensions
.. -----------------------------------------
.. 
.. Protogame ships with a version of MonoDevelop that contains additional functionality
.. that makes it easier to write and develop your games.  It also ships with a
.. Visual Studio extension that provides the same functionality as the MonoDevelop
.. extension for those users that are using Windows.
.. 
.. These extensions allow for hot reloading of game code, visual world and scene editing
.. and visualisation of game assets.
.. 
.. This documentation covers how to use those IDE extensions and how to write your
.. own editor tools for Protogame.
.. 
.. .. toctree::
..     :maxdepth: 1
..     :caption: IDE Extensions

.. _utilities:

Other Documentation
----------------------------

Miscellanous documentation that doesn't belong under any other category.

.. toctree::
    :maxdepth: 1
    :caption: Other Documentation
    
    _internal/index