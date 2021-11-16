using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;

namespace AwesomeBot.Contracts
{
    public interface IQnAService
    {
        IAsyncEnumerable<QnA> Ask(ITurnContext turnContext, CancellationToken cancellationToken);
    }
}