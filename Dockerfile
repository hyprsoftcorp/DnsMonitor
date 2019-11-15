#FROM mcr.microsoft.com/dotnet/core/runtime:2.2.6-stretch-slim-arm32v7
FROM mcr.microsoft.com/dotnet/core/runtime:2.2
COPY Hyprsoft.Dns.Monitor/bin/Release/netcoreapp2.2/publish/ app/
COPY Hyprsoft.Dns.Monitor/appsettings.json app/
ENTRYPOINT ["dotnet", "app/Hyprsoft.Dns.Monitor.dll"]

#docker build --rm -t hyprsoft/hyprsoft.dns.monitor:linux-x64 -f Dockerfile .
#docker push hyprsoft/hyprsoft.dns.monitor:linux-x64
#docker run -d --restart always hyprsoft/hyprsoft.dns.monitor:linux-x64