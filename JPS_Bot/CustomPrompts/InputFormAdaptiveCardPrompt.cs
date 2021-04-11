using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JPS_Bot.CustomPrompts
{
    public class InputFormAdaptiveCardPrompt : Prompt<object>
    {
        public InputFormAdaptiveCardPrompt(
            string dialogId,
            PromptValidator<object> validator = null
        ) : base(dialogId: dialogId, validator: validator)
        {

        }

        protected override async Task OnPromptAsync(
            ITurnContext turnContext,
            IDictionary<string, object> state,
            PromptOptions options, bool isRetry,
            CancellationToken cancellationToken = default
        ) {
            if (turnContext == null)
            {
                throw new ArgumentException(nameof(turnContext));
            }

            if (options == null)
            {
                throw new ArgumentException(nameof(options));
            }

            if (isRetry && options.Prompt != null)
            {
                await turnContext.SendActivityAsync(options.RetryPrompt, cancellationToken).ConfigureAwait(false);
            }
            else if (options.Prompt != null)
            {
                await turnContext.SendActivityAsync(options.Prompt, cancellationToken).ConfigureAwait(false);
            }
        }

        protected override Task<PromptRecognizerResult<object>> OnRecognizeAsync(
            ITurnContext turnContext,
            IDictionary<string, object> state,
            PromptOptions options,
            CancellationToken cancellationToken = default
        ) {
            if (turnContext == null)
            {
                throw new ArgumentException(nameof(turnContext));
            }

            if (turnContext.Activity == null)
            {
                throw new ArgumentException(nameof(turnContext));
            }

            var result = new PromptRecognizerResult<object>();

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                if (turnContext.Activity.Value != null)
                {
                    if (turnContext.Activity.Value is object)
                    {
                        result.Value = turnContext.Activity.Value as object;
                        result.Succeeded = true;
                    }
                }
            }
            return Task.FromResult(result);
        }
    }
}
