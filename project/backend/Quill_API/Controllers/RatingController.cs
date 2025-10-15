using Microsoft.AspNetCore.Mvc;
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
            if (QuillBdContext.Context.Articles.Where(obj => obj.Id == idArticle).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            return Ok(QuillBdContext.Context.Ratings.Where(obj => obj.ArticleId == idArticle && obj.Rating1 == "Нравится").ToList().Count());
        }
        
        [HttpGet("GetDislikeRatingsArticle/{idArticle}")]
        public ActionResult GetSilikeRatingsArticle(int idArticle)
        {
            if (QuillBdContext.Context.Articles.Where(obj => obj.Id == idArticle).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            return Ok(QuillBdContext.Context.Ratings.Where(obj => obj.ArticleId == idArticle && obj.Rating1 == "Не нравится").ToList().Count());
        }

        [HttpGet("GetRatingUser/{idUser}")]
        public ActionResult GetRatingUser(int idUser)
        {
            if (QuillBdContext.Context.Users.Where(obj => obj.Id == idUser).FirstOrDefault() == null)
                return NotFound("Пользователь не найден!");

            return Ok(QuillBdContext.Context.Ratings.Where(obj => obj.UserId == idUser).ToList());
        }

        [HttpPost("SetRatingUser")]
        public ActionResult SetRatingUser(RatingClass ratingClass)
        {
            if (!HelpFunc.CheckCorrectlyIdUser(QuillBdContext.Context.Users, ratingClass.UserId))
                return NotFound("Пользователь не найден!");

            if (QuillBdContext.Context.Articles.Where(obj => obj.Id == ratingClass.ArticleId).FirstOrDefault() == null)
                return NotFound("Статья не найдена!");

            List<string> ratings = new List<string>()
            {
                "Нравится",
                "Не нравится"
            };

            if (!ratings.Contains(ratingClass.Rating1))
                return NotFound("Рейтинг не найден!");

            Rating? rating = QuillBdContext.Context.Ratings.Where(obj => obj.ArticleId == ratingClass.ArticleId && obj.UserId == ratingClass.UserId).FirstOrDefault();

            if (rating != null)
            {
                rating.Rating1 = ratingClass.Rating1;
            }
            else
            {
                QuillBdContext.Context.Ratings.Add(new Rating{
                    ArticleId = ratingClass.ArticleId,
                    UserId = ratingClass.UserId,
                    Rating1 = ratingClass.Rating1,
                });
            }

            QuillBdContext.Context.SaveChanges();
            return Ok("Рейтинг добавлен!");
        }

        [HttpDelete("DeleteRating")]
        public ActionResult DeleteRating(RatingClass ratingClass)
        {
            Rating? rating = QuillBdContext.Context.Ratings.Where(obj => obj.ArticleId == ratingClass.ArticleId && obj.UserId == ratingClass.UserId).FirstOrDefault();

            if (rating == null)
                return NotFound("Рейтинг не найден!");

            QuillBdContext.Context.Ratings.Remove(rating!);
            QuillBdContext.Context.SaveChanges();
            return Ok("Рейтинг удалён!");
        }

    }
}
