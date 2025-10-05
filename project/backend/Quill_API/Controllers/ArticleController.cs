using Microsoft.AspNetCore.Mvc;
using Quill_API.Model;
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

    }
}
