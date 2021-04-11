using JPS_Bot.CustomPrompts;
using JPS_Bot.Models;
using JPS_Bot.Services.Interfaces;
using JPS_Bot.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
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
    public class ConsultLotteryCardsDialog : ComponentDialog
    {
        private const string DIALOG_PROCESS_WITH_CARDS = "DialogProcessCards";

        //private readonly IStatePropertyAccessor<LotteryState> _statePropertyAccessor;
        private readonly IJPSService _JPSService;

        public ConsultLotteryCardsDialog(IJPSService JPSService)
        {
            _JPSService = JPSService;

            AddDialog(new WaterfallDialog(DIALOG_PROCESS_WITH_CARDS, new WaterfallStep[]
            {
                SendInputLotteryDataCard,
                GetResultsForLotteryAndSendResultsCards
            }));
            AddDialog(new InputFormAdaptiveCardPrompt(nameof(InputFormAdaptiveCardPrompt)));

            // Sets which one is the Initial (Sub)Dialog.
            InitialDialogId = DIALOG_PROCESS_WITH_CARDS;
        }

        /// <summary>
        /// Sends the card where the user will introduce the necessary data to do the consult.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SendInputLotteryDataCard(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cardActivity = (Activity)MessageFactory.Attachment(
                attachment: JPSBotUtility.GenerateInputLotteryDataCard());

            return await stepContext.PromptAsync(
                dialogId: nameof(InputFormAdaptiveCardPrompt),
                options: new PromptOptions() { Prompt = cardActivity });
        }

        /// <summary>
        /// Gets the result of the card, calculates and returns the result of the consult in a card
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetResultsForLotteryAndSendResultsCards(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var result = stepContext.Result;
            var lotteryState = JsonConvert.DeserializeObject<LotteryState>(result.ToString());

            // Requests the calculation from the service
            var lotteryRewardInfo = await _JPSService.CalculateRewards(lotteryState);
            if (lotteryRewardInfo != null)
            {
                var resume = (Activity)MessageFactory.Attachment(
                    attachment: JPSBotUtility.GenerateDataResumeCard(lotteryState)
                );

                await stepContext.Context.SendActivityAsync(
                    activity: resume, cancellationToken: cancellationToken);

                Activity amountEarnedCard;

                if (lotteryRewardInfo.AmountEarned > 0)
                {
                    amountEarnedCard = (Activity)MessageFactory.Attachment(
                        attachment: JPSBotUtility.GenerateAmountEarnedCard(
                            title: "¡Felicidades!",
                            amountEarned: "₡" + lotteryRewardInfo.AmountEarned.ToString("#,#", CultureInfo.CurrentCulture)
                        )
                    );
                }
                else
                {
                    amountEarnedCard = (Activity)MessageFactory.Attachment(
                        attachment: JPSBotUtility.GenerateAmountEarnedCard(
                            title: "No hay premios",
                            amountEarned: "Nada :("
                        )
                    );
                }

                await stepContext.Context.SendActivityAsync(
                    activity: amountEarnedCard, cancellationToken: cancellationToken);
            }
            else
            {
                var resultMessage = "Ocurrió un error consultando la información del sorteo. " +
                    "Inténtelo nuevamente.";
                var promptResultMessage = MessageFactory.Text(
                    text: resultMessage,
                    ssml: resultMessage
                );
                await stepContext.Context.SendActivityAsync(
                    activity: promptResultMessage, cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
