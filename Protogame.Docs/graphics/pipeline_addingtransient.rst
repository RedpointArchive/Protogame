Adding transient render passes
===========================

Transient render passes are additional render passes that apply
only on the frame that they are appended.

Transient render passes are primarily used for applying temporary
effects to the screen, such as applying a post-processing
effect when the player dies.

Appending at runtime
-----------------------

You can append transient render passes to the game at runtime using the 
:dotnet:method:`Protogame.IRenderContext.AppendTransientRenderPass(IRenderPass)`
method:

.. literalinclude:: snippet/game_runtimetransient.cs
    :language: csharp
    :emphasize-lines: 14