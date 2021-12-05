# Introduction 
This .NET 5.0 "worker" background service monitors your public IP address for changes and updates the appropriate DNS records to allow remote access to your home/office network.
It performs very basic functions similar to [changeip.com](https://changeip.com), [dyndns.com](https://dyndns.com), [easydns.com](https://easydns.com), and [no-ip.com](https://noip.com).

## Getting Started
<b>This service must be run inside your home/office network, not in the cloud.</b>

Since .NET 5 supports a number of operating systems, this app can be run almost anywhere.
In our test case we are running this service on a Raspberry PI 4 (4GB) using the Rasberry PI OS Lite (Bullseye).
The service ensures that anytime our public IP address changes, our DNS records are automatically updated so that we can always access various services on our network from anywhere in the world.

### Supported Public IP Address Providers
1. [IpifyPublicIpProvider](https://www.ipify.org/) - No authentication required and is free.
2. [HyprsoftPublicIpProvider](https://hyprsoftidentity.azurewebsites.net/) - Requires an API key and secret.

### Supported DNS Providers
1. [HyprsoftDnsProvider](https://www.hyprsoft.com/) - No authentication required and can be used for testing.  This provider returns random IP addresses.
2. [GoDaddyDnsProvider](https://www.godaddy.com/) - Requires a [GoDaddy API key and secret](https://developer.godaddy.com/keys).

## Sample Service Settings File
The 'config.json' file is required and here is a sample.  This file will contain sensitive/secret information.
~~~json
{
  "Domains": [ "subdomain1.mydomain.com", "subdomain2.mydomain.com" ],
  "DnsProviderApiCredentials": {
    "ProviderKey": "HyprsoftDnsProvider",
    "ApiKey": "[optional value if API authentication is required]",
    "ApiSecret": "[optional value if API authentication is required]"
  },
  "PublicIpProviderApiCredentials": {
    "ProviderKey": "IpifyPublicIpProvider",
    "ApiKey": "[optional value if API authentication is required]",
    "ApiSecret": "[optional value if API authentication is required]"
  },
  "CheckIntervalMinutes": 10
}
~~~

## Docker Setup
See our [Docker Hub](https://hub.docker.com/repository/docker/hyprsoft/hyprsoft.dns.monitor) for more details.  Make sure to adjust your host volume mapping file path for the 'config.json' file.
### Linux (amd64)
```
docker run -it -d --name dnsmonitor --restart always -v C:\Docker\dnsmonitor\config.json:/app/config.json hyprsoft/hyprsoft.dns.monitor:latest
```
### Linux (arm64)
```
docker run -it -d --name dnsmonitor --restart always -v /home/pi/dnsmonitor/config.json:/app/config.json hyprsoft/hyprsoft.dns.monitor:linux-arm64
```

## Automatic Service Startup on Linux
Create a dnsmonitor.service file using:
```
sudo nano /etc/systemd/system/dnsmonitor.service
```

### Sample dnsmonitor.service
This assumes the service binaries have been copied to the '/usr/bin/dnsmonitor' directory.
```
[Unit]
Description=Hyprsoft Dns Monitor
After=network.target
Require=network.target

[Service]
ExecStart=/usr/bin/dnsmonitor/Hyprsoft.Dns.Monitor
WorkingDirectory=/usr/bin/dnsmonitor/

[Install]
WantedBy=multi-user.target
```

Start the DNS monitor service:
```
sudo systemctl enable dnsmonitor.service
sudo systemctl daemon-reload
sudo systemctl start dnsmonitor.service
```
Troubleshooting
```
sudo systemctl status dnsmonitor.service
sudo nano /usr/bin/dnsmonitor/app-log.log
```

## Service Startup on Windows IoT Core
```
schtasks /create /tn "Hyprsoft DNS Monitor" /tr c:\hyprsoft\dnsmonitor\Hyprsoft.Dns.Monitor.exe /sc onstart /ru System
```
See [scheduled tasks](https://docs.microsoft.com/en-us/windows/desktop/taskschd/schtasks) for more information.

### Architecture
The architecture allows contributors to easily add additional public IP address and DNS providers by simply deriving from PublicIpProvider or DnsProvider and implementing the GetPublicIPAddressAsync() method or the GetDnsIPAddressAsync() and SetDnsIPAddressAsync() methods.

