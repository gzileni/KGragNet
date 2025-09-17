
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGragNet
{

    /*
     * KGragGraphConfig holds the configuration details for connecting to a graph database.
     */
    

    public class KGragGraph : IKragGraph
    {
        private IDriver driver;
        private string dbUri;
        private string dbUser;
        private string dbPassword;
        private string dbName = "neo4j";

        public KGragGraph(string dbUri, string dbUser, string dbPassword)
        {
            DbUri = dbUri;
            DbUser = dbUser;
            DbPassword = dbPassword;
        }

        public KGragGraph(string dbUri, string dbUser, string dbPassword, string dbName = "neo4j") : this(dbUri, dbUser, dbPassword)
        {
            DbName = dbName;
        }

        public KGragGraph(IKGragGraphConfig config) : this(config.Uri, config.User, config.Password, config.Name) { }

        public string DbUri { get => dbUri; set => dbUri = value; }
        public string DbUser { get => dbUser; set => dbUser = value; }
        public string DbPassword { get => dbPassword; set => dbPassword = value; }
        public string DbName { get => dbName; set => dbName = value; }

        public async Task<bool> Connect()
        {
            try
            {
                /* 
                 * If both user and password are provided, use them for authentication.
                 * Otherwise, connect without authentication.
                 */
                bool hasAuth = !string.IsNullOrEmpty(this.dbUser) && !string.IsNullOrEmpty(this.dbPassword);
                IAuthToken authToken = hasAuth ? AuthTokens.Basic(this.dbUser, this.dbPassword) : null;
                this.driver = GraphDatabase.Driver(this.dbUri, authToken);
                /* 
                 * Verify the connectivity to the database.
                 */
                await this.driver.VerifyConnectivityAsync();
                Console.WriteLine("Connected to database successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                return false;
            }

        }

        public async Task<IResultSummary> Create(string query, object parameters = null)
        {
            var e = driver.ExecutableQuery(query);

            if (parameters != null)
                e = e.WithParameters(parameters);

            if (!string.IsNullOrEmpty(this.dbName))
            {
                var q = new QueryConfig(database: this.DbName);
                e = e.WithConfig(q);
            }

            // Execute the query and get the result summary
            var result = await e.ExecuteAsync();

            // Summary information
            var summary = result.Summary;
            Console.WriteLine($"Created {summary.Counters.NodesCreated} nodes in {summary.ResultAvailableAfter.Milliseconds} ms.");
            return summary;
        }

        public async Task<IReadOnlyList<IRecord>> Query(string query)
        {
            // Prepare the executable query
            var e = this.driver.ExecutableQuery(query);

            if (!string.IsNullOrEmpty(this.dbName))
            {
                var q = new QueryConfig(database: this.DbName);
                e = e.WithConfig(q);
            }

            // Execute the query and get the result
            var result = await e.ExecuteAsync();
            return result.Result;
        }

        public async Task Close() => await this.driver.DisposeAsync();
    }
}
