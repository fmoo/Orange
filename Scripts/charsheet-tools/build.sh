#!/bin/bash
SCRIPTPATH="$( cd "$(dirname "$0")" >/dev/null 2>&1 ; pwd -P )"
pushd $SCRIPTPATH/src
pyinstaller -F charsheettool.py && cp dist/charsheettool.exe ../
rm -rfv ./__pycache__ *.spec build dist
cd ..
rm -rfv charsheet-tools/ charsheet-tools.zip
mkdir charsheet-tools
cp *.exe charsheet-tools
cp *.cmd charsheet-tools
echo 'compressing archive...'
powershell.exe Compress-Archive charsheet-tools/ charsheet-tools.zip
rm -rfv charsheet-tools/
popd
