namespace Core.Models
{
    public class QuizSubmission
    {
        public int QuizId { get; set; }
        public List<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
    }
}