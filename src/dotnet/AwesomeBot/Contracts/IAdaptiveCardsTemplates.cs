using System.Collections.Generic;
using AwesomeBot.Models;
using Microsoft.Bot.Schema;

namespace AwesomeBot.Contracts
{
    public interface IAdaptiveCardsTemplates
    {
        IActivity EchoCard(string text);
        IActivity QnACard(IEnumerable<QnA> questionAndAnswers);
    }
}