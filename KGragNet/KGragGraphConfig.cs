namespace KGragNet
{
    public class KGragGraphConfig : IKGragGraphConfig
    {
        private string host = "localhost";
        private int port = 7687;
        private string user = null;
        private string password = null;
        private string name = "neo4j";

        public string Host { get => host; set => host = value; }
        public int Port { get => port; set => port = value; }
        public string User { get => user; set => user = value; }
        public string Password { get => password; set => password = value; }
        public string Name { get => name; set => name = value; }

        public string Uri => $"neo4j://{Host}:{Port}";
        public KGragGraphConfig() { }
        public KGragGraphConfig(string host)
        {
            Host = host;
            Port = 7687;
            User = null;
            Password = null;
        }
        public KGragGraphConfig(string host, int port) : this(host)
        {
            Host = host;
            Port = port;
        }

        public KGragGraphConfig(string host, int port, string user = null, string password = null, string name = null) : this(host, port)
        {
            User ??= user;
            Password ??= password;
            Name = name ?? "neo4j";
        }
    }
}
