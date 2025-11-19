using Microsoft.AspNetCore.Mvc;
using TEST.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TEST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ExamProctoringSuiteContext _context;

        public CategoryController()
        {
            _context = ExamProctoringSuiteContext.GetContext;
        }

        [HttpGet]
        public ActionResult GetCategories()
        {
            try
            {
                var categories = _context.TestCategories
                    .Where(c => c.IsActive == true)
                    .ToList();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult CreateCategory(string name, string description, int createdBy)
        {
            try
            {
                var category = new TestCategory
                {
                    Name = name,
                    Description = description,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.TestCategories.Add(category);
                _context.SaveChanges();
                return Ok("Категория создана");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCategory(int id, string? name = null, string? description = null)
        {
            try
            {
                var category = _context.TestCategories.FirstOrDefault(c => c.Id == id && c.IsActive == true);
                if (category == null) return NotFound("Категория не найдена");

                if (!string.IsNullOrEmpty(name)) category.Name = name;
                if (!string.IsNullOrEmpty(description)) category.Description = description;

                _context.SaveChanges();
                return Ok("Категория обновлена");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                var category = _context.TestCategories.FirstOrDefault(c => c.Id == id && c.IsActive == true);
                if (category == null) return NotFound("Категория не найдена");

                category.IsActive = false;
                _context.SaveChanges();
                return Ok("Категория удалена");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
    }
}
