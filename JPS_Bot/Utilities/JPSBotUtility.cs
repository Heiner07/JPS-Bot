using AdaptiveCards.Templating;
using JPS_Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JPS_Bot.Utilities
{
    public class JPSBotUtility
    {
        private const string CARDS_FOLDER = "CardTemplates";
        public static Activity GetDefaultMessageActivity()
        {
            return (Activity)MessageFactory.Attachment(attachment: GenerateWelcomeCard());
        }

        public static Activity GetEnterAValidFractionNumberMsgActivity()
        {
            const string ERROR_MESSAGE = "Proporcione un número correcto. " +
                "El rango permitido es [0 - 99].";
            return MessageFactory.Text(
                text: ERROR_MESSAGE,
                ssml: ERROR_MESSAGE
            );
        }

        public static Activity GetEnterAValidFractionSerieMsgActivity()
        {
            const string ERROR_MESSAGE = "Proporcione una serie correcta. " +
                "El rango permitido es [0 - 999].";
            return MessageFactory.Text(
                text: ERROR_MESSAGE,
                ssml: ERROR_MESSAGE
            );
        }

        public static Activity GetEnterAValidLotteryNumberMsgActivity()
        {
            const string ERROR_MESSAGE = "Proporcione una número de lotería válido. " +
                "Debe ser entero positivo.";
            return MessageFactory.Text(
                text: ERROR_MESSAGE,
                ssml: ERROR_MESSAGE
            );
        }

        public static Attachment GenerateWelcomeCard()
        {
            string welcomeCardPath = Path.Combine(".", CARDS_FOLDER, "WelcomeCard.json");

            var adaptiveCardJson = File.ReadAllText(welcomeCardPath);
            var template = new AdaptiveCardTemplate(adaptiveCardJson);

            var rootData = new
            {
                title = "Saludos",
                description = "Saludos, puedo ayudarte a saber si ganaste premios en un sorteo " +
                "de la lotería nacional. Solamente escríbeme un mensaje con una de las" +
                " siguientes palabras: \"consultar\" o \"sorteo\". O presiona el botón inferior."
            };

            var cardJson = template.Expand(rootData);

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };
        }

        public static Attachment GenerateInputLotteryDataCard()
        {
            string inputLotteryDataCardPath = Path.Combine(".", CARDS_FOLDER, "InputLotteryDataCard.json");

            var adaptiveCardJson = File.ReadAllText(inputLotteryDataCardPath);
            var template = new AdaptiveCardTemplate(adaptiveCardJson);

            var rootData = new
            {
                fractionNumberMsg = "Ingrese el número de la fracción",
                fractionSerieMsg = "Ingrese la serie de la fracción",
                lotteryNumberMsg = "Ingrese el número de sorteo",
                title = "Ingrese los datos de la fracción por consultar",
                imageUrl = "https://www.jps.go.cr/sites/all/themes/jps/images/logo_loteria.jpg",
            };

            var cardJson = template.Expand(rootData);

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };
        }

        public static Attachment GenerateDataResumeCard(LotteryState lotteryState)
        {
            string dataResumeCardPath = Path.Combine(".", CARDS_FOLDER, "DataResumeCard.json");

            var adaptiveCardJson = File.ReadAllText(dataResumeCardPath);
            var template = new AdaptiveCardTemplate(adaptiveCardJson);

            var rootData = new
            {
                title = "Según los datos proporcionados",
                labelNumber = "Número:",
                lotteryNumber = lotteryState.LotteryNumber.ToString(),
                number = lotteryState.FractionNumber.ToString(),
                serie = lotteryState.FractionSerie.ToString()
            };

            var cardJson = template.Expand(rootData);

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };
        }

        public static Attachment GenerateAmountEarnedCard(string title, string amountEarned)
        {
            string amountEarnedCardPath = Path.Combine(".", CARDS_FOLDER, "AmountEarnedCard.json");

            var adaptiveCardJson = File.ReadAllText(amountEarnedCardPath);
            var template = new AdaptiveCardTemplate(adaptiveCardJson);

            var rootData = new
            {
                title,
                amountEarned,
            };

            var cardJson = template.Expand(rootData);

            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };
        }
    }
}
