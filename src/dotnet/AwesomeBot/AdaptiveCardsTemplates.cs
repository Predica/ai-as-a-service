using AdaptiveCards.Templating;
using AwesomeBot.Contracts;

namespace AwesomeBot
{
    public class AdaptiveCardsTemplates : IAdaptiveCardsTemplates
    {
        public string EchoCard(string text)
        {
            var templateJson = @"
            {
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.4"",
                ""body"": [
                    {
                        ""type"": ""TextBlock"",
                        ""text"": ""echo ${text}!""
                    }
                ]
            }";
            
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);
            var data = new
            {
                Text = text
            };
            
            return  template.Expand(data);
        }
    }
}