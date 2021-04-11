using JPS_Bot.Models;
using JPS_Bot.Services.Interfaces;
using JPS_Bot.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPS_Bot.Dialogs
{
    public class ConsultLotteryMsgDialog : ComponentDialog
    {
        private const string DIALOG_PROCESS_WITH_MESSAGES = "DialogProcessMessages";

        private const string DIALOG_FRACTION_NUMBER = "DialogFractionNumber";
        private const string DIALOG_FRACTION_SERIE = "DialogFractionSerie";
        private const string DIALOG_LOTTERY_NUMBER = "DialogLotteryNumber";

        private readonly IStatePropertyAccessor<LotteryState> _statePropertyAccessor;
        private readonly IJPSService _JPSService;

        public ConsultLotteryMsgDialog(ConversationState conversationState, IJPSService JPSService)
        {
            _statePropertyAccessor = conversationState.CreateProperty<LotteryState>("LotteryState");
            _JPSService = JPSService;

            // Allow the NumberPrompts and set a validation function
            AddDialog(new NumberPrompt<int>(DIALOG_FRACTION_NUMBER, FractionNumberValidation));
            AddDialog(new NumberPrompt<int>(DIALOG_FRACTION_SERIE, FractionSerieValidation));
            AddDialog(new NumberPrompt<int>(DIALOG_LOTTERY_NUMBER, LotteryNumberValidation));

            AddDialog(new WaterfallDialog(DIALOG_PROCESS_WITH_MESSAGES, new WaterfallStep[]{
                SendInputFractionNumberMsgStepAsync,
                GetFractionNumberStepAsync,
                SendInputFractionSerieMsgStepAsync,
                GetFractionSerieStepAsync,
                SendInputLotteryNumberMsgStepAsync,
                GetLotteryNumberStepAsync,
                SendFinalResult
            }));

            // Sets which one is the Initial (Sub)Dialog.
            InitialDialogId = DIALOG_PROCESS_WITH_MESSAGES;
        }

        /// <summary>
        /// Sends a message requesting the Fraction Number and sets a prompt
        /// to expect a user input.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SendInputFractionNumberMsgStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestFractionNumberMsg = "¿Cuál es el número de la fracción?";
            var promptMessage = MessageFactory.Text(
                text: requestFractionNumberMsg,
                ssml: requestFractionNumberMsg,
                inputHint: InputHints.ExpectingInput
            );

            return await stepContext.PromptAsync(
                dialogId: DIALOG_FRACTION_NUMBER,
                options: new PromptOptions
                {
                    Prompt = promptMessage,
                },
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Validates if the Fraction Number is a number between 0 and 99.
        /// </summary>
        /// <param name="promptValidatorContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>If it is valid or not</returns>
        private async Task<bool> FractionNumberValidation(
            PromptValidatorContext<int> promptValidatorContext, CancellationToken cancellationToken)
        {
            var numberEntered = promptValidatorContext.Recognized.Value;

            if (!promptValidatorContext.Recognized.Succeeded || numberEntered < 0 || numberEntered > 99)
            {
                await promptValidatorContext.Context.SendActivityAsync(
                    activity: JPSBotUtility.GetEnterAValidFractionNumberMsgActivity(),
                    cancellationToken: cancellationToken
                );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the Fraction Number from the user response and save it with
        /// the accessor property.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetFractionNumberStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var lotteryState = await _statePropertyAccessor.GetAsync(
                turnContext: stepContext.Context,
                defaultValueFactory: () => new LotteryState(),
                cancellationToken: cancellationToken
            );

            // Get the Fraction Number from the user's response and assign to lotteryState
            lotteryState.FractionNumber = (int)stepContext.Result;

            // Save the object with the FractionNumber assigned
            await _statePropertyAccessor.SetAsync(
                turnContext: stepContext.Context,
                value: lotteryState,
                cancellationToken: cancellationToken
            );

            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Sends a message requesting the Fraction Serie and sets a prompt
        /// to expect a user input.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SendInputFractionSerieMsgStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestFractionSerieMsg = "¿Cuál es la serie de la fracción?";
            var promptMessage = MessageFactory.Text(
                text: requestFractionSerieMsg,
                ssml: requestFractionSerieMsg,
                inputHint: InputHints.ExpectingInput
            );

            return await stepContext.PromptAsync(
                dialogId: DIALOG_FRACTION_SERIE,
                options: new PromptOptions { Prompt = promptMessage },
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Validates if the Fraction Serie is a number between 0 and 999.
        /// </summary>
        /// <param name="promptValidatorContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>If it is valid or not</returns>
        private async Task<bool> FractionSerieValidation(
            PromptValidatorContext<int> promptValidatorContext, CancellationToken cancellationToken)
        {
            var serieEntered = promptValidatorContext.Recognized.Value;

            if (!promptValidatorContext.Recognized.Succeeded || serieEntered < 0 || serieEntered > 999)
            {
                await promptValidatorContext.Context.SendActivityAsync(
                    activity: JPSBotUtility.GetEnterAValidFractionSerieMsgActivity(),
                    cancellationToken: cancellationToken
                );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the Fraction Serie from the user response and save it with
        /// the accessor property.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetFractionSerieStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var lotteryState = await _statePropertyAccessor.GetAsync(
                turnContext: stepContext.Context,
                defaultValueFactory: () => new LotteryState(),
                cancellationToken: cancellationToken
            );

            // Get the Fraction Serie from the user's response and assign to lotteryState
            lotteryState.FractionSerie = (int)stepContext.Result;

            // Save the object with the FractionSerie assigned
            await _statePropertyAccessor.SetAsync(
                turnContext: stepContext.Context,
                value: lotteryState,
                cancellationToken: cancellationToken
            );

            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Sends a message requesting the Lottery Number and sets a prompt
        /// to expect a user input.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SendInputLotteryNumberMsgStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var requestLotteryNumberMsg = "¿Cuál es el número de sorteo?";
            var promptMessage = MessageFactory.Text(
                text: requestLotteryNumberMsg,
                ssml: requestLotteryNumberMsg,
                inputHint: InputHints.ExpectingInput
            );

            return await stepContext.PromptAsync(
                dialogId: DIALOG_LOTTERY_NUMBER,
                options: new PromptOptions { Prompt = promptMessage },
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Validates if the Lottery Number is a positive integer
        /// </summary>
        /// <param name="promptValidatorContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>If it is valid or not</returns>
        private async Task<bool> LotteryNumberValidation(
            PromptValidatorContext<int> promptValidatorContext, CancellationToken cancellationToken)
        {
            var lotteryNumberEntered = promptValidatorContext.Recognized.Value;

            if (!promptValidatorContext.Recognized.Succeeded || lotteryNumberEntered < 0)
            {
                await promptValidatorContext.Context.SendActivityAsync(
                    activity: JPSBotUtility.GetEnterAValidLotteryNumberMsgActivity(),
                    cancellationToken: cancellationToken
                );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the Lottery Number from the user response and save it with
        /// the accessor property.
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> GetLotteryNumberStepAsync(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var lotteryState = await _statePropertyAccessor.GetAsync(
                turnContext: stepContext.Context,
                defaultValueFactory: () => new LotteryState(),
                cancellationToken: cancellationToken
            );

            // Get the Lottery Number from the user's response and assign to lotteryState
            lotteryState.LotteryNumber = (int)stepContext.Result;

            // Save the object with the LotteryNumber assigned
            await _statePropertyAccessor.SetAsync(
                turnContext: stepContext.Context,
                value: lotteryState,
                cancellationToken: cancellationToken
            );

            return await stepContext.NextAsync();
        }

        /// <summary>
        /// Calculates the rewards obtained based on the lottery state that contains the
        /// data entered by the user. 
        /// </summary>
        /// <param name="stepContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DialogTurnResult> SendFinalResult(
            WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the lotteryState with all the assigned data
            var lotteryState = await _statePropertyAccessor.GetAsync(
                turnContext: stepContext.Context,
                defaultValueFactory: () => new LotteryState(),
                cancellationToken: cancellationToken
            );

            // Requests the calculation from the service
            var lotteryRewardInfo = await _JPSService.CalculateRewards(lotteryState);
            if (lotteryRewardInfo != null)
            {
                string resultMessage = $"Según los datos proporcionados: \n" +
                    $"\tNúmero: {lotteryState.FractionNumber:00}." +
                    $"\tSerie: {lotteryState.FractionSerie:000}." +
                    $"\tSorteo: {lotteryState.LotteryNumber}.\n";

                if (lotteryRewardInfo.AmountEarned > 0)
                {
                    var amount = lotteryRewardInfo.AmountEarned.ToString("#,#", CultureInfo.CurrentCulture);
                    resultMessage += $"¡Has ganado! {amount} Colones :)";
                }
                else
                {
                    resultMessage += $"No has ganado nada :( ...";
                }

                var promptResultMessage = MessageFactory.Text(
                    text: resultMessage,
                    ssml: resultMessage
                );
                await stepContext.Context.SendActivityAsync(
                    activity: promptResultMessage, cancellationToken: cancellationToken);
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
