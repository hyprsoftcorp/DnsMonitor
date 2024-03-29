cd ./Hyprsoft.Dns.Monitor

dotnet publish -c Release -r linux-amd64
dotnet publish -c Release -r linux-arm64

cd ..

docker build --rm -t hyprsoft/hyprsoft.dns.monitor:1.3.2-linux-amd64 -f Dockerfile.linux-amd64 .
docker push hyprsoft/hyprsoft.dns.monitor:1.3.2-linux-amd64

docker build --rm -t hyprsoft/hyprsoft.dns.monitor:1.3.2-linux-arm64 -f Dockerfile.linux-arm64 .
docker push hyprsoft/hyprsoft.dns.monitor:1.3.2-linux-arm64