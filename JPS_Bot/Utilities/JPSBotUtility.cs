using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JPS_Bot.Utilities
{
    public class JPSBotUtility
    {
        public static Activity GetDefaultMessageActivity()
        {
            var message = "Saludos, puedo ayudarte a saber si ganaste premios en un sorteo " +
                "de la lotería nacional. Solamente escríbeme un mensaje con una de las" +
                " siguientes palabras: \"consultar\" o \"sorteo\".";
            return MessageFactory.Text(text: message, ssml: message);
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
    }
}
