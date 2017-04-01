.. _remote-compilation:

Remote compilation
========================

Compilation of effects (shaders) requires a Windows machine because the process
requires access to the DirectX APIs to parse the HLSL syntax.  However, it is
possible for the compilation to be performed remotely; that is, the compilation
is invoked on a Linux machine (either during the game or part of the asset
tool) and the request is routed over the network to a Windows machine
where the compilation actually occurs.  The resulting shader code is then
returned back over the network to the original machine.

For non-Windows machines, the default effect compiler searches the local
network for a Windows machine running the remote compiler.  On a Windows
machine, you can start the remote compiler with the following command:

::

    ProtogameAssetTool.exe -m remote

This tool requires port 4321 and 8080 to be open in the Windows firewall; when
it starts for the first time it will attempt to do this and you may see
UAC dialogs appear as it requests administrator permission.

As long as there is at least one Windows machine running this tool on the
same subnet as the Linux or Mac OS X machines, they will transparently compile
effects correctly as if it was being done locally.

The remote compiler requires both the latest DirectX and the Visual C++ 2010
Redistributable installed on the machine that is running it in order to
invoke the shader compilation library.