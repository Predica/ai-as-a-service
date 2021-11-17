using System.Threading;
using System.Threading.Tasks;

namespace AwesomeBot.Contracts
{
    public interface ITranslationClient
    {
        ValueTask<string> Translate(string text, string fromLangCode, string toLangCode, CancellationToken cancellationToken);
    }
}