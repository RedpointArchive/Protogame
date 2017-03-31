#!/bin/bash

sphinx-build . _build/
sphinx-autobuild -i *.kate-swp -i *.new -i *.gen.rst -i .git -i .git/index.lock . _build/
