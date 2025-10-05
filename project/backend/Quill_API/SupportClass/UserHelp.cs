using System.Text;
using Quill_API.Model;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Quill_API.SupportClass
{
    public class UserHelp
    {
        static string patternMail = @"^[a-zA-Z0-9_.]{3,}[@][a-zA-Z]{3,}[.][a-zA-Z]+$";
        public static bool CheckCorrectlyMailAddress(string mail)
        {
            return Regex.IsMatch(mail, patternMail);
        }
        public static bool CheckExitMail(string mail)
        {
            return QuillBdContext.Context.Users.Where(c => c.Email == mail).FirstOrDefault() is User;
        }
        public static User? GetUserOnMail(string mail)
        {
            return QuillBdContext.Context.Users.Where(c => c.Email == mail).FirstOrDefault();
        }
        public static bool CheckCorrectlyPassword(string password)
        {
            return Regex.IsMatch(password, @"^[a-zA-Z0-9]{5,}$");
        }
        public static string CreateHeshPassword(string password)
        {
            using SHA256 hash = SHA256.Create();
            byte[] byteArrayPassword = Encoding.UTF8.GetBytes(password);
            byte[] hashPassword = hash.ComputeHash(byteArrayPassword);
            return Convert.ToHexString(hashPassword);
        }
        public static bool CheckMatchHesh(string inputPassword, string heshClient)
        {
            return CreateHeshPassword(inputPassword) == heshClient;
        }

        public static bool CheckCorrectlyNickName(string nickname)
        {
            return Regex.IsMatch(nickname, @"^[a-zA-Z0-9_\s]{3,}$");
        }

        public static bool CheckUniqueNickname(string nickname)
        {
            return QuillBdContext.Context.Users.Where(c => c.Nickname == nickname).FirstOrDefault() is User;
        }
    }
}
