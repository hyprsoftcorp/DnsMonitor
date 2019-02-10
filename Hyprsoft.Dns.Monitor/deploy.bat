dotnet publish -r linux-arm -c Release
del /s /q d:\shared\publish\*.*
xcopy .\bin\Release\netcoreapp2.2\linux-arm\publish d:\shared\publish /i /d /y /e