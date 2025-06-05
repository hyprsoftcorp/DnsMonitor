namespace Hyprsoft.Dns.Monitor.Providers
{
    internal sealed class ZoneRecordResponse
    {
        public sealed class Error
        {
            public int Code { get; set; }
            public string Message { get; set; } = null!;
        }

        public sealed class ResultNested
        {
            public string Id { get; set; } = null!;
            public string Name { get; set; } = null!;
            public string Cname_suffix { get; set; } = null!;
            public bool Paused { get; set; }
            public string[] Permissions { get; set; } = [];
            public string Type { get; set; } = null!;
        }

        public Error[] Errors { get; set; } = [];
        public bool Success { get; set; }
        public ResultNested[] Result { get; set; } = [];
    }
}
