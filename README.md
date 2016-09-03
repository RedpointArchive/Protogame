Protogame
==========

_A powerful cross-platform game engine for C#_

Protogame is a cross-platform game engine for C#, built on top of MonoGame.  It is highly modular and flexible, built on the ideal that you should only have to pull in dependencies for what you want to use.  Many parts of Protogame can be used either together or seperately, and you can use many of the APIs without adopting the core architecture.

Protogame is one of the few engines for MonoGame that has built-in support for the development of 3D multiplayer games.

![Minute of Mayhem Screenshot](https://cloud.githubusercontent.com/assets/504826/18222411/7caee5b0-71da-11e6-9c77-08fe27a12360.png)

Documentation
----------------

Documentation for Protogame is available on [Read the Docs](https://protogame.readthedocs.org/en/latest/).

Supported Platforms
--------------------

Protogame supports the following platforms:

  * Windows
  * MacOS
  * Linux

Experimental support is also available for:

  * Android (via Xamarin)
  * Ouya (via Xamarin)

Features
----------

Protogame is a collection of modules, each of which you can optionally use in your game.  These modules include:

* **Core & Graphics**
  * Full support for both 2D and 3D games.
  * Highly modular architecture allowing you to replace and extend any part of the engine without modifying library code.
  * A rendering pipeline which supports both forward and **deferred** rendering pipelines.
  * Built-in forward and deferred surface shaders with support for normal and specular mappings.
  * The deferred pipeline supports thousands of lights without requiring any additional programming.
  * Support for adding post-processing and custom render passes with a single line of code.
  * Support for automatic batching of rendering requests.
* **Inputs & Events**
  * A highly powerful and extensible event system, which can be used to propagate events of any type through a system while still maintaining type safety.
  * An input system which allows you to prevent double consumation of input events (e.g. two entities handling the same input event).
  * A flexible binding system that allows you to map events to actions in a fluent syntax.
* **Components**
  * A very powerful, yet optional component system that supports compile-time validation of dependencies.
  * Built-in components that making adding rendering and physics to entities easy.
  * A network synchronisation component that enables multiplayer entities with very few lines of code.
  * A component hierarchy which supports automatically combining transforms efficiently.
* **Asset System**
  * Highly flexible asset system that can be extended to support custom game data.
  * Real-time asset reload, allowing you to replace textures, models and other game assets while the game is running, and see your changes without restarting the game or compiling assets.
  * Support for compiling textures, models, audio, confguration, shaders & effects, fonts, language translations, level data and more into cross-platform, fast to load, binary formats.
  * Support for uber shaders, allowing you to define and compile multiple variations of the same shader and choose which variant to use at runtime.
* **Level Editors**
  * A 3D level editor based on the Sony ATF Level Editor.  *This is a WIP, and we'll be improving the workflow in the future.*
  * Support for loading levels from the Ogmo Level Editor.
* **Physics**
  * A full physics engine provided by Jitter, which synchronises with the entity and component hierarchy.
  * The physics system is fully managed, with no native dependencies.
* **Networking & Servers**
  * Full support for building multiplayer games, with both low-level and high-level APIs.
  * A networking protocol which supports sending both realtime and reliable messages.
  * A full framework for writing dedicated servers, and server code for all entities and components.
  * Support for using [Hive Multiplayer Services](https://hivemp.com) for game lobbies and user sessions, as well as performing NAT punchthrough for peer-to-peer games.
  * Input prediction APIs for client prediction.
  * Interpolation and extrapolation support for smooth rendering of interval data from servers.
  * A framework which allows you to build cheat-free multiplayer games.
* **User Interface**
  * A full user interface framework built upon a container system.
  * Ships with support for many common UI elements, such as buttons, lists, tree views, scrollable views, and many more.
  * Full skinning support for custom rendering of UI elements.
  * Optionally declarative XML syntax for user interfaces, imported through the asset system.
* **Coroutines**
  * A coroutine system that allows you to use `async` / `await` transparently with coroutines.
* **Debugging & Profiling Framework**
  * Utilities and supporting methods for showing debug information in the world, such as the positions and dimensions of entities, as well as any other arbitrary data you want to display.
  * Support for an in-game profiler, which immediately shows developers where the game is spending time during game loops.
  * The profiler can also expose additional information, such as network state and rendering calls.
  * A framework for capturing and handling errors in your game, with the ability to extend it to automatially submit error reports to online systems.
* **In-Game Console**
  * An in-game console which allows developers and players to access commands by pressing `~`.
  * A logging framework which supports fast logging of messages, with automatic grouping of similar messages in the console.
* **Miscellaneous Features**
  * Support for setting up common camera viewports.
  * LZMA compression utilities for data compression
  * APIs for streaming data from hardware such as webcams and device cameras.
  * Process and analyse image data, such as images sourced from cameras.
  * APIs for generating various types of noise, useful for procedural generation.
  * Various useful classes for mathematical calculations.
  * A framework for pooling instances of objects.
  * A fast, 64-bit addressable octree that operates both setting and getting data in `O(1)` time.
  * A scripting framework that includes a built-in scripting language ideal for pure logic evaluation (*LogicScript*).
  * Support for extending the scripting framework with other scripting languages as needed.

How to Contribute
--------------------

To contribute to Protogame, submit a pull request to `RedpointGames/Protogame`.

The developer chat is hosted on [Gitter](https://gitter.im/RedpointGames/Protogame)

[![Gitter](https://badges.gitter.im/RedpointGames/Protogame.svg)](https://gitter.im/RedpointGames/Protogame?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Providing Feedback / Obtaining Support
-----------------------------------------

To provide feedback or get support about issues, please file a GitHub issue on this repository.

License Information
---------------------

Protogame is licensed under the MIT license.

```
Copyright (c) 2015 Various Authors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
```

Community Code of Conduct
------------------------------

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).
