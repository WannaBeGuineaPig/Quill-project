using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quill_API.Model;
using Quill_API.SupportClass;
using System.Buffers.Text;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quill_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {

        [HttpGet("GetFilteredArticles")]
        public ActionResult GetFilteredArticles(
                           [FromQuery] string search = null,
                            int category = -1,
                            string sortBy = "newest")
        {
            try
            {
                var query = DbArticlesContext.Context.Articles.Include(x => x.Author)
                    .Include(x => x.Comments)
                    .Include(x => x.IdTopicsNavigation)
                    .Include(x => x.Ratings)
                    .Where(x => x.Status == "active").AsQueryable();

                // Фильтр по поиску
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(x => x.Title.Contains(search) ||
                                           x.Content.Contains(search));
                }

                // Фильтр по категории
                if (category != -1)
                {
                    query = query.Where(x => x.IdTopics == category);
                }

                // Сортировка
                switch (sortBy.ToLower())
                {
                    case "oldest":
                        query = query.OrderBy(x => x.PublishedAt);
                        break;
                    case "popular":
                        query = query.OrderByDescending(x => x.Ratings.Count);
                        break;
                    case "title":
                        query = query.OrderBy(x => x.Title);
                        break;
                    case "newest":
                    default:
                        query = query.OrderByDescending(x => x.PublishedAt);
                        break;
                }

                //var totalCount = query.Count();
                //var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var articles = query
                   .Select(x => new
                   {
                       Id = x.Id,
                       Title = x.Title,
                       Content = x.Content,
                       TopicName = x.IdTopicsNavigation.NameTopics,
                       PublishedAt = x.PublishedAt,
                       AuthorName = x.Author.Nickname,
                       Image = x.Image,
                       Likes = x.LikesAmount.ToString(),
                       Dislikes = x.DislikesAmount.ToString(),
                       CommentsCount = x.Comments.Count(c => c.Status == "active")
                   })
                   .ToList();

                return Ok(articles);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка при фильтрации статей: " + ex.Message);
            }
        }

        [HttpGet("GetUsersArticles")]
        public ActionResult<List<Article>> GetUsersArticles(int userId)
        {
            try
            {
                var query = DbArticlesContext.Context.Articles.Include(x => x.Author)
                         .Include(x => x.Comments)
                         .Include(x => x.IdTopicsNavigation)
                         .Include(x => x.Ratings)
                         .Where(x => x.Status == "active" && x.AuthorId==userId).AsQueryable();

                var articles = query
                      .Select(x => new
                      {
                          Id = x.Id,
                          Title = x.Title,
                          Content = x.Content,
                          TopicName = x.IdTopicsNavigation.NameTopics,
                          PublishedAt = x.PublishedAt,
                          AuthorName = x.Author.Nickname,
                          Image = x.Image,
                          Likes = x.LikesAmount.ToString(),
                          Dislikes = x.DislikesAmount.ToString(),
                          CommentsCount = x.Comments.Count(c => c.Status == "active")
                      })
                      .ToList();
                return Ok(articles);
            }
            catch(Exception ex)
            {
                return BadRequest("Ошибка при выводе статей пользователя: " + ex.Message);
            }

        }

        [HttpGet("GetLastetsNews")]
        public List<Article> GetLastetsNews()
        {
            return DbArticlesContext.Context.Articles.ToList();
        }

        [HttpGet("GetOldNews")]
        public List<Article> GetOldNews()
        {
            return DbArticlesContext.Context.Articles.OrderByDescending(obj => obj.PublishedAt).ToList();
        }


        [HttpGet("GetNewsOnTitle/{titleInput}")]
        public List<Article> GetOldNews(string titleInput)
        {
            return DbArticlesContext.Context.Articles.Where(obj => Regex.IsMatch(obj.Title, titleInput)).ToList();
        }

        [HttpGet("GetArticleOnId")]
        public ActionResult GetArticleOnId(int id)
        {
            try
            {
                var article = DbArticlesContext.Context.Articles
                    .Where(x => x.Id == id && x.Status == "active")
                    .Select(x => new
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Content = x.Content,
                        TopicName = x.IdTopicsNavigation.NameTopics,
                        PublishedAt = x.PublishedAt,
                        AuthorName = x.Author.Nickname,
                        AuthorId = x.Author.Id,
                        Image = x.Image,
                        Likes = x.LikesAmount,
                        Dislikes = x.DislikesAmount
                    })
                    .FirstOrDefault();

                if (article == null)
                {
                    return NotFound("Статья не найдена");
                }

                return Ok(article);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка при получении статьи: " + ex.Message);
            }
        }

        [HttpPost("AddNewArticle")]
        public ActionResult AddNewArticle(ArticleClass article) 
        {
            if (article.Title.Trim().Length == 0)
            {
                return BadRequest("Введите название");
            }
            if (article.Content.Trim().Length == 0)
            {
                return BadRequest("Введите содержание");
            }
            if (!HelpFunc.CheckCorrectlyIdUser(DbArticlesContext.Context.Users, article.AuthorId))
                return NotFound("Пользователь не найден!");

            if (!HelpFunc.CheckUniqueTitle(DbArticlesContext.Context.Articles, article.Title, article.AuthorId))
                return BadRequest("Данный пользователь уже выкладывал статью с таким заголовком!");

            if (!HelpFunc.CheckCorrectlyIdTopic(DbArticlesContext.Context.Topics, article.IdTopics))
                return NotFound("Категория не найдена!");

            Article newArticle = new Article()
            {
                Title = article.Title,
                Content = article.Content,
                PublishedAt = DateTime.Now,
                Status = "active",
                AuthorId = article.AuthorId,
                IdTopics = article.IdTopics,
            };



            if (article.Image is not null)
            {
                try
                {
                    newArticle.Image = Convert.FromBase64String(article.Image);
                }
                catch (System.FormatException ex) 
                {
                    return BadRequest("Не корректный формат изображения!");
                }
            }

            DbArticlesContext.Context.Articles.Add(newArticle);
            DbArticlesContext.Context.SaveChanges();
            return Ok("Статья добавлена!");
        }

        [HttpPut("ChangeArticle")]
        public ActionResult ChangeArticle(ArticleClass article) 
        {
            Article? article1 = DbArticlesContext.Context.Articles.Where(obj => obj.Id == article.Id).FirstOrDefault();

            if (article1 == null)
                return NotFound("Статья не найдена!");

            if (article.Title.Trim().Length == 0)
            {
                return BadRequest("Введите название");
            }
            if (article.Content.Trim().Length == 0)
            {
                return BadRequest("Введите содержание");
            }
            if (DbArticlesContext.Context.Articles.Any(x => x.Id != article.Id && x.AuthorId == x.AuthorId && x.Title == article.Title && x.Status == "active"))
            {
                return BadRequest("Данный пользователь уже выкладывал статью с таким заголовком!");
            }

            if (!HelpFunc.CheckCorrectlyIdTopic(DbArticlesContext.Context.Topics, article.IdTopics))
                return NotFound("Категория не найдена!");

            article1.Title = article.Title;
            article1.Content = article.Content;
            article1.IdTopics = article.IdTopics;

            if (article.Image is not null)
            {
                try
                {
                    article1.Image = Convert.FromBase64String(article.Image);
                }
                catch (System.FormatException ex)
                {
                    return BadRequest("Не корректный формат изображения!");
                }
            }

            DbArticlesContext.Context.SaveChanges();
            return Ok("Данные статьи обновлны!");
        }

        [HttpPut("ChangeStatusArticle")]
        public ActionResult ChangeStatusArticle(StatusArticleClass statusArticleClass)
        {
            Article? article = DbArticlesContext.Context.Articles.Where(obj => obj.Id == statusArticleClass.Id).FirstOrDefault();

            if (article == null)
                return NotFound("Статья не найдена!");

            List<string> allStatus = new List<string>()
            {
                "active",
                "deleted",
            };

            if (!allStatus.Contains(statusArticleClass.Status))
                return NotFound("Статус статьи не найден!");

            article.Status = statusArticleClass.Status;
            DbArticlesContext.Context.SaveChanges();
            return Ok("Статус статьи успешно изменён!");
        }
       
    } 
  
}
