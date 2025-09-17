using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGragNet
{
    public abstract class KGragIngestion : KGragStore, IKGragIngestion
    {
        public KGragIngestion(KGragQDrantConfig qdrantConfig, KGragGraphConfig graphConfig)
            : base(qdrantConfig, graphConfig)
        {
        }

        /*
         * IngestToGraph ingests nodes and relationships into the Neo4j graph database.
         * Args:
         *   nodes (Dictionary<string, string>): A dictionary where the key is the node name and the value is the node ID.
         *   relationships (Dictionary<string, string>[]): An array of dictionaries representing relationships. Each dictionary should contain "source", "target", and "type" keys.
         */
        private async Task IngestToGraph(Dictionary<string, string> nodes, Dictionary<string, string>[] relationships)
        {
            try
            {
                /*
                 * Ingest to Neo4j graph database.
                 */
                foreach (var node in nodes)
                {
                    string query = $@"CREATE (n:Entity {{id: '{node.Value}', name: '{node.Key}'}}) RETURN n";
                    await Graph.Create(query);
                }

                /*
                 * Ingest relationships to Neo4j graph database.
                 */
                foreach (var relationship in relationships)
                {
                    string query = $@"
                        MATCH (a:Entity {{id: '{relationship["source"]}'}}), (b:Entity {{id: '{relationship["target"]}'}})
                        CREATE (a)-[r:{relationship["type"]}]->(b)
                        RETURN r";
                    await Graph.Create(query);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during graph ingestion: {ex.Message}");
                throw ex;
            }
        }

        /*
         * IngestTo ingests embeddings, nodes, relationships, and optional metadata into both the Neo4j graph database and the Qdrant vector database.
         * Args:
         *   embeds (float[]): An array of floats representing the embeddings to be ingested into Qdrant.
         *   nodes (Dictionary<string, string>): A dictionary where the key is the node name and the value is the node ID.
         *   relationships (Dictionary<string, string>[]): An array of dictionaries representing relationships. Each dictionary should contain "source", "target", and "type" keys.
         *   metadata (Dictionary<string, string>, optional): Additional metadata to associate with the points in Qdrant. Default is null.
         */
        public async Task IngestTo(
            string text,
            Dictionary<string, string> nodes,
            Dictionary<string, string>[] relationships,
            Dictionary<string, string> metadata = null
        )
        {
            try
            {
                // Ingest to Neo4j graph database.
                await IngestToGraph(nodes, relationships);

                // Get embeddings from the text using the implemented LLM model.
                var embeds = GetEmbeddings(text);

                // Create points for Qdrant.
                var points = Vector.CreatePointsByDict(embeds, nodes, metadata);
                // Ingest to Qdrant vector database.
                await Vector.Add(QdrantConfig.CollectionName, points);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during ingestion: {ex.Message}");
                throw ex;
            }
        }

        public async Task IngestTo(
            string text,
            Dictionary<string, string> metadata = null
        )
        {
            try
            {
                var parseResult = await Parser(text);
                await IngestTo(text, parseResult.Nodes, parseResult.Relationships, metadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during ingestion: {ex.Message}");
                throw ex;
            }
        }

        // TODO: Implement this method to get embeddings from a text using an LLM model.
        public abstract float[] GetEmbeddings(string text);

        // TODO: Implement this method to parse text and extract nodes and relationships.
        public abstract Task<IKGragParserObject> Parser(string text);
    }
}
