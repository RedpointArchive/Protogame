Types of assets
===================

Textures
----------------

Textures can be provided in source form with a standard PNG image.  They
represent images that can be rendered at runtime in your game.

Textures are platform-specific and thus they require compilation.  When the
game is running on a desktop platform, compilation can be done transparently
from the PNG file specified, but for either mobile or release builds you
need to compile texture assets using the Protogame asset tool.

Sounds
----------------

Sounds are used for playing back audio during the game.  Sounds are provided
in a standard Wave format.  This format is reasonably specific due to the
requirement for it to be playable across platforms.

As specified in the XNA documentation, the requirements for the Wave format
are:

.. note::

  The Stream object must point to the head of a valid PCM wave file.
  Also, this wave file must be in the RIFF bitstream format.

  The audio format has the following restrictions:

  - Must be a PCM wave file
  - Can only be mono or stereo
  - Must be 8 or 16 bit
  - Sample rate must be between 8,000 Hz and 48,000 Hz

Sounds are platform-specific and thus they require compilation.  When the
game is running on a desktop platform, compilation can be done transparently
from the Wave file specified, but for either mobile or release builds you
need to compile sound assets using the Protogame asset tool.

3D Models
----------------

3D models are used to render shapes when building a 3D game.  They are provided
in source form with an FBX file, and have support for animations in this format.

3D models must be converted into a format for fast loading at runtime and thus
they require compilation.  The library that reads FBX files runs on desktop
platforms and thus FBX files can be read at runtime on these platforms, but for
either mobile or release builds you need to compile model assets using
the Protogame asset tool.

Effects
----------------

Effects are cross-platform shaders that are written in a format similar to
HLSL and are provided in a ``.fx`` file on disk.  Since effects are written in
a derivative of HLSL, effects can only be compiled on a Windows machine.

Protogame provides a remote compilation mechanism that allows effects
to be sent from a non-Windows machine (such as Linux) over the network,
compiled on a Windows machine for the appropriate target type, returned and
loaded transparently.  For more information, see :ref:`remote-compilation`.

Sounds are platform-specific and thus they require compilation.  When the
game is running on a desktop platform, compilation can be done transparently
from the effect file specified, but for either mobile or release builds you
need to compile effect assets using the Protogame asset tool.

AI logic
----------------

Unlike other forms on assets, AIs are defined as classes in your code and
carry the ``[Asset("name")]`` attribute on the class declaration, while also
inheriting from ``AIAsset``.  An example of an AI asset that does not
perform any functionality would be as follows:

.. literalinclude:: snippet/ai_logic.cs
    :language: csharp
    
AI assets do not require any additional processing to work as they are compiled
as part of your codebase.  AI assets are designed as a way for custom assets
(such as enemy types) to specify how they should behave in the game.

Ogmo Editor Levels
--------------------

Ogmo Editor levels are saved as ``.oel`` files by Ogmo Editor.  ``.oel`` files
can be read and loaded on any platform, and are internally converted to
normal source asset files.  When you compile assets, the result will be
an ``.asset`` file that embeds the original level file's data.
