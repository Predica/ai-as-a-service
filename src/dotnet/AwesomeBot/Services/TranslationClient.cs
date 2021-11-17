using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Newtonsoft.Json.Serialization;

namespace AwesomeBot.Services
{
    public class TranslationClient : ITranslationClient
    {
        private readonly HttpClient _httpClient;
        private readonly TranslationOption _option;
        private readonly JsonSerializerOptions _options =  new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        public TranslationClient(HttpClient httpClient, IOptions<TranslationOption> options)
        {
            _option = options?.Value ??
                      throw new OptionsValidationException(nameof(TranslationOption), typeof(TranslationOption), new []{"option doesn't exist"});
            _httpClient = httpClient;
        }

        public async ValueTask<string> Translate(string text, string fromLangCode, string toLangCode, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(new[]
            {
                new TranslationDto
                {
                    Text = text
                }
            });
            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{_option.Endpoint}/translate?api-version=3.0&from={fromLangCode}&to={toLangCode}");
            httpRequest.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            httpRequest.Headers.Add("Ocp-Apim-Subscription-Key", _option.EndpointKey);
            httpRequest.Headers.Add("Ocp-Apim-Subscription-Region", _option.EndpointRegion);
            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var content = await response.Content.ReadAsStreamAsync(cancellationToken);
            var item = await JsonSerializer.DeserializeAsync<IEnumerable<MainTranslationDto>>(content,_options, cancellationToken);
            if (item is { })
            {
                var translation = item.FirstOrDefault()?.Translations?.FirstOrDefault()?.Text ?? string.Empty;
                return translation;
            }
            return string.Empty;
        }

        private class MainTranslationDto
        {
            public IEnumerable<TranslationDto> Translations { get; set; }
        }
        private class TranslationDto
        {
            public string Text { get; set; }
        }
    }
}