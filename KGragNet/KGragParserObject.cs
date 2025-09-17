using System.Collections.Generic;

namespace KGragNet
{
    public class KGragParserObject : IKGragParserObject
    {
        private Dictionary<string, string> nodes;
        private Dictionary<string, string>[] relationships;

        public KGragParserObject(Dictionary<string, string> nodes, Dictionary<string, string>[] relationships)
        {
            this.nodes = nodes;
            this.relationships = relationships;
        }

        public Dictionary<string, string> Nodes { get => nodes; set => nodes = value; }
        public Dictionary<string, string>[] Relationships { get => relationships; set => relationships = value; }
    }
}
