Adding fixed render passes
===========================

Fixed render passes are the main render passes that apply
every frame, for the entire time that your game is running.

Fixed render passes are primarily set up when deriving from
:dotnet:method:`Protogame.CoreGame<TInitialWorld>.ConfigureRenderPipeline(IRenderPipeline,IKernel)`
in your own game class.  You can add and remove fixed render
passes at runtime using the methods on 
:dotnet:type:`Protogame.IRenderContext`.

Configuring at game start
--------------------------------

You can configure the fixed render passes used in game at game start,
by calling :dotnet:method:`Protogame.IRenderPipeline.AddFixedRenderPass(IRenderPass)`:

.. literalinclude:: snippet/game_addingfixed.cs
    :language: csharp
    :emphasize-lines: 12, 15

Modifying at runtime
-----------------------

You can add and remove fixed render passes at runtime, by calling
:dotnet:method:`Protogame.IRenderContext.AddFixedRenderPass(IRenderPass)`
and :dotnet:method:`Protogame.IRenderContext.RemoveFixedRenderPass(IRenderPass)`:

.. literalinclude:: snippet/game_runtimefixed.cs
    :language: csharp
    :emphasize-lines: 17-21