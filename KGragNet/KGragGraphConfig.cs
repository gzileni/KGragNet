namespace KGragNet
{
    public class KGragGraphConfig : IKGragGraphConfig
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 7687;
        public string User { get; set; } = "neo4j";
        public string Password { get; set; } = "password";
        public string Name { get; set; } = "neo4j";

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

        public KGragGraphConfig(string host, int port, string user, string password) : this(host, port)
        {
            User = user;
            Password = password;
        }

        public KGragGraphConfig(string host, int port, string user, string password, string name) : this(host, port, user, password)
        {
            Name = name;
        }
    }
}
