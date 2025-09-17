namespace KGragNet
{
    /*
     * IKGragGraphConfig holds the configuration details for connecting to a graph database.
     */
    public interface IKGragGraphConfig
    {
        /* The host of the graph database. Default is "localhost". */
        string Host { get; set; }

        /* The password for the database user. Default is "password". */
        string Password { get; set; }

        /* The port of the graph database. Default is 7687. */
        int Port { get; set; }

        /* The URI of the graph database. */
        string Uri { get; }

        /* The username for the database. Default is "neo4j". */
        string User { get; set; }

        /* The name of the database. Default is "neo4j". */
        string Name { get; set; }
    }
}