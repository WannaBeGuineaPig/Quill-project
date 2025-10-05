using Microsoft.AspNetCore.Mvc;
using Quill_API.Model;
using Quill_API.SupportClass;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Quill_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public List<User> GetAllUser()
        {
            return QuillBdContext.Context.Users.ToList();
        }

        [HttpPost("AuthorizationUser")]
        public ActionResult AuthorizationUser([FromBody] SupportClass.AuthorizationClass authorizationClass)
        {
            if (!UserHelp.CheckCorrectlyMailAddress(authorizationClass.email)) 
                return BadRequest("Не корректная почта!");
            if (!UserHelp.CheckCorrectlyPassword(authorizationClass.password)) 
                return BadRequest("Не корректный пароль!");
            if (!UserHelp.CheckExitMail(authorizationClass.email)) 
                return BadRequest("Не правильна введена почта или пароль!");
            User? user = UserHelp.GetUserOnMail(authorizationClass.email);
            if (!UserHelp.CheckMatchHesh(authorizationClass.password, user!.Password)) 
                return BadRequest("Не правильна введена почта или пароль!");
            return Ok(user.Id);
        }

        [HttpPost("RegistrationUser")]
        public ActionResult RegistrationUser(User user)
        {
            if (!UserHelp.CheckCorrectlyMailAddress(user.Email)) 
                return BadRequest("Не корректная почта!");
            
            if (UserHelp.CheckExitMail(user.Email)) 
                return BadRequest("Пользователь с такой почтой уже существует!");
            
            if (!UserHelp.CheckCorrectlyPassword(user.Password)) 
                return BadRequest("Не корректный пароль!");
            
            if (!UserHelp.CheckCorrectlyNickName(user.Nickname)) 
                return BadRequest("Не корректный логин!");

            if (!UserHelp.CheckUniqueNickname(user.Nickname)) 
                return BadRequest("Данный логин уже существует!");

            user.Password = UserHelp.CreateHeshPassword(user.Password);
            user.Role = "user";
            user.Status = "active";
            QuillBdContext.Context.Users.Add(user);
            QuillBdContext.Context.SaveChanges();
            return Ok("Успешная регистрация!");
        }
    }
}
