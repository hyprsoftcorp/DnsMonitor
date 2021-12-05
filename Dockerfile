# linux
FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY Hyprsoft.Dns.Monitor/bin/Release/net5.0/publish/ app/
ENTRYPOINT ["dotnet", "app/Hyprsoft.Dns.Monitor.dll"]

# docker build --rm -t hyprsoft/hyprsoft.dns.monitor -f Dockerfile .
# docker push hyprsoft/hyprsoft.dns.monitor:latest
# docker run -it -d --name dnsmonitor --restart always -v C:\Docker\dnsmonitor\config.json:/app/config.json hyprsoft/hyprsoft.dns.monitor:latest

# linux-arm64
# FROM mcr.microsoft.com/dotnet/runtime:5.0.12-bullseye-slim-arm64v8
# COPY Hyprsoft.Dns.Monitor/bin/Release/net5.0/linux-arm64/publish/ app/
# ENTRYPOINT ["dotnet", "app/Hyprsoft.Dns.Monitor.dll"]

# docker build --rm -t hyprsoft/hyprsoft.dns.monitor:linux-arm64 -f Dockerfile .
# docker push hyprsoft/hyprsoft.dns.monitor:linux-arm64
# docker run -it -d --name dnsmonitor --restart always -v /home/pi/dnsmonitor/config.json:/app/config.json hyprsoft/hyprsoft.dns.monitor:linux-arm64