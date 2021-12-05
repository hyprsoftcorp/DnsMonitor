using System;

namespace Hyprsoft.Dns.Monitor.Providers
{
    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(string message) : base(message) { }
        
        public ProviderNotFoundException(string message, Exception innerExeption) : base(message, innerExeption) { }
    }
}
