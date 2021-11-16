using System.Collections.Generic;
using AdaptiveCards.Templating;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace AwesomeBot
{
    public class AdaptiveCardsTemplates : IAdaptiveCardsTemplates
    {
        public IActivity EchoCard(string text)
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
            
            var card =  template.Expand(data);

            return CreateCard(card);
        }

        public IActivity QnACard(IEnumerable<QnA> qna)
        {
            var templateJson = @"
            {
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.4"",
                ""body"": [
                    {
                        ""$data"": ""${{answers}}"",
                        ""type"": ""Container"",
                        ""style"": ""emphasis"",
                        ""items"":[ 
                            {
                                ""type"": ""ColumnSet"",
                                ""columns"": [
                                    {{
                                        ""type"": ""Column"",
                                        ""items"":[ 
                                           {
                                               ""type"": ""TextBlock"",                                       
                                                ""text"": ""${question}"",
                                                ""wrap"": true
                                            },
                                            {
                                                ""type"": ""TextBlock"",
                                                ""weight"": ""Bolder"",
                                                ""spacing"": ""None"",
                                                ""text"": ""${answer}"",
                                                ""isSubtle"": false,
                                                ""wrap"": true
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }";
            
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);
            var data = new
            {
                Answers = qna
            };
            
            var card =  template.Expand(data);

            return CreateCard(card);
        }

        private IActivity CreateCard(string card)
            => new Activity()
            {
                Attachments = new[]
                {
                    new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = JsonConvert.DeserializeObject(card),
                    }
                }
            };
    }
}