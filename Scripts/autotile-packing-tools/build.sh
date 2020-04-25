#!/bin/bash
SCRIPTPATH="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"
pushd $SCRIPTPATH/src
pyinstaller -F pack-minitile-a2.py && cp dist/pack-minitile-a2.exe ../
pyinstaller -F unpack-minitile-a2.py && cp dist/unpack-minitile-a2.exe ../
pyinstaller -F unpack-minitile-a2-gms16.py && cp dist/unpack-minitile-a2-gms16.exe ../
pyinstaller -F scale2x.py && cp dist/scale2x.exe ../
pyinstaller -F scale3x.py && cp dist/scale3x.exe ../
rm -rfv ./__pycache__ *.spec build dist
cd ..
rm -rfv autotile-packing-tools/ autotile-packing-tools.zip
mkdir autotile-packing-tools
cp *.exe autotile-packing-tools
echo 'compressing archive...'
powershell.exe Compress-Archive autotile-packing-tools/ autotile-packing-tools.zip
rm -rfv autotile-packing-tools/
popd
