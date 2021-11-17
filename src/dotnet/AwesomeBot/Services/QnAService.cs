using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Azure.AI.Language.QuestionAnswering;
using Microsoft.Bot.Builder;

namespace AwesomeBot.Services
{
    public class QnAService : IQnAService
    {
        private readonly QuestionAnsweringClient _maker;
        private readonly QuestionAnsweringProject _project;
        private readonly ITranslationService _translationService;

        private readonly AnswersOptions _makerOptions = new()
        {
            Size = 2,
            ConfidenceThreshold = 0.5f
        };

        public QnAService(QuestionAnsweringClient maker, QuestionAnsweringProject project, ITranslationService translationService)
        {
            _maker = maker;
            _project = project;
            _translationService = translationService;
        }

        public async IAsyncEnumerable<QnA> Ask(ITurnContext turnContext, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var lang = await _translationService.Detect(turnContext, cancellationToken);
            var text = turnContext.Activity.Text;
            if ((lang & Language.Polish) != 0)
            {
                text = await _translationService.Translate(text, lang, cancellationToken);
            }
            var qnaResults = await _maker.GetAnswersAsync(text, _project,_makerOptions, cancellationToken);
            if (qnaResults is not { Value.Answers.Count: > 0 }) yield break;
            foreach (var queryResult in qnaResults.Value.Answers)
            {
                var answer = await _translationService.Translate(queryResult.Answer, Language.English, cancellationToken);
                var question =  await _translationService.Translate(queryResult.Questions.First(), Language.English, cancellationToken);
                yield return new QnA(answer, question);
            }
        }
    }
}