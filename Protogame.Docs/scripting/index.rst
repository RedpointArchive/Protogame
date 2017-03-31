Scripting and Modding
================================

.. note::

    This documentation is a work-in-progress.
    
The scripting module provides mechanisms for loading and executing scripts written in
a shader-like language called LogicControl.  LogicControl is implemented purely in C#,
and can be used on all platforms.

Like shaders, method parameters and return values in LogicControl have semantics
declared on them, which are then used to call scripts from C#.  Scripts are integrated
into the asset management system, so scripts are loaded through the standard asset
management system, while still being available on disk for players to modify.

The scripting module also supports being extended with different languages, by creating
new implementations of ``IScriptEngine`` and ``IScriptEngineInstance``.

Thus the Protogame scripting module provides an extremely easy way of providing modding
and extension APIs for your games.