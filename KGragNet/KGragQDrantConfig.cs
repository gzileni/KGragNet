using Qdrant.Client.Grpc;

namespace KGragNet
{
    public class KGragQDrantConfig : IKGragQDrantConfig
    {
        private string host = "localhost";
        private int port = 6334;
        private ulong vectorSize = 1536;
        private Distance distance = Distance.Cosine;
        private string collectionName = "memory_collection";

        public string Host { get => host; set => host = value; }
        public int Port { get => port; set => port = value; }
        public ulong VectorSize { get => vectorSize; set => vectorSize = value; }
        public Distance Distance { get => distance; set => distance = value; }
        public string CollectionName { get => collectionName; set => collectionName = value; }

        public KGragQDrantConfig() { }

        public KGragQDrantConfig(string host)
        {
            Host = host;
            Port = 6334;
            VectorSize = 1536;
            Distance = Distance.Cosine;
        }

        public KGragQDrantConfig(string host, int port) : this(host)
        {
            Host = host;
            Port = port;
        }

        public KGragQDrantConfig(string host, int port, ulong vectorSize, Distance distance) : this(host, port)
        {
            VectorSize = vectorSize;
            Distance = distance;
        }

        public KGragQDrantConfig(string host, int port, ulong vectorSize, Distance distance, string collectionName) : this(host, port, vectorSize, distance)
        {
            CollectionName = collectionName;
        }
    }
}
