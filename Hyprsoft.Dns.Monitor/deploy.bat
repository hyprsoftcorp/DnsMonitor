
dotnet publish -c Release -r linux-arm
del "bin\Release\netcoreapp5.0\linux-arm\publish\appsettings.json"
dotnet publish -c Release -r linux-x64