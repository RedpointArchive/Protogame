#!/bin/bash

set -e

COMMIT=$(git rev-parse HEAD)

# Check if the documentation already exists.
set +e
curl -f -X GET https://storage.googleapis.com/protogame-docs/$COMMIT/ProtogameDocsTool/bin/Windows/AnyCPU/Debug/Protogame.combined.xml >/dev/null 2>/dev/null
CODE=$?
set -e
if [ $CODE -eq 0 ]; then
  echo "Downloading existing file..."
  curl -f -X GET https://storage.googleapis.com/protogame-docs/$COMMIT/ProtogameDocsTool/bin/Windows/AnyCPU/Debug/Protogame.combined.xml > ../Protogame.combined.xml
  exit 0
fi

# Ask the documentation to be built on the build server.
echo "Starting build..."
curl -f -X GET "https://jenkins.redpointgames.com.au/buildByToken/buildWithParameters?job=Protogame-Docs-Build&token=BuildProtogameDocs&Commit=$COMMIT"

# Wait until the file appears on Google Cloud Storage.
echo -n "Building..."
i=0
while [ $i -lt 20 ]; do
  set +e
  curl -f -X GET https://storage.googleapis.com/protogame-docs/$COMMIT/ProtogameDocsTool/bin/Windows/AnyCPU/Debug/Protogame.combined.xml >/dev/null 2>/dev/null
  CODE=$?
  set -e
  if [ $CODE -eq 0 ]; then
    echo "!"
    break
  fi

  echo -n "."
  sleep 15
  i=$[$i+1]
done

echo "Downloading result file..."
curl -f -X GET https://storage.googleapis.com/protogame-docs/$COMMIT/ProtogameDocsTool/bin/Windows/AnyCPU/Debug/Protogame.combined.xml > ../Protogame.combined.xml
exit 0