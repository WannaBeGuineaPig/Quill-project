using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TEST.Model;
using TEST.Model.DTO;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public TestController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        [HttpGet]
        public ActionResult GetTests()
        {
            try
            {
                var tests = _context.Tests
                    .Where(t => t.IsActive == true)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.PassingScore,
                        t.TimeLimitMinutes,
                        QuestionCount = _context.Questions.Count(q => q.TestId == t.Id)
                    })
                    .ToList();

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("WithCategories")]
        public ActionResult GetTestsWithCategories()
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
                              test.CategoryId,
                              CategoryName = category.Name,
                              test.CreatedBy,
                              test.CreatedAt,
                              test.PassingScore,
                              test.TimeLimitMinutes,
                              QuestionCount = _context.Questions.Count(q => q.TestId == test.Id)
                          })
                    .ToList();

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetTest(int id)
        {
            try
            {
                var test = _context.Tests
                    .Where(t => t.Id == id && t.IsActive == true)
                    .Select(t => new TestDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        TimeLimitMinutes = t.TimeLimitMinutes,
                        PassingScore = t.PassingScore
                    })
                    .FirstOrDefault();

                if (test == null) return NotFound("Тест не найден");

                return Ok(test);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult CreateTest(int categoryId, string title, string description, int createdBy, int passingScore, int timeTimeMinutes)
        {
            try
            {
                var test = new Test
                {
                    CategoryId = categoryId,
                    Title = title,
                    Description = description,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    PassingScore = passingScore,
                    TimeLimitMinutes = timeTimeMinutes
                };

                _context.Tests.Add(test);
                _context.SaveChanges();
                return Ok("Тест создан");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateTest(int id, string? title = null, string? description = null, int? passingScore = null, int? timeTimeMinutes = null)
        {
            try
            {
                var test = _context.Tests.FirstOrDefault(t => t.Id == id && t.IsActive == true);
                if (test == null) return NotFound("Тест не найден");

                if (!string.IsNullOrEmpty(title)) test.Title = title;
                if (!string.IsNullOrEmpty(description)) test.Description = description;
                if (passingScore.HasValue) test.PassingScore = passingScore.Value;
                if (timeTimeMinutes.HasValue) test.TimeLimitMinutes = timeTimeMinutes.Value;

                _context.SaveChanges();
                return Ok("Тест обновлен");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteTest(int id)
        {
            try
            {
                var test = _context.Tests.FirstOrDefault(t => t.Id == id && t.IsActive == true);
                if (test == null) return NotFound("Тест не найден");

                test.IsActive = false;
                _context.SaveChanges();
                return Ok("Тест удален");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost("{testId}/favorite")]
        public ActionResult AddToFavorite(int testId, [FromQuery] int userId = 10)
        {
            try
            {
                Console.WriteLine($"Добавление в избранное: UserId={userId}, TestId={testId}");

                // Проверяем существование пользователя
                var userExists = _context.Users.Any(u => u.Id == userId);
                if (!userExists)
                {
                    Console.WriteLine($"Пользователь с ID {userId} не найден");
                    return NotFound($"Пользователь с ID {userId} не найден");
                }

                // Проверяем существование теста
                var testExists = _context.Tests.Any(t => t.Id == testId && t.IsActive == true);
                if (!testExists)
                {
                    Console.WriteLine($"Тест с ID {testId} не найден");
                    return NotFound($"Тест с ID {testId} не найден");
                }

                // Проверяем, не добавлен ли уже в избранное
                var existingFavorite = _context.FavoriteTests
                    .FirstOrDefault(f => f.UserId == userId && f.TestId == testId);

                if (existingFavorite != null)
                {
                    Console.WriteLine($"Тест {testId} уже в избранном пользователя {userId}");
                    return BadRequest("Уже в избранном");
                }

                var favorite = new FavoriteTest
                {
                    UserId = userId,
                    TestId = testId
                };

                Console.WriteLine($"Создаем новую запись избранного: UserId={favorite.UserId}, TestId={favorite.TestId}");

                _context.FavoriteTests.Add(favorite);
                _context.SaveChanges();

                Console.WriteLine("Успешно добавлено в избранное");
                return Ok("Добавлено в избранное");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                return StatusCode(500, $"Ошибка базы данных: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Общая ошибка: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpDelete("{testId}/favorite")]
        public ActionResult RemoveFromFavorite(int testId, [FromQuery] int userId = 10)
        {
            try
            {
                Console.WriteLine($"Удаление из избранного: UserId={userId}, TestId={testId}");

                var favorite = _context.FavoriteTests
                    .FirstOrDefault(f => f.UserId == userId && f.TestId == testId);

                if (favorite == null)
                {
                    Console.WriteLine($"Запись избранного не найдена: UserId={userId}, TestId={testId}");
                    return NotFound("Не найдено в избранном");
                }

                Console.WriteLine($"Найдена запись избранного: Id={favorite.Id}");

                _context.FavoriteTests.Remove(favorite);
                _context.SaveChanges();

                Console.WriteLine("Успешно удалено из избранного");
                return Ok("Удалено из избранного");
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException при удалении: {dbEx.Message}");
                Console.WriteLine($"Inner Exception: {dbEx.InnerException?.Message}");
                return StatusCode(500, $"Ошибка базы данных при удалении: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Общая ошибка при удалении: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("favorite")]
        public async Task<ActionResult> GetFavoriteTests([FromQuery] int userId = 10)
        {
            try
            {
                Console.WriteLine($"Загрузка избранного для пользователя: {userId}");

                var favoriteTestIds = await _context.FavoriteTests
                    .Where(f => f.UserId == userId)
                    .Select(f => f.TestId)
                    .ToListAsync();

                Console.WriteLine($"Найдено избранных тестов: {favoriteTestIds.Count}");

                var tests = await _context.Tests
                    .Where(t => favoriteTestIds.Contains(t.Id) && t.IsActive == true)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.TimeLimitMinutes,
                        t.PassingScore,
                        QuestionCount = _context.Questions.Count(q => q.TestId == t.Id) // ← ДОБАВЬТЕ ЭТУ СТРОКУ
                    })
                    .ToListAsync();

                Console.WriteLine($"Загружено тестов: {tests.Count}");
                return Ok(tests);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки избранного: {ex.Message}");
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}