using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;

namespace AwesomeBot.Contracts
{
    public interface IContextOrchestrator
    {
        ValueTask<BotContext> Decide(ITurnContext turnContext, CancellationToken cancellationToken);
    }
}