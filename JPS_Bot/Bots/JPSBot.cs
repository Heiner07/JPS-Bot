// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using JPS_Bot.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JPS_Bot.Bots
{
    public class JPSBot<T> : ActivityHandler where T : Dialog
    {
        private readonly T _dialog;
        private readonly ConversationState _conversationState;

        public JPSBot(T dialog, ConversationState conversationState)
        {
            _dialog = dialog;
            _conversationState = conversationState;
        }

        protected override async Task OnMembersAddedAsync(
            IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken
        )
        {
            await turnContext.SendActivityAsync(
                activity: JPSBotUtility.GetDefaultMessageActivity(),
                cancellationToken: cancellationToken
            );
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(
                turnContext: turnContext,
                force: false,
                cancellationToken: cancellationToken
            );
            //await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Run the Dialog with the new message Activity.
            await _dialog.RunAsync(
                turnContext: turnContext,
                accessor: _conversationState.CreateProperty<DialogState>("DialogState"),
                cancellationToken: cancellationToken
            );
        }
    }
}
