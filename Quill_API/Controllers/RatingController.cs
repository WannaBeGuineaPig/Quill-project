using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quill_API.Model;
using Quill_API.SupportClass;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quill_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        [HttpGet("GetLikeRatingsArticle/{idArticle}")]
        public ActionResult GetLikeRatingsArticle(int idArticle)
        {
            if (DbArticlesContext.Context.Articles.Where(obj => obj.Id == idArticle).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            return Ok(DbArticlesContext.Context.Ratings.Where(obj => obj.ArticleId == idArticle && obj.Rating1 == 1).ToList().Count());
        }
        
        [HttpGet("GetDislikeRatingsArticle/{idArticle}")]
        public ActionResult GetSilikeRatingsArticle(int idArticle)
        {
            if (DbArticlesContext.Context.Articles.Where(obj => obj.Id == idArticle).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            return Ok(DbArticlesContext.Context.Ratings.Where(obj => obj.ArticleId == idArticle && obj.Rating1 == -1).ToList().Count());
        }

        [HttpGet("GetUserArticleRating")]
        public ActionResult GetUserArticleRating(int userId, int articleId)
        {
            if (DbArticlesContext.Context.Users.Where(obj => obj.Id == userId).FirstOrDefault() == null)
                return NotFound("Пользователь не найден!");

            var rating = DbArticlesContext.Context.Ratings
                .Where(obj => obj.UserId == userId && obj.ArticleId == articleId)
                .FirstOrDefault();

            return Ok(rating?.Rating1 ?? 0);
        }


        [HttpPost("SetRatingUser")]
        public ActionResult SetRatingUser(RatingClass ratingClass)
        {
            if (!HelpFunc.CheckCorrectlyIdUser(DbArticlesContext.Context.Users, ratingClass.UserId))
                return NotFound("Пользователь не найден!");

            if (DbArticlesContext.Context.Articles.Where(obj => obj.Id == ratingClass.ArticleId).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            //List<string> ratings = new List<string>()
            //{
            //    "Нравится",
            //    "Не нравится"
            //};

            //if (!ratings.Contains(ratingClass.Rating1))
            //    return NotFound("Рейтинг не найден!");

            Rating? rating = DbArticlesContext.Context.Ratings.Where(obj => obj.ArticleId == ratingClass.ArticleId && obj.UserId == ratingClass.UserId).FirstOrDefault();

            if (rating != null)
            {
                rating.Rating1 = ratingClass.Rating1;
            }
            else
            {
                DbArticlesContext.Context.Ratings.Add(new Rating{
                    ArticleId = ratingClass.ArticleId,
                    UserId = ratingClass.UserId,
                    Rating1 = ratingClass.Rating1
                });
            }

            DbArticlesContext.Context.SaveChanges();
            return Ok("Рейтинг добавлен!");
        }

        [HttpDelete("DeleteRating")]
        public ActionResult DeleteRating(RatingClass ratingClass)
        {
            Rating? rating = DbArticlesContext.Context.Ratings.Where(obj => obj.ArticleId == ratingClass.ArticleId && obj.UserId == ratingClass.UserId).FirstOrDefault();

            if (rating == null)
                return NotFound("Рейтинг не найден!");

            DbArticlesContext.Context.Ratings.Remove(rating!);
            DbArticlesContext.Context.SaveChanges();
            return Ok("Рейтинг удалён!");
        }

    }
}
