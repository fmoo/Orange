#!/bin/bash
SCRIPTPATH="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"
pushd $SCRIPTPATH/src
pyinstaller -F minitile.py && cp dist/minitile.exe ../
rm -rfv ./__pycache__ *.spec build dist
cd ..
rm -rfv autotile-packing-tools/ autotile-packing-tools.zip
mkdir autotile-packing-tools
cp *.exe autotile-packing-tools
cp *.cmd autotile-packing-tools
echo 'compressing archive...'
powershell.exe Compress-Archive autotile-packing-tools/ autotile-packing-tools.zip
rm -rfv autotile-packing-tools/
popd
