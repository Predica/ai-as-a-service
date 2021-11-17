using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwesomeBot.Contracts;
using AwesomeBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace AwesomeBot
{
    public class Bot : ActivityHandler
    {
        private readonly IAdaptiveCardsTemplates _templates;
        private readonly IContextOrchestrator _contextOrchestrator;
        private readonly IQnAService _qnAService;
        public Bot(IAdaptiveCardsTemplates templates, IContextOrchestrator contextOrchestrator, IQnAService qnAService)
        {
            _templates = templates;
            _contextOrchestrator = contextOrchestrator;
            _qnAService = qnAService;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var context = await _contextOrchestrator.Decide(turnContext, cancellationToken);
            var activity = context switch
            {
                BotContext.Echo => _templates.EchoCard(
                    turnContext.Activity.Text),
                BotContext.QnA => _templates.QnACard(
                    await _qnAService.Ask(turnContext, cancellationToken).ToEnumerableAsync(cancellationToken)
                    ),
                _ => throw new ArgumentOutOfRangeException()
            };
            await turnContext.SendActivityAsync(activity, cancellationToken);
      
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
