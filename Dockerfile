
# linux-arm32
FROM mcr.microsoft.com/dotnet/core/runtime:3.1.3-buster-slim-arm32v7
COPY Hyprsoft.Dns.Monitor/bin/Release/netcoreapp3.1/linux-arm/publish/ app/
ENTRYPOINT ["dotnet", "app/Hyprsoft.Dns.Monitor.dll"]

# docker build --rm -t hyprsoft/hyprsoft.dns.monitor:linux-arm -f Dockerfile .
# docker push hyprsoft/hyprsoft.dns.monitor:linux-arm
# docker run -d --restart always hyprsoft/hyprsoft.dns.monitor:linux-arm

# linux-x64
#FROM mcr.microsoft.com/dotnet/core/runtime:3.1.3-buster-slim
#COPY Hyprsoft.Dns.Monitor/bin/Release/netcoreapp3.1/linux-x64/publish/ app/
#ENTRYPOINT ["dotnet", "app/Hyprsoft.Dns.Monitor.dll"]

# docker build --rm -t hyprsoft/hyprsoft.dns.monitor:linux-x64 -f Dockerfile .
# docker push hyprsoft/hyprsoft.dns.monitor:linux-x64
# docker run -d --restart always hyprsoft/hyprsoft.dns.monitor:linux-x64