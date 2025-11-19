using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TEST.Model;
using TEST.Model.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public QuestionController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }


        [HttpGet("test/{testId}")]
        public async Task<IActionResult> GetQuestionsByTestId(int testId)
        {
            var questions = await _context.Questions
                .Where(q => q.TestId == testId)
                .Include(q => q.QuestionAnswers)
                .ToListAsync();

            var result = questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                text = q.QuestionText, 
                question_type = q.QuestionType ?? "single_choice", 
                answers = q.QuestionAnswers.Select(a => new AnswerDto
                {
                    text = a.AnswerText, 
                    isCorrect = a.IsCorrect == true 
                }).ToList()
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public ActionResult CreateQuestion(int testId, string questionText, string positionType, int points, int createdBy)
        {
            try
            {
                var question = new Question
                {
                    TestId = testId,
                    QuestionText = questionText,
                    QuestionType = positionType,
                    Points = points,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now
                };

                _context.Questions.Add(question);
                _context.SaveChanges();
                return Ok("Вопрос создан");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateQuestion(int id, string? questionText = null, string? positionType = null, int? points = null)
        {
            try
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == id);
                if (question == null) return NotFound("Вопрос не найден");

                if (!string.IsNullOrEmpty(questionText)) question.QuestionText = questionText;
                if (!string.IsNullOrEmpty(positionType)) question.QuestionType = positionType;
                if (points.HasValue) question.Points = points.Value;

                _context.SaveChanges();
                return Ok("Вопрос обновлен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteQuestion(int id)
        {
            try
            {
                var question = _context.Questions.FirstOrDefault(q => q.Id == id);
                if (question == null) return NotFound("Вопрос не найден");

                _context.Questions.Remove(question);
                _context.SaveChanges();
                return Ok("Вопрос удален");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}
