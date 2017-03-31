.. _faq:

Frequently Asked Questions
===========================

If there is a question not answered here, ask `@hachque <https://twitter.com/hachque>`_ on Twitter.

How do I get started?
------------------------

We recommend reading the :ref:`walkthrough-your-first-game` as this will
guide you through the initial download and setup, and lead onto future tutorials
which cover rendering, entities and handling input.

What platforms are supported?
--------------------------------

Windows, Mac OS X and Linux are all currently supported and tested development platforms.
These desktop platforms are also supported targets for running games made with Protogame.

Android and Ouya are currently supported mobile platforms for running games.  iOS support is
expected in the future.

MonoGame supports other platforms as well, such as PlayStation Mobile and Native Client.  We
don't know whether Protogame will work on these platforms, but it's likely they will work
with some changes depending on platform limitations.

For more information, refer to the table below.

  * "Supported" indicates that we actively develop and run games on this platform.
  * "Changes Needed" means that you would need to make changes to the engine for it to work.
  * An empty space means it's not supported and / or we don't know how well it will work.

==================  =======================================  ==============================
Platform            Games can be developed on this platform  Games can run on this platform
==================  =======================================  ==============================
Windows             Supported                                Supported
Mac OS X            Supported                                Supported
Linux               Supported                                Supported
Android                                                      Supported
Ouya                                                         Supported
iOS                                                          Supported
Windows Phone
Windows 8 (Metro)
==================  =======================================  ==============================

What platforms can compile assets?
------------------------------------

Asset compilation is needed to turn source information (such as PNG and FBX files)
into a version of the texture, model or otherwise that is suitable for the
game to use at runtime.  Since each platform needs assets in different formats,
you have to compile your assets for the target platforms.

Support is determined by:

  * The type of asset
  * The platform you are targetting (for running the game)
  * The platform you are compiling the asset on (where you are developing)

The general rule of thumb is, **if you are developing on Windows, you can
target all platforms**.  Windows has the strongest support for compiling assets
and can target all available platforms that MonoGame supports.

Linux support can compile all types of assets, although shaders require another
computer running Windows for remote compilation (effects require DirectX to
parse the shader language).  Fonts must be under the /usr/share/fonts/truetype
folder for them to be picked up, and the font name indicates the TTF filename.

Font compilation is likely to change in the future, with font assets being
specified from a source TTF instead of by name.

**You will need a Windows machine to compile shaders.**

See the :ref:`asset-management` article for more information
on compile assets, cross-compilation and compilation of shaders on non-Windows
platforms.

Missing MSVCR100.dll or MSVCP100.dll
-------------------------------------

Compilation of model assets under Windows requires `the Visual C++ runtime`_.

.. _the Visual C++ runtime: http://www.microsoft.com/en-us/download/confirmation.aspx?id=30679

FBX animations are not importing from Maya correctly
------------------------------------------------------

The FBX importer has some restrictions about the structure of the animations
that you import.  These relate to "optimizations" that Maya and other 3D modelling
tools do in their export; unfortunately these optimizations are not compatible
with the importer.

The restrictions are as follows:

  * **All bones in the hierarchy must have a joint offset.**  Maya is
    known to optimize hierarchy that doesn't have a joint offset, and this
    causes transformations to be applied to the wrong node.
  * **You must have at least one frame for each bone with a non-default value
    for translation, rotation and scaling.** If you leave the default
    translation (0, 0, 0), rotation (0, 0, 0), scaling (1, 1, 1) for a given
    bone, and you export a rig with no animations, relevant nodes for the bone will
    not be exported and Protogame will not be able to re-apply the
    animation transformations because the target node does not exist.  An easy
    solution to this issue is to have the base FBX file also contain an animation,
    so Maya will not remove the nodes required to re-apply the animation.

Note that these restrictions only apply when you are dealing with animations.  Non-animated
FBX files will import just fine.

Running games under Mono for Windows does not work
------------------------------------------------------

Running Protogame-based games under Mono for Windows is not supported.  Please use
the Microsoft.NET runtime instead.

This means that you should use Visual Studio Express instead of Xamarin Studio if your
target is the Windows platform (i.e. on Windows use Xamarin Studio only if the runtime platform
is Android or iOS).

'FastDev directory creation failed' when deploying to Android
----------------------------------------------------------------

This appears to be a transient error with Xamarin Android.  To fix this issue, right-click on
the project for your game in Xamarin Studio and select "Options".  In the Options dialog that
appears, under Build -> Android Build, uncheck the "Fast Assembly Deployment" option.
