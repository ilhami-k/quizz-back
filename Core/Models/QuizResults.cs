namespace Core.Models
{
    public class QuizResult
    {
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}