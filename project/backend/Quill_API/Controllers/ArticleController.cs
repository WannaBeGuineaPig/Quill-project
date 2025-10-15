using Microsoft.AspNetCore.Mvc;
using Quill_API.Model;
using Quill_API.SupportClass;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quill_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        [HttpGet("GetLastetsNews")]
        public List<Article> GetLastetsNews()
        {
            return QuillBdContext.Context.Articles.OrderBy(obj => obj.PublishedAt).ToList();
        }

        [HttpGet("GetOldNews")]
        public List<Article> GetOldNews()
        {
            return QuillBdContext.Context.Articles.OrderByDescending(obj => obj.PublishedAt).ToList();
        }


        [HttpGet("GetNewsOnTitle/{titleInput}")]
        public List<Article> GetOldNews(string titleInput)
        {
            return QuillBdContext.Context.Articles.Where(obj => Regex.IsMatch(obj.Title, titleInput)).ToList();
        }

        [HttpPost("AddNewArticle")]
        public ActionResult AddNewArticle(ArticleClass article) 
        {

            if (!HelpFunc.CheckCorrectlyNickName(article.Title))
                return BadRequest("Не корректное название статьи!");

            if (!HelpFunc.CheckCorrectlyIdUser(QuillBdContext.Context.Users, article.AuthorId))
                return NotFound("Пользователь не найден!");

            if (!HelpFunc.CheckUniqueTitle(QuillBdContext.Context.Articles, article.Title, article.AuthorId))
                return BadRequest("Данный пользователь уже выкладывал статью с таким заголовком!");

            if (!HelpFunc.CheckCorrectlyIdTopic(QuillBdContext.Context.Topics, article.IdTopics))
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

            QuillBdContext.Context.Articles.Add(newArticle);
            QuillBdContext.Context.SaveChanges();
            return Ok("Статья добавлена!");
        }

        [HttpPut("ChangeArticle")]
        public ActionResult ChangeArticle(ArticleClass article) 
        {
            Article? article1 = QuillBdContext.Context.Articles.Where(obj => obj.Id == article.Id).FirstOrDefault();

            if (article1 == null)
                return NotFound("Статья не найдена!");

            if (!HelpFunc.CheckCorrectlyNickName(article.Title))
                return BadRequest("Не корректное название статьи!");

            if (!HelpFunc.CheckUniqueTitle(QuillBdContext.Context.Articles, article.Title, article.AuthorId))
                return BadRequest("Данный пользователь уже выкладывал статью с таким заголовком!");

            if (!HelpFunc.CheckCorrectlyIdTopic(QuillBdContext.Context.Topics, article.IdTopics))
                return NotFound("Категория не найдена!");

            article1.Title = article.Title;
            article1.Content = article.Content;
            article1.IdTopics = article.IdTopics;

            QuillBdContext.Context.SaveChanges();
            return Ok("Данные статьи обновлны!");
        }

        [HttpPut("ChangeStatusArticle")]
        public ActionResult ChangeStatusArticle(StatusArticleClass statusArticleClass)
        {
            Article? article = QuillBdContext.Context.Articles.Where(obj => obj.Id == statusArticleClass.Id).FirstOrDefault();

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
            QuillBdContext.Context.SaveChanges();
            return Ok("Статус статьи успешно изменён!");
        }
    }
}
