using JPS_Bot.CustomPrompts;
using JPS_Bot.Models;
using JPS_Bot.Services.Interfaces;
using JPS_Bot.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPS_Bot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ConsultLotteryMsgDialog _consultLotteryMsgDialog;
        private readonly ConsultLotteryCardsDialog _consultLotteryCardsDialog;

        public MainDialog(
            ConsultLotteryMsgDialog consultLotteryMsgDialog, ConsultLotteryCardsDialog consultLotteryCardsDialog)
        {
            _consultLotteryMsgDialog = consultLotteryMsgDialog;
            _consultLotteryCardsDialog = consultLotteryCardsDialog;

            // The defaultLocale is only read if the Activity.Locale is not specified
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), defaultLocale: "es-es"));

            // Sets the WaterfallDialog that will be used as an Initial Dialog
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CheckIfTheUserWantsToConsult,
                SendIfCardsWillBeUsedMsg,
                CheckIfCardsWillBeUsed,
            }));

            AddDialog(consultLotteryMsgDialog);
            AddDialog(consultLotteryCardsDialog);

            // Sets which one is the Initial (Sub)Dialog.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> CheckIfTheUserWantsToConsult(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userMessage = stepContext.Context.Activity.Text;

            if(!string.IsNullOrEmpty(userMessage) &&
                (userMessage.ToLower().Contains("consultar") ||
                    userMessage.ToLower().Contains("sorteo")))
            {
                return await stepContext.NextAsync();
            }

            await stepContext.Context.SendActivityAsync(
                activity: JPSBotUtility.GetDefaultMessageActivity(),
                cancellationToken: cancellationToken
            );

            return await stepContext.EndDialogAsync();
        }

        private async Task<DialogTurnResult> SendIfCardsWillBeUsedMsg(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var confirmMessage = MessageFactory.Text("¿Desea seguir el proceso usando \"cards\"?");
            
            return await stepContext.PromptAsync(
                dialogId: nameof(ConfirmPrompt),
                options: new PromptOptions() { Prompt = confirmMessage });
        }

        private async Task<DialogTurnResult> CheckIfCardsWillBeUsed(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cardsWillBeUsed = (bool)stepContext.Result;

            if (cardsWillBeUsed)
            {
                return await stepContext.BeginDialogAsync(dialogId: _consultLotteryCardsDialog.Id);
            }
            else
            {
                return await stepContext.BeginDialogAsync(dialogId: _consultLotteryMsgDialog.Id);
            }
        }
    }
}
