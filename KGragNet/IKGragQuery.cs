using System.Threading.Tasks;

namespace KGragNet
{
    public interface IKGragQuery
    {
        /*
         * Query performs a query using the provided text.
         * Args:
         *  text (string): The input text to query.
         */
        Task Query(string text);
    }
}