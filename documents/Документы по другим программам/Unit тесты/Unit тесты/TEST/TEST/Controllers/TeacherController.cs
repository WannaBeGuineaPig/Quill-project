using Microsoft.AspNetCore.Mvc;
using TEST.Model;
using Microsoft.EntityFrameworkCore;

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public TeacherController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        // === МЕТОДЫ ДЛЯ РАБОТЫ С КАТЕГОРИЯМИ ===

        // Создать категорию
        [HttpPost("CreateCategory")]
        public ActionResult CreateCategory([FromQuery] string name, [FromQuery] string description, [FromQuery] int teacherId)
        {
            try
            {
                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest("Название категории не может быть пустым");

                if (teacherId <= 0)
                    return BadRequest("Неверный ID учителя");

                // Проверяем, существует ли учитель
                var teacher = _context.Users.FirstOrDefault(u => u.Id == teacherId);
                if (teacher == null)
                    return NotFound($"Учитель с ID {teacherId} не найден");

                // Проверяем, нет ли уже категории с таким именем
                var existingCategory = _context.TestCategories
                    .FirstOrDefault(c => c.Name.ToLower() == name.ToLower() && c.IsActive == true);

                if (existingCategory != null)
                {
                    return Conflict($"Категория с названием '{name}' уже существует");
                }

                // Создаем новую категорию
                var category = new TestCategory
                {
                    Name = name.Trim(),
                    Description = description?.Trim() ?? "",
                    CreatedBy = teacherId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.TestCategories.Add(category);
                _context.SaveChanges();

                return Ok(new
                {
                    category_id = category.Id,
                    message = "Категория успешно создана"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания категории: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return StatusCode(500, $"Ошибка создания категории: {ex.Message}");
            }
        }

        // Получить категории (с фильтром по учителю)
        [HttpGet("Categories")]
        public ActionResult GetCategories([FromQuery] int? teacherId)
        {
            try
            {
                IQueryable<TestCategory> query = _context.TestCategories.Where(c => c.IsActive == true);

                // Если указан teacherId, фильтруем по создателю
                if (teacherId.HasValue && teacherId > 0)
                {
                    query = query.Where(c => c.CreatedBy == teacherId.Value);
                }

                var categories = query
                    .OrderBy(c => c.Name)
                    .Select(c => new {
                        id = c.Id,
                        name = c.Name,
                        description = c.Description,
                        teacherId = c.CreatedBy,
                        created_at = c.CreatedAt
                    })
                    .ToList();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Удалить категорию
        [HttpDelete("DeleteCategory/{categoryId}")]
        public ActionResult DeleteCategory(int categoryId, [FromQuery] int teacherId)
        {
            try
            {
                var category = _context.TestCategories
                    .FirstOrDefault(c => c.Id == categoryId && c.CreatedBy == teacherId && c.IsActive == true);

                if (category == null)
                    return NotFound($"Категория с ID {categoryId} не найдена или у вас нет прав для её удаления");

                // Проверяем, есть ли тесты в этой категории
                var testsInCategory = _context.Tests
                    .Any(t => t.CategoryId == categoryId && t.IsActive == true);

                if (testsInCategory)
                {
                    return BadRequest("Невозможно удалить категорию, так как в ней есть тесты. Сначала переместите или удалите тесты.");
                }

                // Мягкое удаление - устанавливаем IsActive = false
                category.IsActive = false;
                _context.TestCategories.Update(category);
                _context.SaveChanges();

                return Ok(new { message = "Категория успешно удалена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Обновить категорию
        [HttpPut("UpdateCategory/{categoryId}")]
        public ActionResult UpdateCategory(int categoryId, [FromBody] UpdateCategoryModel model)
        {
            try
            {
                var category = _context.TestCategories
                    .FirstOrDefault(c => c.Id == categoryId && c.CreatedBy == model.TeacherId && c.IsActive == true);

                if (category == null)
                    return NotFound($"Категория с ID {categoryId} не найдена");

                // Проверяем, нет ли другой категории с таким же именем
                var duplicateCategory = _context.TestCategories
                    .FirstOrDefault(c => c.Name.ToLower() == model.Name.ToLower()
                        && c.CreatedBy == model.TeacherId
                        && c.Id != categoryId
                        && c.IsActive == true);

                if (duplicateCategory != null)
                {
                    return Conflict($"Категория с названием '{model.Name}' уже существует");
                }

                category.Name = model.Name.Trim();
                category.Description = model.Description?.Trim() ?? "";
                category.CreatedAt = DateTime.Now;

                _context.TestCategories.Update(category);
                _context.SaveChanges();

                return Ok(new { message = "Категория успешно обновлена" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        public class UpdateCategoryModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int TeacherId { get; set; }
        }

        // === СУЩЕСТВУЮЩИЕ МЕТОДЫ (остаются без изменений) ===

        // Получить мои тесты (созданные учителем)
        [HttpGet("MyTests/{teacherId}")]
        public ActionResult GetMyTests(int teacherId)
        {
            try
            {
                var tests = _context.Tests
                    .Where(t => t.CreatedBy == teacherId && t.IsActive == true)
                    .Join(_context.TestCategories,
                          test => test.CategoryId,
                          category => category.Id,
                          (test, category) => new {
                              Id = test.Id,
                              Title = test.Title,
                              Description = test.Description,
                              category_name = category.Name,
                              PassingScore = test.PassingScore,
                              TimeLimitMinutes = test.TimeLimitMinutes,
                              question_count = _context.Questions.Count(q => q.TestId == test.Id),
                              total_attempts = _context.UserTestResults.Count(tr => tr.TestId == test.Id && tr.FinishedAt != null)
                          })
                    .ToList();

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Получить результаты по моим тестам
        [HttpGet("MyTestResults/{teacherId}")]
        public ActionResult GetMyTestResults(int teacherId)
        {
            try
            {
                var results = _context.UserTestResults
                    .Where(tr => tr.FinishedAt != null)
                    .Join(_context.Tests.Where(t => t.CreatedBy == teacherId),
                          result => result.TestId,
                          test => test.Id,
                          (result, test) => new { result, test })
                    .Join(_context.Users,
                          combined => combined.result.UserId,
                          user => user.Id,
                          (combined, user) => new {
                              result_id = combined.result.Id,
                              user_name = user.FirstName + " " + user.LastName + " " + user.Patronymic,
                              test_title = combined.test.Title,
                              test_id = combined.test.Id,
                              started_at = combined.result.StartedAt,
                              finished_at = combined.result.FinishedAt,
                              score_achieved = combined.result.ScoreAchieved,
                              max_score = combined.result.MaxScore,
                              is_passed = combined.result.IsPassed
                          })
                    .OrderByDescending(r => r.finished_at)
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Создать тест (для учителя)
        [HttpPost("CreateTest")]
        public ActionResult CreateTest(int categoryId, string title, string description, int teacherId, int passingScore, int timeTimeMinutes)
        {
            try
            {
                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(title))
                    return BadRequest("Название теста не может быть пустым");

                if (passingScore <= 0 || passingScore > 100)
                    return BadRequest("Проходной балл должен быть от 1 до 100");

                if (timeTimeMinutes <= 0)
                    return BadRequest("Время выполнения должно быть больше 0 минут");

                var test = new Test
                {
                    CategoryId = categoryId,
                    Title = title.Trim(),
                    Description = description?.Trim(),
                    CreatedBy = teacherId,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    PassingScore = passingScore,
                    TimeLimitMinutes = timeTimeMinutes
                };

                _context.Tests.Add(test);
                _context.SaveChanges();

                return Ok(new { test_id = test.Id, message = "Тест создан" });
            }
            catch (Exception ex)
            {
                // Логируем полную информацию об ошибке
                Console.WriteLine($"Ошибка создания теста: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, $"Ошибка создания теста: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        // Получить полные данные теста для редактирования
        [HttpGet("GetTest/{testId}")]
        public ActionResult GetTest(int testId, int teacherId)
        {
            try
            {
                var test = _context.Tests
                    .Include(t => t.Questions)
                        .ThenInclude(q => q.QuestionAnswers)
                    .FirstOrDefault(t => t.Id == testId && t.CreatedBy == teacherId);

                if (test == null) return NotFound("Тест не найден или у вас нет прав");

                var result = new
                {
                    id = test.Id,
                    title = test.Title,
                    description = test.Description,
                    categoryId = test.CategoryId,
                    passingScore = test.PassingScore,
                    timeLimitMinutes = test.TimeLimitMinutes,
                    questions = test.Questions.Select(q => new
                    {
                        id = q.Id,
                        text = q.QuestionText,
                        type = q.QuestionType,
                        points = q.Points,
                        options = q.QuestionAnswers.Select(a => new
                        {
                            id = a.Id,
                            text = a.AnswerText,
                            isCorrect = a.IsCorrect
                        }).ToList()
                    }).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Обновить тест
        [HttpPut("UpdateTest/{testId}")]
        public ActionResult UpdateTest(int testId, [FromBody] UpdateTestModel model)
        {
            try
            {
                var test = _context.Tests.FirstOrDefault(t => t.Id == testId && t.CreatedBy == model.TeacherId);
                if (test == null) return NotFound("Тест не найден или у вас нет прав");

                // Обновляем основные данные теста
                test.Title = model.Title;
                test.Description = model.Description;
                test.CategoryId = model.CategoryId;
                test.PassingScore = model.PassingScore;
                test.TimeLimitMinutes = model.TimeLimitMinutes;
                test.CreatedAt = DateTime.Now;

                _context.Tests.Update(test);
                _context.SaveChanges();

                return Ok(new { message = "Тест обновлен" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        public class UpdateTestModel
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public int CategoryId { get; set; }
            public int PassingScore { get; set; }
            public int TimeLimitMinutes { get; set; }
            public int TeacherId { get; set; }
        }

        [HttpGet("QuestionTypes")]
        public ActionResult GetQuestionTypes()
        {
            try
            {
                var types = new[]
                {
            new { value = "single", name = "Один вариант", dbValue = "single_choice" },
            new { value = "multiple", name = "Несколько вариантов", dbValue = "multiple_choice" }
        };

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Добавить вопрос к тесту
        [HttpPost("AddQuestion")]
        public ActionResult AddQuestion(int testId, string questionText, string questionType, int points, int teacherId)
        {
            try
            {
                // Проверка, принадлежит ли тест учителю
                var test = _context.Tests.FirstOrDefault(t => t.Id == testId && t.CreatedBy == teacherId);
                if (test == null) return BadRequest("Тест не найден или у вас нет прав");

                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(questionText))
                    return BadRequest("Текст вопроса не может быть пустым");

                if (points <= 0)
                    return BadRequest("Баллы за вопрос должны быть больше 0");

                // Преобразуем тип вопроса в допустимые значения БД
                string dbQuestionType = questionType?.ToLower() switch
                {
                    "single" => "single_choice",
                    "multiple" => "multiple_choice",
                    "text" => "open_answer",
                    "single_choice" => "single_choice",
                    "multiple_choice" => "multiple_choice",
                    _ => "single_choice" // значение по умолчанию
                };

                var question = new Question
                {
                    TestId = testId,
                    QuestionText = questionText.Trim(),
                    QuestionType = dbQuestionType,
                    Points = points,
                    CreatedBy = teacherId,
                    CreatedAt = DateTime.Now
                };

                _context.Questions.Add(question);
                _context.SaveChanges();

                return Ok(new { question_id = question.Id, message = "Вопрос добавлен" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления вопроса: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                return StatusCode(500, $"Ошибка добавления вопроса: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        // Добавить ответ к вопросу
        [HttpPost("AddAnswer")]
        public ActionResult AddAnswer(int questionId, string answerText, bool isCorrect)
        {
            try
            {
                var question = _context.Questions
                    .Include(q => q.Test)
                    .FirstOrDefault(q => q.Id == questionId);

                if (question == null) return NotFound("Вопрос не найден");

                // Валидация входных данных
                if (string.IsNullOrWhiteSpace(answerText))
                    return BadRequest("Текст ответа не может быть пустым");

                var answer = new QuestionAnswer
                {
                    QuestionId = questionId,
                    AnswerText = answerText.Trim(),
                    IsCorrect = isCorrect
                };

                _context.QuestionAnswers.Add(answer);
                _context.SaveChanges();

                return Ok(new { answer_id = answer.Id, message = "Ответ добавлен" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка добавления ответа: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

                return StatusCode(500, $"Ошибка добавления ответа: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        // Удалить тест (и связанное содержимое)
        [HttpDelete("DeleteTest/{testId}")]
        public ActionResult DeleteTest(int testId, int teacherId)
        {
            try
            {
                var test = _context.Tests
                    .Include(t => t.Questions)
                        .ThenInclude(q => q.QuestionAnswers)
                    .Include(t => t.UserTestResults)
                    .FirstOrDefault(t => t.Id == testId && t.CreatedBy == teacherId);

                if (test == null) return NotFound("Тест не найден или у вас нет прав");

                // Удаляем связанные данные через каскадное удаление или явно
                _context.QuestionAnswers.RemoveRange(test.Questions.SelectMany(q => q.QuestionAnswers));
                _context.Questions.RemoveRange(test.Questions);
                _context.UserTestResults.RemoveRange(test.UserTestResults);
                _context.Tests.Remove(test);

                _context.SaveChanges();

                return Ok(new { message = "Тест и связанное содержимое удалены" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Получить список студентов (простая версия)
        [HttpGet("Students")]
        public ActionResult GetStudents()
        {
            try
            {
                var students = _context.Users
                    .Where(u => u.RoleId == 3)
                    .Select(u => new
                    {
                        id = u.Id,
                        name = u.FirstName + " " + u.LastName + " " + u.Patronymic,
                        email = u.Email,
                        completedTests = _context.UserTestResults.Count(utr => utr.UserId == u.Id && utr.FinishedAt != null)
                    })
                    .ToList();

                // Рассчитываем средний балл отдельно для каждого студента
                var result = new List<object>();
                foreach (var student in students)
                {
                    var grades = _context.UserTestResults
                        .Where(utr => utr.UserId == student.id && utr.FinishedAt != null && utr.MaxScore > 0)
                        .ToList();

                    double averageGrade = 0;
                    if (grades.Any())
                    {
                        averageGrade = grades.Average(utr => (double)utr.ScoreAchieved * 100.0 / utr.MaxScore);
                    }

                    result.Add(new
                    {
                        student.id,
                        student.name,
                        student.email,
                        completedTests = student.completedTests,
                        averageGrade = Math.Round(averageGrade, 1),
                        avatar = "👨‍🎓"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}. Inner: {ex.InnerException?.Message}");
            }
        }

        // Получить результаты конкретного студента
        [HttpGet("StudentResults/{studentId}")]
        public ActionResult GetStudentResults(int studentId, int teacherId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == studentId);
                if (user == null) return NotFound("Студент не найден");

                var results = _context.UserTestResults
                    .Where(tr => tr.UserId == studentId && tr.FinishedAt != null)
                    .Join(_context.Tests.Where(t => t.CreatedBy == teacherId),
                          result => result.TestId,
                          test => test.Id,
                          (result, test) => new {
                              result_id = result.Id,
                              test_title = test.Title,
                              test_id = test.Id,
                              started_at = result.StartedAt,
                              finished_at = result.FinishedAt,
                              score_achieved = result.ScoreAchieved,
                              max_score = result.MaxScore,
                              is_passed = result.IsPassed,
                              percentage = result.MaxScore > 0 ? Math.Round((double)result.ScoreAchieved * 100 / result.MaxScore, 1) : 0
                          })
                    .OrderByDescending(r => r.finished_at)
                    .ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        // Получить данные преподавателя
        [HttpGet("TeacherInfo/{teacherId}")]
        public ActionResult GetTeacherInfo(int teacherId)
        {
            try
            {
                var teacher = _context.Users
                    .Where(u => u.Id == teacherId)
                    .Select(u => new
                    {
                        id = u.Id,
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        patronymic = u.Patronymic,
                        email = u.Email,
                        fullName = u.LastName + " " + u.FirstName + " " + u.Patronymic
                    })
                    .FirstOrDefault();

                if (teacher == null)
                    return NotFound("Преподаватель не найден");

                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}