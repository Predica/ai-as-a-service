using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;

namespace AwesomeBot
{
    public class ContextOrchestrator : IContextOrchestrator
    {
        private readonly LuisRecognizer _luisRecognizer;

        public ContextOrchestrator(LuisRecognizer luisRecognizer)
        {
            _luisRecognizer = luisRecognizer;
        }
        
        public async ValueTask<BotContext> Decide(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var luisResponse = await _luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
            var intent = luisResponse.Intents.FirstOrDefault();
            
            return intent.Key switch
            {
                "None" => BotContext.QnA,
                _ => BotContext.Echo
            };
        }
    }
}