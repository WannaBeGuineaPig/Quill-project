using Microsoft.AspNetCore.Mvc;
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
        public ActionResult GetCommentsArticle(int idArticle)
        {
            Article? article = QuillBdContext.Context.Articles.Where(obj => obj.Id == idArticle).FirstOrDefault();

            if (article == null)
                return NotFound("Статья не найдена!");

            return Ok(QuillBdContext.Context.Comments.Where(obj => obj.ArticleId == idArticle && obj.Status == "published").ToList());
        }

        [HttpPost("AddNewComment")]
        public ActionResult AddNewComment(CommentClass commentClass)
        {
            if (QuillBdContext.Context.Articles.Where(obj => obj.Id == commentClass.ArticleId).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            if (QuillBdContext.Context.Users.Where(obj => obj.Id == commentClass.AuthorId).FirstOrDefault() == null)
                return NotFound("Пользователь не найден!");

            QuillBdContext.Context.Comments.Add(new Comment()
            {
                Content = commentClass.Content,
                PublishedAt = DateTime.Now,
                Status = "published",
                ArticleId = commentClass.ArticleId,
                AuthorId = commentClass.AuthorId,
            });
            QuillBdContext.Context.SaveChanges();
            return Ok("Комментарий добавлен!");
        }

        [HttpPut("ChangeStatusComment")]
        public ActionResult ChangeStatusComment(StatusCommentClass statusCommentClass)
        {
            Comment? comment = QuillBdContext.Context.Comments.Where(obj => obj.Id == statusCommentClass.Id).FirstOrDefault();
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

            QuillBdContext.Context.SaveChanges();
            return Ok("Статус комментария изменён!");
        }
    }
}
