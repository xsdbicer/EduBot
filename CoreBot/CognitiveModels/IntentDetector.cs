namespace CoreBot.CognitiveModels
{
    public class IntentDetector
    {
        public static UserIntent DetectIntent(string userInput)
        {
            if (!string.IsNullOrEmpty(userInput))
            {
                if (userInput.Contains("book a course"))
                {
                    return UserIntent.BookCourse;
                }
                else if (userInput.Contains("cancel a course"))
                {
                    return UserIntent.CancelCourse;
                }
                else if (userInput.Contains("get course details"))
                {
                    return UserIntent.GetCourseDetails;
                }
            }

            return UserIntent.None; 
        }
    }
}
