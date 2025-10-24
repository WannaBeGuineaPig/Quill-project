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
            if (!HelpFunc.CheckCorrectlyMailAddress(authorizationClass.email)) 
                return BadRequest("Не корректная почта!");
            if (!HelpFunc.CheckCorrectlyPassword(authorizationClass.password)) 
                return BadRequest("Не корректный пароль!");
            if (!HelpFunc.CheckExitMail(QuillBdContext.Context.Users, authorizationClass.email)) 
                return BadRequest("Не правильна введена почта или пароль!");
            User? user = HelpFunc.GetUserOnMail(QuillBdContext.Context.Users, authorizationClass.email);
            if (!HelpFunc.CheckMatchHesh(authorizationClass.password, user!.Password)) 
                return BadRequest("Не правильна введена почта или пароль!");
            return Ok(user);
        }

        [HttpPost("RegistrationUser")]
        public ActionResult RegistrationUser(User user)
        {
            if (!HelpFunc.CheckCorrectlyMailAddress(user.Email)) 
                return BadRequest("Введена некорректная почта!!!");
            
            if (HelpFunc.CheckExitMail(QuillBdContext.Context.Users, user.Email)) 
                return BadRequest("Пользователь с такой почтой уже существует!");
            
            if (!HelpFunc.CheckCorrectlyPassword(user.Password)) 
                return BadRequest("Не корректный пароль!");
            
            if (!HelpFunc.CheckCorrectlyNickName(user.Nickname)) 
                return BadRequest("Не корректный логин!");

            if (!HelpFunc.CheckUniqueNickname(QuillBdContext.Context.Users, user.Nickname)) 
                return BadRequest("Данный логин уже существует!");

            user.Password = HelpFunc.CreateHeshPassword(user.Password);
            user.Role = "user";
            user.Status = "active";
            QuillBdContext.Context.Users.Add(user);
            QuillBdContext.Context.SaveChanges();
            return Ok("Успешная регистрация!");
        }

        [HttpPut("ChangeUserInfo")]
        public ActionResult ChangeUserInfo(User user)
        {

            var changedUser = QuillBdContext.Context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (changedUser == null)
            {
                return BadRequest("Пользователь не найден");
            }
            if (!HelpFunc.CheckCorrectlyMailAddress(user.Email))
                return BadRequest("Не корректная почта!");
            var u = QuillBdContext.Context.Users.Where(c => c.Email == user.Email).FirstOrDefault();
            if (u is User && user.Id != u.Id)
                return BadRequest("Пользователь с такой почтой уже существует!");

            //if (!UserHelp.CheckCorrectlyPassword(user.Password))
            //    return BadRequest("Не корректный пароль!");

            if (!HelpFunc.CheckCorrectlyNickName(user.Nickname))
                return BadRequest("Не корректный логин!");
            var n = QuillBdContext.Context.Users.Where(c => c.Nickname == user.Nickname).FirstOrDefault();
            if (n is User && n.Id != user.Id)
                return BadRequest("Данный логин уже существует!");

            changedUser.Email = user.Email;
            changedUser.Nickname = user.Nickname;
            QuillBdContext.Context.SaveChanges();
            return Ok(changedUser);
        }

        [HttpPut("ChangeStatusUser")]
        public ActionResult ChangeStatusUser(StatusUserClass statusUserClass)
        {
            User? user = QuillBdContext.Context.Users.Where(obj => obj.Id == statusUserClass.Id).FirstOrDefault();
            if (user == null)
            {
                return BadRequest("Пользователь не найден!");
            }

            List<string> allStatus = new List<string>()
            {
                "active",
                "deleted",
            };

            if (!allStatus.Contains(statusUserClass.Status))
                return BadRequest("Статус пользователя не найден!");

            user.Status = statusUserClass.Status;
            QuillBdContext.Context.SaveChanges();
            return Ok("Статус пользователя успешно изменён!");
        }
    }
}
