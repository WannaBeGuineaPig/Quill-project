using Microsoft.AspNetCore.Mvc;
using Quill_API.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quill_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        [HttpGet("GetTopics")]
        public ActionResult<List<Topic>> GetTopics()
        {
            var categories = DbArticlesContext.Context.Topics.Select(x => new{Id = x.IdTopics, TopicName = x.NameTopics }).ToList();
            return Ok(categories);
        }

        [HttpGet("GetTopicsOnName/{nameTopic}")]
        public ActionResult GetTopicsOnName(string nameTopic)
        {
            Topic? topic = DbArticlesContext.Context.Topics.Where(obj => obj.NameTopics == nameTopic).FirstOrDefault();
            return topic == null ? NotFound("Категория не найдена!") : Ok(topic);
        }
    }
}
