using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;

namespace AwesomeBot.Services
{
    public class QnAService : IQnAService
    {
        private readonly QnAMaker _maker;
        private readonly QnAMakerOptions _makerOptions = new()
        {
            Top = 2,
            ScoreThreshold = 0.5f
        };

        public QnAService(QnAMaker maker)
        {
            _maker = maker;
        }

        public async IAsyncEnumerable<QnA> Ask(ITurnContext turnContext, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var qnaResults = await _maker.GetAnswersAsync(turnContext, _makerOptions);
            if (qnaResults is not { Length: > 0 }) yield break;
            foreach (var queryResult in qnaResults)
            {
                yield return new QnA(queryResult.Answer, queryResult.Questions.First());
            }
        }
    }
}