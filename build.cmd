cd ./Hyprsoft.Dns.Monitor

dotnet publish -c Release -r linux-x64
dotnet publish -c Release -r linux-arm64

cd ..

docker build --rm -t hyprsoft/hyprsoft.dns.monitor:2.0.0-linux-amd64 -f Dockerfile.linux-amd64 .
docker push hyprsoft/hyprsoft.dns.monitor:2.0.0-linux-amd64

docker build --rm --platform linux/arm64 -t hyprsoft/hyprsoft.dns.monitor:2.0.0-linux-arm64 -f Dockerfile.linux-arm64 .
docker push hyprsoft/hyprsoft.dns.monitor:2.0.0-linux-arm64