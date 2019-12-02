namespace Labyrinth.Models {
    public class ClashConfig {
        public ushort Port { get; set; } = 0;
        public ushort SocksPort { get; set; } = 0;
        public ushort RedirPort { get; set; } = 0;
        public bool AllowLan { get; set; } = false;
        public string Mode { get; set; } = "";
        public string LogLevel { get; set; } = "";
    }
}
