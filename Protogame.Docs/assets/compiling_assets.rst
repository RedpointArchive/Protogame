Compiling assets
===================

As mentioned above, some types of assets require compilation before they can
be used.  This can happen at runtime for desktop platforms in Debug mode, but
you must precompile assets for mobile or release builds.

To compile assets, we use the Protogame asset tool, which is included in the
solution you use to build your game.

The Protogame asset tool accepts the following options to determine how
it will compile assets:

.. raw:: html

    <style type="text/css">
      .wy-table-responsive table td, .wy-table-responsive table th {
        white-space: initial;
      }
      
      .wy-table td, .rst-content table.docutils td, .rst-content table.field-list td {
        vertical-align: top;
      }
      
      .rst-content table.docutils td p {
        font-size: 100%;
        margin-bottom: 16px;
      }
      
      .rst-content table.docutils td ul {
        margin-bottom: 0px;
      }
    </style>

+--------------------------------------------------+--------------------------------------------------------------------+
| Option                                           | Description                                                        |
+==================================================+====================================================================+
| ``-o ...\MyProject.Content\compiled``            | This should be the "compiled" folder in your content project.      |
|                                                  | It specifies the location to output the compiled assets            |
|                                                  | for each target platform.                                          |
+--------------------------------------------------+--------------------------------------------------------------------+
| ``-p Windows -p Linux ...``                      | This option is specified for each platform                         |
|                                                  | you want to target.  If you wanted to target Linux and Android,    |
|                                                  | you would specify ``-p Linux -p Android``.                         |
|                                                  |                                                                    |
|                                                  | The supported platform names are (these are from the               |
|                                                  | ``TargetPlatform`` enumeration in Protogame):                      |
|                                                  |                                                                    |
|                                                  | - Windows                                                          |
|                                                  | - Xbox360                                                          |
|                                                  | - WindowsPhone                                                     |
|                                                  | - iOS                                                              |
|                                                  | - Android                                                          |
|                                                  | - Linux                                                            |
|                                                  | - MacOSX                                                           |
|                                                  | - WindowsStoreApp                                                  |
|                                                  | - NativeClient                                                     |
|                                                  | - Ouya                                                             |
|                                                  | - PlayStationMobile                                                |
|                                                  | - WindowsPhone8                                                    |
|                                                  | - RaspberryPi                                                      |
+--------------------------------------------------+--------------------------------------------------------------------+
| ``-a MyAssembly.dll``                            | This specifies additional assemblies to load during asset          |
| ``--assembly=MyAssembly.dll``                    | compilation.  This is required if you have defined custom          |
|                                                  | asset types as the Protogame asset tool needs to know where        |
|                                                  | to locate the loader, saver and (optionally) compiler classes      |
|                                                  | for the custom assets.                                             |
+--------------------------------------------------+--------------------------------------------------------------------+
| ``-m remote``                                    | This changes the behaviour of the Protogame asset tool so that     |
|                                                  | instead of compiling assets, it instead listens for remote         |
|                                                  | requests for asset compilation.  See the :ref:`remote-compilation` |
|                                                  | section for more information on remote effect compilation.         |
+--------------------------------------------------+--------------------------------------------------------------------+

Often for large projects you will want to encapsulate this command in a
batch or shell script so that you don't need to type the command each time.

For a Windows machine, a batch script placed in the content project (such
that this file resides next to the "assets" and "compiled" directories), might
look like:

.. literalinclude:: snippet/compile.bat
    :language: batch

For Linux, the shell script might look like:

.. literalinclude:: snippet/compile.sh
    :language: shell