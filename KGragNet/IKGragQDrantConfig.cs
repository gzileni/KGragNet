using Qdrant.Client.Grpc;

namespace KGragNet
{
    /*
     * IKGragQDrantConfig defines the configuration settings for connecting to a Qdrant vector database.
     */
    public interface IKGragQDrantConfig
    {
        /* The distance metric to use for the collection. Default is Distance.Cosine. */
        Distance Distance { get; set; }

        /* The host of the Qdrant database. Default is "localhost". */
        string Host { get; set; }

        /* The port of the Qdrant database. Default is 6334. */
        int Port { get; set; }

        /* The size of the vectors to be stored in the collection. Default is 1536. */
        ulong VectorSize { get; set; }
    }
}