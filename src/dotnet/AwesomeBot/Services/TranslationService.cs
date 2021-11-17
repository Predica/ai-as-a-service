using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Azure.AI.TextAnalytics;
using Microsoft.Bot.Builder;

namespace AwesomeBot.Services
{
    public class TranslationService: ITranslationService
    {
        private readonly TextAnalyticsClient _client;
        private readonly ITranslationClient _translationClient;

        public TranslationService(TextAnalyticsClient client, ITranslationClient translationClient )
        {
            _client = client;
            _translationClient = translationClient;
        }
        public async ValueTask<Language> Detect(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var detectedLanguage = await _client.DetectLanguageAsync(turnContext.Activity.Text, cancellationToken: cancellationToken);
            var language = detectedLanguage.Value.Name;
            _ = Enum.TryParse<Language>(language, true, out var result);
            return result;
        }

        public async ValueTask<string> Translate(string text, Language language, CancellationToken cancellationToken = default)
        {
            var (fromLangCode, toLangCode) = language switch
            {
                Language.English => ("en", "pl"),
                Language.Polish => ("pl", "en"),
                _ => ("en", "pl")
            };
            var translate = await _translationClient.Translate(text, fromLangCode, toLangCode, cancellationToken);
            return translate;
        }
    }
}