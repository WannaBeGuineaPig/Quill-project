using Microsoft.AspNetCore.Mvc;
using TEST.Model;
using System.Text.Json;
using TEST.Model.DTO;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public UserController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        // Модели для запросов
        public class RegisterModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronymic { get; set; }
        }

        public class LoginModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UpdateProfileModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronymic { get; set; }
        }

        public class ChangePasswordModel
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        // Регистрация пользователя
        [HttpPost("Register")]
        public ActionResult Register([FromBody] RegisterModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Данные не предоставлены");
                }

                if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) ||
                    string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                {
                    return BadRequest("Все обязательные поля должны быть заполнены");
                }

                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    return BadRequest("Email уже используется");
                }

                var user = new User
                {
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic ?? "",
                    RoleId = 3
                };

                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok(new { message = "Регистрация успешна" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Вход пользователя
        [HttpPost("Login")]
        public ActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Данные не предоставлены");
                }

                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null) return NotFound(new { message = "Пользователь не найден" });

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);
                if (!isPasswordValid) return BadRequest(new { message = "Неверный пароль" });

                // Возвращаем пользователя без пароля
                return Ok(new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Patronymic,
                    user.RoleId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Получить профиль
        [HttpGet("Profile/{userId}")]
        public ActionResult GetProfile(int userId)
        {
            try
            {
                var user = _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName, u.Patronymic, u.RoleId })
                    .FirstOrDefault();

                if (user == null) return NotFound(new { message = "Пользователь не найден" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        [HttpPut("Profile/{id}")]
        public async Task<ActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Пользователь не найден");

            
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Patronymic = dto.Patronymic; 

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // Сменить пароль
        [HttpPut("ChangePassword/{userId}")]
        public ActionResult ChangePassword(int userId, [FromBody] ChangePasswordModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Данные не предоставлены");
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return NotFound(new { message = "Пользователь не найден" });

                bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash);
                if (!isOldPasswordValid) return BadRequest(new { message = "Неверный текущий пароль" });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.SaveChanges();
                return Ok(new { message = "Пароль изменен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Получить мои результаты тестов
        [HttpGet("MyResults/{userId}")]
        public ActionResult GetMyResults(int userId)
        {
            try
            {
                var results = _context.UserTestResults
                    .Where(tr => tr.UserId == userId && tr.FinishedAt != null)
                    .Join(_context.Tests,
                          result => result.TestId,
                          test => test.Id,
                          (result, test) => new {
                              Id = result.Id,
                              TestId = result.TestId, // ← ДОБАВЬТЕ ЭТУ СТРОКУ
                              test.Title,
                              result.StartedAt,
                              result.FinishedAt,
                              result.ScoreAchieved,
                              result.MaxScore,
                              result.IsPassed
                          })
                    .OrderByDescending(r => r.FinishedAt)
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Получить доступные тесты
        [HttpGet("AvailableTests")]
        public ActionResult GetAvailableTests()
        {
            try
            {
                var tests = _context.Tests
                    .Where(t => t.IsActive == true)
                    .Join(_context.TestCategories,
                          test => test.CategoryId,
                          category => category.Id,
                          (test, category) => new {
                              test.Id,
                              test.Title,
                              test.Description,
                              category_name = category.Name,
                              test.PassingScore,
                              test.TimeLimitMinutes,
                              question_count = _context.Questions.Count(q => q.TestId == test.Id)
                          })
                    .ToList();

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Получить детали результата теста с ответами на вопросы по userId и testId
        [HttpGet("TestResultDetails/{userId}/{testId}")]
        public ActionResult GetTestResultDetails(int userId, int testId)
        {
            try
            {
                // Получаем ID результата теста по userId и testId
                var resultId = _context.UserTestResults
                    .Where(tr => tr.UserId == userId && tr.TestId == testId)
                    .OrderByDescending(tr => tr.FinishedAt) // Берем последний результат
                    .Select(tr => tr.Id)
                    .FirstOrDefault();

                if (resultId == 0)
                    return NotFound(new { message = "Результат теста не найден для данного пользователя" });

                // Получаем основную информацию о результате
                var resultInfo = _context.UserTestResults
                    .Where(tr => tr.Id == resultId)
                    .Join(_context.Tests,
                          result => result.TestId,
                          test => test.Id,
                          (result, test) => new {
                              ResultId = result.Id,
                              TestTitle = test.Title,
                              StartedAt = result.StartedAt,
                              FinishedAt = result.FinishedAt,
                              ScoreAchieved = result.ScoreAchieved,
                              MaxScore = result.MaxScore,
                              IsPassed = result.IsPassed,
                              PassingScore = test.PassingScore
                          })
                    .FirstOrDefault();

                if (resultInfo == null)
                    return NotFound(new { message = "Результат теста не найден" });

                // Получаем ответы пользователя на вопросы
                var userAnswers = _context.UserAnswers
                    .Where(ua => ua.TestResultId == resultId)
                    .Join(_context.Questions,
                          answer => answer.QuestionId,
                          question => question.Id,
                          (answer, question) => new {
                              QuestionText = question.QuestionText,
                              UserAnswer = answer.AnswerText,
                              IsCorrect = answer.IsCorrect,
                              PointsEarned = answer.PointsEarned,
                              MaxPoints = question.Points
                          })
                    .ToList();

                return Ok(new
                {
                    ResultInfo = resultInfo,
                    UserAnswers = userAnswers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Получить все мои результаты с возможностью посмотреть детали
        [HttpGet("MyResultsWithDetails/{userId}")]
        public ActionResult GetMyResultsWithDetails(int userId)
        {
            try
            {
                var results = _context.UserTestResults
                    .Where(tr => tr.UserId == userId && tr.FinishedAt != null)
                    .Join(_context.Tests,
                          result => result.TestId,
                          test => test.Id,
                          (result, test) => new {
                              ResultId = result.Id,
                              TestId = result.TestId, // ← ДОБАВЬТЕ ЭТУ СТРОКУ
                              test.Title,
                              StartedAt = result.StartedAt,
                              FinishedAt = result.FinishedAt,
                              ScoreAchieved = result.ScoreAchieved,
                              MaxScore = result.MaxScore,
                              IsPassed = result.IsPassed,
                              // Добавляем количество вопросов
                              QuestionCount = _context.Questions.Count(q => q.TestId == test.Id),
                              // Добавляем количество правильных ответов
                              CorrectAnswersCount = _context.UserAnswers
                                                         .Count(ua => ua.TestResultId == result.Id && ua.IsCorrect == true),
                              // Добавляем проходной балл
                              PassingScore = test.PassingScore // ← ДОБАВЬТЕ ЭТУ СТРОКУ
                          })
                    .OrderByDescending(r => r.FinishedAt)
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }

        // Получить ответы на конкретный вопрос в результате теста
        [HttpGet("QuestionAnswers/{resultId}/{questionId}")]
        public ActionResult GetQuestionAnswers(int resultId, int questionId)
        {
            try
            {
                // Ответ пользователя
                var userAnswer = _context.UserAnswers
                    .FirstOrDefault(ua => ua.TestResultId == resultId && ua.QuestionId == questionId);

                if (userAnswer == null) return NotFound(new { message = "Ответ на вопрос не найден" });

                // Правильные ответы на этот вопрос
                var correctAnswers = _context.QuestionAnswers
                    .Where(qa => qa.QuestionId == questionId && qa.IsCorrect == true)
                    .Select(qa => qa.AnswerText)
                    .ToList();

                // Информация о вопросе
                var questionInfo = _context.Questions
                    .Where(q => q.Id == questionId)
                    .Select(q => new {
                        q.QuestionText,
                        q.Points,
                        q.QuestionType
                    })
                    .FirstOrDefault();

                return Ok(new
                {
                    Question = questionInfo,
                    UserAnswer = userAnswer.AnswerText,
                    IsUserAnswerCorrect = userAnswer.IsCorrect,
                    PointsEarned = userAnswer.PointsEarned,
                    CorrectAnswers = correctAnswers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка: {ex.Message}" });
            }
        }
    }
}