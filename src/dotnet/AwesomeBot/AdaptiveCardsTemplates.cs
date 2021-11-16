using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using AdaptiveCards;
using AdaptiveCards.Templating;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace AwesomeBot
{
    public class AdaptiveCardsTemplates : IAdaptiveCardsTemplates
    {
        public IActivity EchoCard(string text)
        {
            // return CreateCard(new AdaptiveCard(
            //     new AdaptiveSchemaVersion(1, 3))
            //     {
            //         Body = new List<AdaptiveElement>()
            //         {
            //             new AdaptiveTextBlock($"echo {text}")
            //         }
            //     });
            var templateJson = @"
            {
                ""type"": ""AdaptiveCard"",
                ""version"": ""1.3"",
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
                ""version"": ""1.3"",
                ""body"": [
                    {
                        ""$data"": ""${answers}"",
                        ""type"": ""Container"",
                        ""style"": ""emphasis"",
                        ""items"":[ 
                            {
                                ""type"": ""ColumnSet"",
                                ""columns"": [
                                    {
                                        ""type"": ""Column"",
                                        ""items"":[ 
                                           {
                                               ""type"": ""TextBlock"",                                       
                                                ""text"": ""${question}"",
                                                ""wrap"": ""true""
                                            },
                                            {
                                                ""type"": ""TextBlock"",
                                                ""weight"": ""Bolder"",
                                                ""spacing"": ""None"",
                                                ""text"": ""${answer}"",
                                                ""isSubtle"": ""false"",
                                                ""wrap"": ""true""
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

        private IMessageActivity CreateCard(AdaptiveCard card)
        {
            var reply = Activity.CreateMessageActivity();
            reply.Attachments.Add(new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            });
            return reply;
        }
        private IMessageActivity CreateCard(string card)
        {
            var reply = Activity.CreateMessageActivity();
            reply.Attachments.Add(new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject<AdaptiveCard>(card),
            });
            return reply;
        }
    }
}