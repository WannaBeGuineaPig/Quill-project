using System.Text;
using Quill_API.Model;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Quill_API.SupportClass
{
    public class HelpFunc
    {
        static string patternMail = @"^[a-zA-Z0-9_.]{3,}[@][a-zA-Z]{3,}[.][a-zA-Z]+$";

        public static bool CheckCorrectlyMailAddress(string mail)
        {
            return Regex.IsMatch(mail, patternMail);
        }
        public static bool CheckExitMail(DbSet<User> users, string mail)
        {
            return users.Where(c => c.Email == mail).FirstOrDefault() is User;
        }
        public static User? GetUserOnMail(DbSet<User> users, string mail)
        {
            return users.Where(c => c.Email == mail).FirstOrDefault();
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

        public static bool CheckUniqueNickname(DbSet<User> users, string nickname)
        {
            return users.Where(c => c.Nickname == nickname).Count() == 0;
        }

        public static bool CheckCorrectlyIdUser(DbSet<User> users, int id)
        {
            return users.Where(u => u.Id == id).FirstOrDefault() is User;
        }

        public static bool CheckCorrectlyIdTopic(DbSet<Topic> topics, int id)
        {
            return topics.Where(t => t.IdTopics == id).FirstOrDefault() is Topic;
        }

        public static bool CheckUniqueTitle(DbSet<Article> articles, string title, int authorId)
        {
            return articles.Where(t => t.Title == title && t.AuthorId == authorId).Count() == 0;
        }
    }
}
