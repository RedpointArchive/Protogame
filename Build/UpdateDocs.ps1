param()

trap
{
    Write-Host "An error occurred"
    Write-Host $_
    Write-Host $_.Exception.StackTrace
    exit 1
}

$ErrorActionPreference = 'Stop'

cd $PSScriptRoot\..
$root = Get-Location

# Run the Protogame documentation generator.
.\ProtogameDocsTool\bin\Windows\AnyCPU\Debug\ProtogameDocsTool.exe
if ($LastExitCode -ne 0) { exit 1 }

# Clone the repository we're going to update.
if (Test-Path "$root\Protogame.Docs")
{
    pushd $root\Protogame.Docs
    git reset --hard HEAD
    if ($LastExitCode -ne 0) { exit 1 }
    git fetch --all
    if ($LastExitCode -ne 0) { exit 1 }
    git checkout -f origin/master
    if ($LastExitCode -ne 0) { exit 1 }
    popd
} 
else
{
    # We need to clone the repository
    git clone --progress git@github.com:hach-que/Protogame.Docs.git $root\Protogame.Docs
    if ($LastExitCode -ne 0) { exit 1 }
}

# Commit and push
pushd $root\Protogame.Docs
git reset --hard HEAD
if ($LastExitCode -ne 0) { popd; exit 1 }
git checkout -fB master origin/master
if ($LastExitCode -ne 0) { popd; exit 1 }
Copy-Item -Force $root\ProtogameDocsTool\bin\Windows\AnyCPU\Debug\Protogame.combined.xml $root\Protogame.Docs\Protogame.combined.xml
git add Protogame.combined.xml
if ($LastExitCode -ne 0) { popd; exit 1 }
git commit -m "Automatic update of generated documentation by build server"
if ($LastExitCode -ne 0) { popd; exit 1 }
git push
if ($LastExitCode -ne 0) { popd; exit 1 }
popd
exit 0