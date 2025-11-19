using Microsoft.AspNetCore.Mvc;
using TEST.Model;
using TEST.Model.DTO;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestResultController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public TestResultController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        [HttpPost("Start")]
        public ActionResult StartTest([FromBody] StartTestRequest request)
        {
            try
            {
                // Получаем максимальный балл для теста
                var maxScore = _context.Questions
                    .Where(q => q.TestId == request.TestId)
                    .Sum(q => q.Points);

                var testResult = new UserTestResult
                {
                    UserId = request.UserId,
                    TestId = request.TestId,
                    StartedAt = DateTime.Now,
                    FinishedAt = null,
                    ScoreAchieved = 0,
                    MaxScore = maxScore,
                    IsPassed = false
                };

                _context.UserTestResults.Add(testResult);
                _context.SaveChanges();

                return Ok(new { TestResultId = testResult.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("Question/test/{testId}")]
        public ActionResult GetQuestionsByTest(int testId)
        {
            try
            {
                var questions = _context.Questions
                    .Where(q => q.TestId == testId)
                    .Select(q => new
                    {
                        id = q.Id,
                        text = q.QuestionText,
                        question_type = q.QuestionType,
                        points = q.Points,
                        answers = _context.QuestionAnswers
                            .Where(a => a.QuestionId == q.Id)
                            .Select(a => new
                            {
                                id = a.Id,
                                text = a.AnswerText,
                                isCorrect = a.IsCorrect
                            }).ToList()
                    })
                    .ToList();

                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost("SubmitWithAnswers")]
        public ActionResult SubmitTestWithAnswers([FromBody] SubmitTestRequest request)
        {
            try
            {
                var testResult = _context.UserTestResults.FirstOrDefault(tr => tr.Id == request.TestResultId);
                if (testResult == null) return NotFound("Результат теста не найден");

                var test = _context.Tests.FirstOrDefault(t => t.Id == testResult.TestId);
                if (test == null) return NotFound("Тест не найден");

                // Рассчитываем процент правильных ответов
                double percentage = (request.ScoreAchieved / (double)testResult.MaxScore) * 100;

                // Обновляем результат теста
                testResult.FinishedAt = DateTime.Now;
                testResult.ScoreAchieved = request.ScoreAchieved;
                testResult.IsPassed = percentage >= test.PassingScore;

                // Сохраняем ответы пользователя
                foreach (var answer in request.UserAnswers)
                {
                    var userAnswer = new UserAnswer
                    {
                        TestResultId = request.TestResultId,
                        QuestionId = answer.QuestionId,
                        AnswerText = answer.AnswerText,
                        IsCorrect = answer.IsCorrect,
                        PointsEarned = answer.PointsEarned
                    };

                    _context.UserAnswers.Add(userAnswer);
                }

                _context.SaveChanges();

                return Ok(new
                {
                    IsPassed = testResult.IsPassed,
                    Score = request.ScoreAchieved,
                    MaxScore = testResult.MaxScore,
                    Percentage = Math.Round(percentage, 2),
                    PassingScore = test.PassingScore
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("User/{userId}")]
        public ActionResult GetUserResults(int userId)
        {
            try
            {
                var results = _context.UserTestResults
                    .Where(tr => tr.UserId == userId && tr.FinishedAt != null)
                    .Select(tr => new
                    {
                        tr.Id,
                        tr.TestId,
                        tr.StartedAt,
                        tr.FinishedAt,
                        tr.ScoreAchieved,
                        tr.MaxScore,
                        tr.IsPassed,
                        TestTitle = _context.Tests.FirstOrDefault(t => t.Id == tr.TestId).Title
                    })
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetResult(int id)
        {
            try
            {
                var result = _context.UserTestResults.FirstOrDefault(tr => tr.Id == id);
                if (result == null) return NotFound("Результат не найден");

                var test = _context.Tests.FirstOrDefault(t => t.Id == result.TestId);
                return Ok(new
                {
                    result.Id,
                    result.TestId,
                    result.StartedAt,
                    result.FinishedAt,
                    result.ScoreAchieved,
                    result.MaxScore,
                    result.IsPassed,
                    TestTitle = test?.Title,
                    PassingScore = test?.PassingScore
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }

    // DTO классы
    public class StartTestRequest
    {
        public int UserId { get; set; }
        public int TestId { get; set; }
    }

    public class SubmitTestRequest
    {
        public int TestResultId { get; set; }
        public int ScoreAchieved { get; set; }
        public List<UserAnswerDto> UserAnswers { get; set; } = new List<UserAnswerDto>();
    }

    public class UserAnswerDto
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }
    }
}