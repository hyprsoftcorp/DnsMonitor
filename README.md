# Introduction 
This .NET 9 "worker" background service monitors your public IP address for changes and updates the appropriate DNS records to allow remote access to your home/office network.
It performs very basic functions similar to [changeip.com](https://changeip.com), [dyndns.com](https://dyndns.com), [easydns.com](https://easydns.com), and [no-ip.com](https://noip.com).

## Getting Started
<b>This service must be run inside your home/office network, not in the cloud.</b>

Since .NET 9 supports a number of operating systems, this app can be run almost anywhere.
The service ensures that anytime our public IP address changes, our DNS records are automatically updated so that we can always access various services on our network from anywhere in the world.

### Supported Public IP Address Providers
1. [IpifyPublicIpProvider](https://www.ipify.org/) - No authentication required and is free.
2. [HyprsoftPublicIpProvider](https://hyprsoft.com/) - Requires an API key and secret.  Used only for testing.

### Supported DNS Providers
1. [CloudflareDnsProvider](https://www.cloudflare.com/) - Requires a [Cloudflare API token](https://dash.cloudflare.com/profile/api-tokens) with permissions to edit DNS records.  Set the DnsProviderApiCredentials.ApiSecret to your Cloundflare API token.
2. [GoDaddyDnsProvider](https://www.godaddy.com/) - Requires a [GoDaddy API key and secret](https://developer.godaddy.com/keys).
1. [HyprsoftDnsProvider](https://hyprsoft.com/) - No authentication required and can be used for testing.  This provider returns random IP addresses.

## Sample Settings File
The 'appsettings.json' file is required and here is a sample.  <b>This file COULD contain sensitive/secret information</b>.  Protect it accordingly!
~~~json
{
  "MonitorSettings": {
    "DnsProvider": "HyprsoftDnsProvider", // Options: CloudflareDnsProvider, GoDaddyDnsProvider, HyprsoftDnsProvider
    "Domains": [ "example.com" ],
    "DnsProviderApiCredentials": {
      "ApiKey": "super-secret-api-key",
      "ApiSecret": "super-secret-api-secret"
    },
    "PublicIpProviderApiCredentials": {
      "ApiKey": null,
      "ApiSecret": null
    },
    "CheckInterval": "00:10:00"
  }
}
~~~

## Docker Setup
See our [Docker Hub](https://hub.docker.com/repository/docker/hyprsoft/hyprsoft.dns.monitor) for more details.  <b>Make sure to adjust your host volume mapping file path for the 'appsettings.json' file</b>.
### Linux (amd64)
```
docker run -it -d --name dnsmonitor --restart always -v C:\Docker\dnsmonitor\appsettings.json:/app/appsettings.json hyprsoft/hyprsoft.dns.monitor:2.0.0-linux-amd64
```
### Linux (arm64)
```
docker run -it -d --name dnsmonitor --restart always -v /home/pi/dnsmonitor/appsettings.json:/app/appsettings.json hyprsoft/hyprsoft.dns.monitor:2.0.0-linux-arm64
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

### Architecture
The architecture allows contributors to easily add additional public IP address and DNS providers by simply deriving from PublicIpProvider or DnsProvider and implementing the GetPublicIPAddressAsync() method or the GetDnsIPAddressAsync() and SetDnsIPAddressAsync() methods.

### Upgrading to Version 2
Version 2 has a number of breaking changes exclusively in the appsettings.json configuration file.  
Moving forward the [IpifyPublicIpProvider](https://www.ipify.org/) public IP address provider is automatically used with all DNS providers (i.e. Cloudflare and GoDaddy).
So there is no need for a separate configuration for the public IP provider.
The "CheckIntervalMinutes": 10 became "CheckInterval": "00:10:00" for more flexibility in periodic DNS checks.