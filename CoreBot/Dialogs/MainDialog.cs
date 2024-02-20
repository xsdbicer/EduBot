using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly ILogger<MainDialog> _logger;

        public MainDialog(ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));

            var waterfallSteps = new WaterfallStep[]
            {
                IntroStepAsync,
                FinalStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));

            // CourseDialog'u ekleyin
            AddDialog(new CourseDialog(new CourseApiClient(new HttpClient())));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\nSay something like 'Book a course'";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Result?.ToString()?.ToLowerInvariant();

            if (!string.IsNullOrEmpty(userInput) && userInput.Contains("book a course"))
            {
                var messageText = "Course booking is in progress...";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.BeginDialogAsync(nameof(CourseDialog), null, cancellationToken);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }
            else
            {
                var messageText = "Sorry, I couldn't understand your request.";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
