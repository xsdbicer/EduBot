using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Dialogs
{
    public class CourseDialog : ComponentDialog
    {
        private readonly CourseApiClient _courseApiClient;

        public CourseDialog(CourseApiClient courseApiClient)
            : base(nameof(CourseDialog))
        {
            _courseApiClient = courseApiClient;

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowCoursesStepAsync,
                FinalStepAsync,
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowCoursesStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var courses = await _courseApiClient.GetCoursesAsync();

            if (courses != null && courses.Length > 0)
            {
                var message = "Here are the available courses:\n";
                foreach (var course in courses)
                {
                    message += $"- {course.Name}\n";
                }

                await stepContext.Context.SendActivityAsync(message, cancellationToken: cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("No courses available at the moment.", cancellationToken: cancellationToken);
            }

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
