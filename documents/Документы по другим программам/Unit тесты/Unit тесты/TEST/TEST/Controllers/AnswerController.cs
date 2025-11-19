using Microsoft.AspNetCore.Mvc;
using TEST.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public AnswerController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        [HttpGet("Question/{questionId}")]
        public ActionResult GetAnswersByQuestion(int questionId)
        {
            try
            {
                var answers = _context.QuestionAnswers
                    .Where(a => a.Id == questionId)
                    .ToList();
                return Ok(answers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult CreateAnswer(int questionId, string answerText, bool isCorrect)
        {
            try
            {
                var answer = new QuestionAnswer
                {
                    Id = questionId,
                    AnswerText = answerText,
                    IsCorrect = isCorrect
                };

                _context.QuestionAnswers.Add(answer);
                _context.SaveChanges();
                return Ok("Ответ создан");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateAnswer(int id, string? answerText = null, bool? isCorrect = null)
        {
            try
            {
                var answer = _context.QuestionAnswers.FirstOrDefault(a => a.Id == id);
                if (answer == null) return NotFound("Ответ не найден");

                if (!string.IsNullOrEmpty(answerText)) answer.AnswerText = answerText;
                if (isCorrect.HasValue) answer.IsCorrect = isCorrect.Value;

                _context.SaveChanges();
                return Ok("Ответ обновлен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAnswer(int id)
        {
            try
            {
                var answer = _context.QuestionAnswers.FirstOrDefault(a => a.Id == id);
                if (answer == null) return NotFound("Ответ не найден");

                _context.QuestionAnswers.Remove(answer);
                _context.SaveChanges();
                return Ok("Ответ удален");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}
