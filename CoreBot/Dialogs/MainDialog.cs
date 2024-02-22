using CoreBot.CognitiveModels;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
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
            AddDialog(new CourseDetailsDialog(new CourseApiClient(new HttpClient())));


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
            UserIntent intent = IntentDetector.DetectIntent(userInput);

            switch (intent)
            {
                case UserIntent.BookCourse:
                    var messageText = "Course booking is in progress...";
                    var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(message, cancellationToken);
                    await stepContext.BeginDialogAsync(nameof(CourseDialog), null, cancellationToken);
                    break;
                case UserIntent.CancelCourse:
                    Console.WriteLine("User wants to cancel a course");
                    // İlgili işlemleri gerçekleştirin
                    break;
                case UserIntent.GetCourseDetails:
                    var info = "Course detail booking is in progress...";
                    var text = MessageFactory.Text(info, info, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(text, cancellationToken);
                    await stepContext.BeginDialogAsync(nameof(CourseDetailsDialog), null, cancellationToken);
                    break;
                case UserIntent.None:
                    Console.WriteLine("Intent not recognized");
                    break;
                default:
                    Console.WriteLine("Unknown intent");
                    break;
            }


            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(nameof(MainDialog), promptMessage, cancellationToken);
        }
    }
}
