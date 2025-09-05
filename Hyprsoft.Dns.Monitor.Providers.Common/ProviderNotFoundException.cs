using System;

namespace Hyprsoft.Dns.Monitor.Providers.Common
{
    public class ProviderNotFoundException : Exception
    {
        public ProviderNotFoundException(string message) : base(message) { }
        
        public ProviderNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
