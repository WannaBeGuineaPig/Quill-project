using Microsoft.AspNetCore.Mvc;
using TEST.Model;
using Microsoft.EntityFrameworkCore;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public AdminController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        public AdminController(ExamProctoringSuiteContext context)
        {
            _context = context ?? ExamProctoringSuiteContext.GetContext;
        }

        // Модели для запросов
        public class AddTeacherModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Patronymic { get; set; }
        }

        public class ChangeRoleModel
        {
            public int NewRoleId { get; set; }
        }

        // Получить всех пользователей
        [HttpGet("GetAllUsers")]
        public ActionResult GetAllUsers()
        {
            try
            {
                var users = _context.Users
                    .Include(u => u.Role)
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.Patronymic,
                        RoleId = u.RoleId,
                        RoleName = u.Role.Name
                    })
                    .ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении пользователей: {ex.Message}" });
            }
        }

        // Добавить преподавателя
        [HttpPost("AddTeacher")]
        public ActionResult AddTeacher([FromBody] AddTeacherModel model)
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
                    RoleId = 2 // Роль преподавателя
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { message = "Преподаватель успешно добавлен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при добавлении преподавателя: {ex.Message}" });
            }
        }

        // Изменить роль пользователя
        [HttpPut("ChangeUserRole/{userId}")]
        public ActionResult ChangeUserRole(int userId, [FromBody] ChangeRoleModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Данные не предоставлены");
                }

                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Проверяем существование роли
                var role = _context.Roles.FirstOrDefault(r => r.Id == model.NewRoleId);
                if (role == null)
                {
                    return BadRequest("Указанная роль не существует");
                }

                user.RoleId = model.NewRoleId;
                _context.SaveChanges();

                var roleName = model.NewRoleId == 2 ? "преподаватель" : "студент";
                return Ok(new { message = $"Роль пользователя изменена на: {roleName}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при изменении роли: {ex.Message}" });
            }
        }

        // Удалить пользователя
        [HttpDelete("DeleteUser/{userId}")]
        public ActionResult DeleteUser(int userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                // Проверяем, не является ли пользователь администратором
                if (user.RoleId == 1) // Предполагая, что 1 - это ID роли администратора
                {
                    return BadRequest("Нельзя удалить администратора");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                return Ok(new { message = "Пользователь успешно удален" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при удалении пользователя: {ex.Message}" });
            }
        }

        // Статистика платформы
        [HttpGet("PlatformStatistics")]
        public ActionResult GetPlatformStatistics()
        {
            try
            {
                var totalUsers = _context.Users.Count();
                var totalStudents = _context.Users.Count(u => u.RoleId == 3); // Предполагая, что 3 - студент
                var totalTeachers = _context.Users.Count(u => u.RoleId == 2); // Предполагая, что 2 - преподаватель
                var totalTests = _context.Tests.Count();
                var totalQuestions = _context.Questions.Count();
                var totalTestAttempts = _context.UserTestResults.Count();
                var totalPassedTests = _context.UserTestResults.Count(tr => tr.IsPassed == true);

                var overallPassRate = totalTestAttempts > 0
                    ? Math.Round((double)totalPassedTests / totalTestAttempts * 100, 1)
                    : 0;

                var stats = new
                {
                    total_users = totalUsers,
                    total_students = totalStudents,
                    total_teachers = totalTeachers,
                    total_tests = totalTests,
                    total_questions = totalQuestions,
                    total_test_attempts = totalTestAttempts,
                    total_passed_tests = totalPassedTests,
                    overall_pass_rate = overallPassRate
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении статистики: {ex.Message}" });
            }
        }

        // Статистика преподавателей
        [HttpGet("TeacherStatistics")]
        public ActionResult GetTeacherStatistics()
        {
            try
            {
                var teacherStats = _context.Users
                    .Where(u => u.RoleId == 2) // Преподаватели
                    .Select(teacher => new
                    {
                        teacher_id = teacher.Id,
                        teacher_name = $"{teacher.LastName} {teacher.FirstName} {teacher.Patronymic}".Trim(),
                        email = teacher.Email,
                        created_tests = _context.Tests.Count(t => t.CreatedBy == teacher.Id),
                        total_questions_created = _context.Questions.Count(q => q.CreatedBy == teacher.Id),
                        average_test_pass_rate = _context.Tests
                            .Where(t => t.CreatedBy == teacher.Id)
                            .SelectMany(t => t.UserTestResults)
                            .Where(tr => tr.FinishedAt != null)
                            .Average(tr => (double?)tr.ScoreAchieved / tr.MaxScore * 100) ?? 0
                    })
                    .Where(t => t.created_tests > 0) // Только преподаватели с созданными тестами
                    .OrderByDescending(t => t.created_tests)
                    .ToList();

                return Ok(teacherStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении статистики преподавателей: {ex.Message}" });
            }
        }

        // Топ студентов
        [HttpGet("TopStudents")]
        public ActionResult GetTopStudents([FromQuery] int topCount = 5)
        {
            try
            {
                var topStudents = _context.Users
                    .Where(u => u.RoleId == 3) // Студенты
                    .Select(student => new
                    {
                        user_id = student.Id,
                        user_name = $"{student.LastName} {student.FirstName} {student.Patronymic}".Trim(),
                        total_tests_passed = _context.UserTestResults
                            .Count(tr => tr.UserId == student.Id && tr.IsPassed == true),
                        total_score = _context.UserTestResults
                            .Where(tr => tr.UserId == student.Id && tr.FinishedAt != null)
                            .Sum(tr => (int?)tr.ScoreAchieved) ?? 0,
                        average_score = _context.UserTestResults
                            .Where(tr => tr.UserId == student.Id && tr.FinishedAt != null && tr.MaxScore > 0)
                            .Average(tr => (double?)tr.ScoreAchieved / tr.MaxScore * 10) ?? 0 // Оценка по 10-балльной шкале
                    })
                    .Where(s => s.total_tests_passed > 0)
                    .OrderByDescending(s => s.average_score)
                    .ThenByDescending(s => s.total_tests_passed)
                    .Take(topCount)
                    .ToList();

                return Ok(topStudents);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении топа студентов: {ex.Message}" });
            }
        }

        // Статистика по категориям
        [HttpGet("CategoryStatistics")]
        public ActionResult GetCategoryStatistics()
        {
            try
            {
                var categoryStats = _context.TestCategories
                    .Select(category => new
                    {
                        category_id = category.Id,
                        category_name = category.Name,
                        total_tests = _context.Tests.Count(t => t.CategoryId == category.Id),
                        total_attempts = _context.UserTestResults
                            .Count(tr => tr.Test.CategoryId == category.Id),
                        average_pass_rate = _context.UserTestResults
                            .Where(tr => tr.Test.CategoryId == category.Id && tr.FinishedAt != null && tr.MaxScore > 0)
                            .Average(tr => (double?)tr.ScoreAchieved / tr.MaxScore * 100) ?? 0
                    })
                    .Where(c => c.total_tests > 0)
                    .OrderByDescending(c => c.total_attempts)
                    .ToList();

                return Ok(categoryStats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении статистики категорий: {ex.Message}" });
            }
        }

        // Получить все тесты с детальной информацией
        [HttpGet("AllTests")]
        public ActionResult GetAllTests()
        {
            try
            {
                var tests = _context.Tests
                    .Include(t => t.Category)
                    .Include(t => t.CreatedByNavigation)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        Category = t.Category.Name,
                        CreatedBy = $"{t.CreatedByNavigation.LastName} {t.CreatedByNavigation.FirstName}",
                        t.PassingScore,
                        t.TimeLimitMinutes,
                        QuestionCount = _context.Questions.Count(q => q.TestId == t.Id),
                        TotalAttempts = _context.UserTestResults.Count(tr => tr.TestId == t.Id),
                        AverageScore = _context.UserTestResults
                            .Where(tr => tr.TestId == t.Id && tr.FinishedAt != null && tr.MaxScore > 0)
                            .Average(tr => (double?)tr.ScoreAchieved / tr.MaxScore * 100) ?? 0,
                        t.IsActive,
                        t.CreatedAt
                    })
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении тестов: {ex.Message}" });
            }
        }

        // Получить детальную статистику по тесту
        [HttpGet("TestStatistics/{testId}")]
        public ActionResult GetTestStatistics(int testId)
        {
            try
            {
                var test = _context.Tests
                    .Include(t => t.Category)
                    .FirstOrDefault(t => t.Id == testId);

                if (test == null)
                {
                    return NotFound(new { message = "Тест не найден" });
                }

                // Получаем завершенные попытки
                var completedAttempts = _context.UserTestResults
                    .Where(tr => tr.TestId == testId && tr.FinishedAt != null && tr.StartedAt != null)
                    .ToList();

                var statistics = new
                {
                    TestInfo = new
                    {
                        test.Id,
                        test.Title,
                        Category = test.Category.Name,
                        test.PassingScore,
                        test.TimeLimitMinutes
                    },
                    Attempts = _context.UserTestResults
                        .Count(tr => tr.TestId == testId),
                    CompletedAttempts = completedAttempts.Count,
                    PassedAttempts = _context.UserTestResults
                        .Count(tr => tr.TestId == testId && tr.IsPassed == true),
                    AverageScore = completedAttempts.Any()
                        ? completedAttempts.Average(tr => (double)tr.ScoreAchieved / tr.MaxScore * 100)
                        : 0,
                    AverageCompletionTime = completedAttempts.Any()
                        ? completedAttempts.Average(tr => (tr.FinishedAt - tr.StartedAt).Value.TotalMinutes)
                        : 0
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ошибка при получении статистики теста: {ex.Message}" });
            }
        }
    }
}