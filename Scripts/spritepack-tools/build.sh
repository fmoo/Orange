#!/bin/bash
SCRIPTPATH="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"
pushd $SCRIPTPATH/src
pyinstaller -F spritepackutil.py && cp dist/spritepackutil.exe ../
rm -rfv ./__pycache__ *.spec build dist
cd ..
rm -rfv spritepack-tools/ spritepack-tools.zip
mkdir spritepack-tools
cp *.exe spritepack-tools
cp *.cmd spritepack-tools
echo 'compressing archive...'
powershell.exe Compress-Archive spritepack-tools/ spritepack-tools.zip
rm -rfv spritepack-tools/
popd
