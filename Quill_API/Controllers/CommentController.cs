using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quill_API.Model;
using Quill_API.SupportClass;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quill_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        [HttpGet("GetCommentsArticle/{idArticle}")]
        public ActionResult<List<Comment>> GetCommentsArticle(int idArticle)
        {
            try
            {
                var query = DbArticlesContext.Context.Comments.Include(x => x.Author)
                         .Where(o => o.ArticleId == idArticle && o.Status == "published").AsQueryable();
                var comments = query
                    .Select(x => new
                    {
                        Id = x.Id,
                        AuthorId = x.AuthorId,
                        AuthorName = x.Author.Nickname,
                        Content = x.Content,
                        PublishedAt = x.PublishedAt,
                        Status = x.Status
                    })
                    .ToList();
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return BadRequest("Ошибка при выводе статей пользователя: " + ex.Message);
            }
        }

        [HttpPost("AddNewComment")]
        public ActionResult AddNewComment(CommentClass commentClass)
        {
            if (DbArticlesContext.Context.Articles.Where(obj => obj.Id == commentClass.ArticleId).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            if (DbArticlesContext.Context.Users.Where(obj => obj.Id == commentClass.AuthorId).FirstOrDefault() == null)
                return NotFound("Пользователь не найден!");

            DbArticlesContext.Context.Comments.Add(new Comment()
            {
                Content = commentClass.Content,
                PublishedAt = DateTime.Now,
                Status = "published",
                ArticleId = commentClass.ArticleId,
                AuthorId = commentClass.AuthorId,
            });
            DbArticlesContext.Context.SaveChanges();
            return Ok("Комментарий добавлен!");
        }

        [HttpPut("ChangeStatusComment")]
        public ActionResult ChangeStatusComment(StatusCommentClass statusCommentClass)
        {
            Comment? comment = DbArticlesContext.Context.Comments.Where(obj => obj.Id == statusCommentClass.Id).FirstOrDefault();
            if (comment == null)
                return NotFound("Комментарий не найден!");

            List<string> allStatus = new List<string>()
            {
                "published", 
                "deleted"
            };

            if(!allStatus.Contains(statusCommentClass.Status))
                return NotFound("Статус комментария не найден!");

            comment.Status = statusCommentClass.Status;

            DbArticlesContext.Context.SaveChanges();
            return Ok("Статус комментария изменён!");
        }
    }
}
