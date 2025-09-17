using Neo4j.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGragNet
{
    /*
     * IKragGraph defines the interface for interacting with a graph database.
     */
    public interface IKragGraph
    {
        /*
         * DbName is the name of the database to connect to.
         */
        string DbName { get; set; }
        
        /*
         * DbPassword is the password for the database user.
         */
        string DbPassword { get; set; }
        
        /*
         * DbUri is the URI of the database.
         */
        string DbUri { get; set; }

        /*
         * DbUser is the username for the database.
         */
        string DbUser { get; set; }

        /*
         * Connect establishes a connection to the graph database.
         * Returns true if the connection is successful, otherwise false.
         */
        Task<bool> Connect();

        /*
         * Close closes the connection to the graph database.
         */
        Task Close();

        /*
         * Create executes a write query against the graph database.
         * Args:
         *   query (string): The Cypher query to execute.
         *   parameters (object): Optional parameters for the query.
         * Returns an IResultSummary containing information about the execution of the query.
         */
        Task<IResultSummary> Create(string query, object parameters = null);

        /*
         * Query executes a read query against the graph database.
         * Args:
         *   query (string): The Cypher query to execute.
         * Returns a list of IRecord containing the results of the query.
         */
        Task<IReadOnlyList<IRecord>> Query(string query);
    }
}