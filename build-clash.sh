#!/bin/sh
mkdir -p ./dist/bin
rm -rf ./dist/bin/clash-*

pushd clash

echo 'Building Clash for GNU/Linux 64 Bit'
GOOS=linux GOARCH=amd64 go build -o ../dist/bin/clash-linux-x64 github.com/Dreamacro/clash
echo 'Building Clash for GNU/Linux 32 Bit'
GOOS=linux GOARCH=386 go build -o ../dist/bin/clash-linux-x32 github.com/Dreamacro/clash

echo 'Building Clash for macOS 64 Bit'
GOOS=darwin GOARCH=amd64 go build -o ../dist/bin/clash-darwin-x64 github.com/Dreamacro/clash

echo 'Building Clash for Windows 64 Bit'
GOOS=windows GOARCH=amd64 go build -o ../dist/bin/clash-win32-x64.exe github.com/Dreamacro/clash
echo 'Building Clash for Windows 32 Bit'
GOOS=windows GOARCH=386 go build -o ../dist/bin/clash-win32-x32.exe github.com/Dreamacro/clash

popd
