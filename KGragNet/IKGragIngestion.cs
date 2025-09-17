using System.Collections.Generic;
using System.Threading.Tasks;

namespace KGragNet
{
    public interface IKGragIngestion
    {
        Task IngestTo(string text, Dictionary<string, string> nodes, Dictionary<string, string>[] relationships, Dictionary<string, string> metadata = null);
    }
}