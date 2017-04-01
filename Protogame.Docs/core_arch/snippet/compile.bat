@echo off
cd %~dp0

echo Compiling assets...
cd assets
..\..\Protogame\ProtogameAssetTool\bin\Debug\ProtogameAssetTool.exe ^
    -a ..\..\MyProject\bin\Debug\MyProject.exe ^
    -o ..\compiled ^
    -p Windows ^
    -p Linux ^
    -p Android
cd ..