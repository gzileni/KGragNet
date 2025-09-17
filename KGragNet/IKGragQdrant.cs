using Qdrant.Client.Grpc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGragNet
{
    /*
     * IKGragQdrant defines the interface for interacting with a Qdrant vector database.
     */
    public interface IKGragQdrant
    {
        /*
         * Add adds a list of points to the specified collection in Qdrant.
         * Args:
         *  collectionName (string): The name of the collection to add points to.
         *  points (List<PointStruct>): The list of points to add.
         * Returns:
         *  UpdateResult: The result of the upsert operation.
         */
        Task<UpdateResult> Add(string collectionName, List<PointStruct> points);

        /*
         * Create creates a new collection in Qdrant with the specified parameters.
         * Args:
         *  collectionName (string): The name of the collection to create.
         *  vectorSize (ulong): The size of the vectors to be stored in the collection. Default is 1536.
         *  distance (Distance): The distance metric to use for the collection. Default is Distance.Cosine.
         */
        void CreateCollection(string collectionName, ulong vectorSize = 1536, Distance distance = Distance.Cosine);

        /*
         * Query performs a vector similarity search in the specified collection.
         * Args:
         *  collectionName (string): The name of the collection to query.
         *  vector (float[]): The query vector.
         *  limit (ulong): The maximum number of results to return. Default is 3.
         * Returns:
         *  IReadOnlyList<ScoredPoint>: A list of scored points representing the most similar points to the query vector.
         */
        Task<IReadOnlyList<ScoredPoint>> Query(string collectionName, float[] vector, ulong limit = 3);

        /*
         * Query performs a vector similarity search in the specified collection with an additional keyword filter.
         * Args:
         *  collectionName (string): The name of the collection to query.
         *  vector (float[]): The query vector.
         *  field (string): The name of the field to filter on.
         *  keyword (string): The keyword to filter by.
         *  limit (ulong): The maximum number of results to return. Default is 3.
         * Returns:
         *  IReadOnlyList<ScoredPoint>: A list of scored points representing the most similar points to the query vector that match the keyword filter.
         */
        Task<IReadOnlyList<ScoredPoint>> Query(string collectionName, float[] vector, string field, string keyword, ulong limit = 3);

        /*
         * DeleteCollection deletes the specified collection from Qdrant.
         * Args:
         *  collectionName (string): The name of the collection to delete.
         */
        Task DeleteCollection(string collectionName);
    }
}