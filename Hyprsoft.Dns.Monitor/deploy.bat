echo off

dotnet publish -c Release -r linux-arm
if exist "bin\Release\net5.0\linux-arm\publish\config.json" del "bin\Release\net5.0\linux-arm\publish\config.json"
scp -r bin\Release\net5.0\linux-arm\publish\* pi@devrpi4:/usr/bin/dnsmonitor

dotnet publish -c Release -r linux-x64
if exist "bin\Release\net5.0\linux-x64\publish\config.json" del "bin\Release\net5.0\linux-x64\publish\config.json"
