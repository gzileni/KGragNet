using Qdrant.Client;
using Qdrant.Client.Grpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Qdrant.Client.Grpc.Conditions;

namespace KGragNet
{

    /*
     * KGragQdrant provides methods to interact with a Qdrant vector database.
     */
    public class KGragQdrant : IKGragQdrant
    {
        private readonly QdrantClient qdrantClient = new QdrantClient("localhost", 6334);

        public KGragQdrant(IKGragQDrantConfig config) 
        { 
            this.qdrantClient = new QdrantClient(config.Host, config.Port);
        }

        public KGragQdrant(QdrantClient qdrantClient)
        {
            this.qdrantClient = qdrantClient;
        }

        public KGragQdrant(string host, int port = 6334)
        {
            this.qdrantClient = new QdrantClient(host, port);
        }

        /*
         * Create creates a new collection in Qdrant with the specified parameters.
         * Args:
         *  collectionName (string): The name of the collection to create.
         *  vectorSize (ulong): The size of the vectors to be stored in the collection. Default is 1536.
         *  distance (Distance): The distance metric to use for the collection. Default is Distance.Cosine.
         */
        public async void CreateCollection(string collectionName, ulong vectorSize = 1536, Distance distance = Distance.Cosine)
        {
            var vectorParams = new VectorParams
            {
                Size = vectorSize,
                Distance = distance
            };
            await this.qdrantClient.CreateCollectionAsync(collectionName, vectorParams);
            await this.qdrantClient.SearchAsync(collectionName, new float[vectorSize], 1);
        }

        public async Task<UpdateResult> Add(string collectionName, List<PointStruct> points)
            => await this.qdrantClient.UpsertAsync(collectionName: collectionName, points: points);

        /*
         * Search performs a vector similarity search in the specified collection.
         */
        public async Task<IReadOnlyList<ScoredPoint>> Search(string collectionName, float[] vector, ulong limit = 3)
            => await this.qdrantClient.SearchAsync(collectionName: collectionName, query: vector, limit: limit);

        public async Task<IReadOnlyList<ScoredPoint>> Query(string collectionName, float[] vector, ulong limit = 3)
            => await this.qdrantClient.QueryAsync(collectionName: collectionName, query: vector, limit: limit);

        /*
         * MatchKeyword creates a condition to match a specific keyword in a given field.
         * Args:
         *  field (string): The name of the field to filter on.
         *  keyword (string): The keyword to filter by.
         * Returns:
         *  Condition: A condition object representing the keyword match.
         */
        public async Task<IReadOnlyList<ScoredPoint>> Query(string collectionName, float[] vector, string field, string keyword, ulong limit = 3)
        {
            Condition filter = MatchKeyword(field, keyword);
            return await this.qdrantClient.QueryAsync(
                collectionName: collectionName,
                query: vector,
                limit: limit,
                filter: filter,
                payloadSelector: true
            );
        }

        /*
         * DeleteCollection deletes the specified collection from Qdrant.
         * Args:
         *  collectionName (string): The name of the collection to delete.
         */
        public async Task DeleteCollection(string collectionName) => await this.qdrantClient.DeleteCollectionAsync(collectionName);

        /*
         * CreatePoint creates a new PointStruct with a unique ID, the provided vectors, and an optional payload.
         * Args:
         *  vectors (float[]): The vector representation of the point.
         *  payload (Dictionary<string, string>, optional): Additional metadata to associate with the point. Default is null.  
         */
        public PointStruct CreatePoint(float[] vectors, Dictionary<string, string> payload = null)
        {
            var point = new PointStruct
            {
                Id = new PointId { Uuid = Guid.NewGuid().ToString() },
                Vectors = vectors
            };

            // Add created_at timestamp to payload
            point.Payload.Add("created_at", DateTime.UtcNow.ToString("o"));
            // Add additional payload if provided
            if (payload != null)
            {
                foreach (var item in payload)
                {
                    point.Payload.Add(item.Key, item.Value);
                }
            }
            return point;
        }

        /*
         * CreatePointsByDict creates a list of PointStructs from the provided vectors and a dictionary of nodes.
         * Each entry in the nodes dictionary is used to create a PointStruct with the corresponding ID in the payload.
         * Args:
         *  vectors (float[]): The vector representation of the points.
         *  nodes (Dictionary<string, string>): A dictionary where each key-value pair represents a node's metadata.
         * Returns:
         *  List<PointStruct>: A list of PointStructs created from the nodes dictionary.
         */
        public List<PointStruct> CreatePointsByDict(float[] vectors, Dictionary<string, string> nodes, Dictionary<string, string> metadata = null)
        {
            var points = new List<PointStruct>();

            foreach (var node in nodes)
            {
                var payload = new Dictionary<string, string>
                {
                    { "id", node.Value }
                };

                // Add additional metadata if provided
                if (metadata != null)
                {
                    foreach (var item in metadata)
                    {
                        payload[item.Key] = item.Value;
                    }
                }

                var point = CreatePoint(vectors, payload);
                points.Add(point);
            }
            return points;
        }
    }
}
