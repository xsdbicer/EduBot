using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Threading;

namespace CoreBot.Dialogs
{
    public class CourseDetailsDialog:ComponentDialog
    {
        private readonly CourseApiClient _courseApiClient;

        public CourseDetailsDialog(CourseApiClient courseApiClient)
            : base(nameof(CourseDetailsDialog))
        {
            _courseApiClient = courseApiClient;

           
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
        {
            ShowCoursesDetailsStepAsync,
            FinalStepAsync,
        }));
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> ShowCoursesDetailsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var courses = await _courseApiClient.GetCoursesAsync();

            if (courses != null && courses.Length > 0)
            {
                var message = "Here are the available courses:\n";
                foreach (var course in courses)
                {
                    message += $"- {course.Name}\n {course.Description}";
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
