
dotnet publish -c Release -r linux-arm
del "bin\Release\netcoreapp3.1\linux-arm\publish\appsettings.json"
dotnet publish -c Release -r linux-x64