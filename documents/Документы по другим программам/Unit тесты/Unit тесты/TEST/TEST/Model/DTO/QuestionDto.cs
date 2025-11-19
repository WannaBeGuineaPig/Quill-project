namespace TEST.Model.DTO
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string text { get; set; } = null!;     // ← изменили с QuestionText → text
        public string question_type { get; set; } = "single_choice"; // ← важно для open_answer
        public List<AnswerDto> answers { get; set; } = new(); // ← answers, а не Answers
    }

    public class AnswerDto
    {
        public string text { get; set; } = null!;      // ← изменили с AnswerText → text
        public bool isCorrect { get; set; }            // ← изменили с IsCorrect → isCorrect
    }
}
