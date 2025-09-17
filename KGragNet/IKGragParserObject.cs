using System.Collections.Generic;

namespace KGragNet
{
    public interface IKGragParserObject
    {
        Dictionary<string, string> Nodes { get; set; }
        Dictionary<string, string>[] Relationships { get; set; }
    }
}