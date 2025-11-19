namespace TEST.Model.DTO
{
    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } 
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
    }
}
