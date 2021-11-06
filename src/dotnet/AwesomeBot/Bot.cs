using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Contracts;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace AwesomeBot
{
    public class Bot : ActivityHandler
    {
        private readonly IAdaptiveCardsTemplates _templates;

        public Bot(IAdaptiveCardsTemplates templates)
        {
            _templates = templates;
        }
        protected override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            return turnContext.SendActivityAsync(
                _templates.EchoCard(turnContext.Activity.Text), cancellationToken: cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                }
            }
        }
    }
}
