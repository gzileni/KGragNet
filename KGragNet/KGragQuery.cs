using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGragNet
{
    public abstract class KGragQuery : KGragStore, IKGragQuery
    {

        /*
         * KGragQuery provides methods to query both the Qdrant vector database and the Neo4j graph database.
         */
        private async Task<List> GetVectorIds(string text)
        {
            try
            {
                // Get embeddings for the input text.
                float[] embed = GetEmbeddings(text);
                // Search Qdrant vector database for similar entities.
                var searchResult = await Vector.Search(QdrantConfig.CollectionName, embed, QdrantConfig.Limit);
                return searchResult.Select(r => r.Id).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting vector IDs: {ex.Message}");
                throw;
            }
        }

        /*
         * GetSubGraph retrieves a subgraph from the Neo4j graph database based on the provided entity IDs.
         */
        private async Task<List<Dictionary<string, object>>> GetSubGraph(List ids)
        {
            try
            {
                // Query Neo4j graph database for subgraph related to the retrieved entity IDs.
                string query = """
                MATCH (e:Entity)-[r1]-(n1)-[r2]-(n2)
                WHERE e.id IN $entity_ids
                RETURN e, r1 as r, n1 as related, r2, n2
                UNION
                MATCH (e:Entity)-[r]-(related)
                WHERE e.id IN $entity_ids
                RETURN e, r, related, null as r2, null as n2
                """;
                var parameters = new { entity_ids = ids };
                var graphResult = await Graph.Query(query, parameters);

                var subgraph = new List<Dictionary<string, object>>();
                foreach (var record in graphResult)
                {
                    // Prima relazione
                    if (record is IDictionary<string, object> recDict)
                    {
                        subgraph.Add(new Dictionary<string, object>
                        {
                            ["entity"] = recDict["e"],
                            ["relationship"] = recDict["r"],
                            ["related_node"] = recDict["related"]
                        });

                        // Seconda relazione, se esiste
                        if (recDict["r2"] != null && recDict["n2"] != null)
                        {
                            subgraph.Add(new Dictionary<string, object>
                            {
                                ["entity"] = recDict["related"],
                                ["relationship"] = recDict["r2"],
                                ["related_node"] = recDict["n2"]
                            });
                        }
                    }
                }
                return subgraph;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting subgraph: {ex.Message}");
                throw;
            }
        }

        /*
         * FormatGraphContext formats the retrieved subgraph into a structured context with nodes and edges.
         */
        private Dictionary<string, object> FormatGraphContext(List<Dictionary<string, object>> subgraph)
        {
            if (subgraph == null || subgraph.Count == 0)
                throw new ArgumentException("Il subgraph è vuoto o nullo.");

            var nodes = new HashSet<string>();
            var edges = new List<string>();

            foreach (var entry in subgraph)
            {
                if (!entry.TryGetValue("entity", out var entityObj) ||
                    !entry.TryGetValue("related_node", out var relatedObj) ||
                    !entry.TryGetValue("relationship", out var relationshipObj))
                    continue;

                if (entityObj == null || relatedObj == null || relationshipObj == null)
                    continue;

                if (!entityObj.TryGetValue("name", out var entityNameObj) ||
                    !relatedObj.TryGetValue("name", out var relatedNameObj) ||
                    !relationshipObj.TryGetValue("type", out var relationshipTypeObj))
                    continue;

                var entityName = entityNameObj?.ToString();
                var relatedName = relatedNameObj?.ToString();
                var relationshipType = relationshipTypeObj?.ToString();

                if (entityName == null || relatedName == null || relationshipType == null)
                    continue;

                nodes.Add(entityName);
                nodes.Add(relatedName);

                edges.Add($"{entityName} {relationshipType} {relatedName}");
            }

            return new Dictionary<string, object>
            {
                ["nodes"] = nodes.ToList(),
                ["edges"] = edges
            };
        }

        /*
         * Query processes the input text to retrieve relevant information from both Qdrant and Neo4j,
         * and formats it into a structured context.
         */
        public async Task Query(string text)
        {
            try
            {
                // Retrieve entity IDs from Qdrant based on the input text.
                var ids = await GetVectorIds(text);
                // Retrieve the subgraph from Neo4j based on the retrieved entity IDs.
                var subgraph = await GetSubGraph(ids);
                // Format the subgraph into a structured context.
                var graphContext = FormatGraphContext(subgraph);
                // TODO: Run LLM to generate response

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during query: {ex.Message}");
                throw;
            }

        }
    }

}
