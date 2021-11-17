using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;

namespace AwesomeBot.Contracts
{
    public interface ITranslationService
    {
        ValueTask<Language> Detect(ITurnContext turnContext, CancellationToken cancellationToken = default);
        ValueTask<string> Translate(string text, Language language, CancellationToken cancellationToken = default);
    }
}