#!/bin/bash
cd "${0%/*}"

echo "Compiling assets..."
cd assets
mono ../../Protogame/ProtogameAssetTool/bin/Debug/ProtogameAssetTool.exe \
    -a ../../MyProject/bin/Debug/MyProject.exe \
    -o ../compiled \
    -p Windows \
    -p Linux \
    -p Android
cd ..