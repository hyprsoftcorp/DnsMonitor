# Introduction 
This .NET 5.0 console app monitors your public IP address for changes and updates the appropriate DNS records to allow remote access to your home/office network.
It performs similar functions as [changeip.com](https://changeip.com), [dyndns.com](https://dyndns.com), [easydns.com](https://easydns.com), and [no-ip.com](https://noip.com) for free.


## Getting Started
<b>This app must be run inside your home/office network, not in the cloud.</b>
You will most likely need to open ports on your firewall to allow services back inside your network (like port 80 for HTTP).

Since .NET Core supports a number of operating systems, this app can be run almost anywhere.
In our test case we are running this app on a Raspberry PI 4 using the Rasberry PI OS.
The app ensures that anytime our public IP address changes, our DNS records are updated appropriately so that we can always access various services on our network from anywhere in the world.

## Architecture
The architecture allows developers to easily add additional public IP address providers and DNS providers by simply deriving from PublicIpProvider or DnsProvider and implementing the GetPublicIPAddressAsync method or the GetDnsIPAddressAsync and SetDnsIPAddressAsync methods.

### Supported Public IP Address Providers
1. [IpifyPublicIpProvider](https://www.ipify.org/) - No authentication required and is free.
2. [HyprsoftPublicIpProvider](https://hyprsoftidentity.azurewebsites.net/) - Requires authentication.

### Supported DNS Providers
1. [GoDaddyDnsProvider](https://www.godaddy.com/) - Requires a [GoDaddy API key and secret](https://developer.godaddy.com/keys).

## Sample App Settings File
The 'appsettings.json' file is required.  Here is a sample.
~~~json
{
  "FirstRun": true,
  "Domains": [ "subdomain1.mydomain.com", "subdomain2.mydomain.com" ],

  "PublicIpProviderKey": "IpifyPublicIpProvider",
  "PublicIpProviderApiKey": "[optional value if API authentication is required]",
  "PublicIpProviderApiSecret": "[optional value if API authentication is required]",

  "DnsProviderKey": "HyprsoftDnsProvider",
  "DnsProviderApiKey": "[optional value if API authentication is required]",
  "DnsProviderApiSecret": "[optional value if API authentication is required]"
}
~~~
Note: If the app is being run for the first time (i.e. "FirstRun": true) both the PublicIpProviderApiSecret and DnsProviderApiSecret will automatically be encrypted if they're present.
<b>If you're planning to automatically run this app at startup, you MUST run the app the first time with the same user context as on startup</b> otherwise any encrypted data in the settings file will be inaccessible.

## App Startup on Linux
Create a dnsmonitor.service file using:
```
sudo nano /etc/systemd/system/dnsmonitor.service
```

### Sample dnsmonitor.service
This assumes the app binaries have been copied to the '/usr/bin/dnsmonitor' directory.
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

## App Startup on Windows IoT Core
```
schtasks /create /tn "Hyprsoft DNS Monitor" /tr c:\hyprsoft\dnsmonitor\Hyprsoft.Dns.Monitor.exe /sc onstart /ru System
```
See [scheduled tasks](https://docs.microsoft.com/en-us/windows/desktop/taskschd/schtasks) for more information.