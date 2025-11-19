using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TEST.Controllers;
using TEST.Model;
using Xunit;

namespace TEST.Unit_tests
{
    public class AdminController_AddTeacher_Tests
    {
        private class TestContext : ExamProctoringSuiteContext
        {
            public TestContext(DbContextOptions<ExamProctoringSuiteContext> options) : base(options) { }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        }

        private static ExamProctoringSuiteContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ExamProctoringSuiteContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new TestContext(options);
        }

        // Тест: модель не передана -> ожидаем BadRequest с сообщением "Данные не предоставлены"
        [Fact]
        public void ReturnsBadRequest_WhenModelIsNull()
        {
            var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new AdminController(context);

            var result = controller.AddTeacher(null);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Данные не предоставлены", bad.Value);
        }

        // Тест: отсутствуют обязательные поля (Email/Password/FirstName/LastName) -> BadRequest с сообщением
        [Fact]
        public void ReturnsBadRequest_WhenRequiredFieldsMissing()
        {
            var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new AdminController(context);

            var model = new AdminController.AddTeacherModel
            {
                Email = "",
                Password = "p",
                FirstName = "f",
                LastName = "l",
                Patronymic = null
            };

            var result = controller.AddTeacher(model);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Все обязательные поля должны быть заполнены", bad.Value);
        }

        // Тест: Email уже существует в БД -> BadRequest с сообщением "Email уже используется"
        [Fact]
        public void ReturnsBadRequest_WhenEmailAlreadyExists()
        {
            var context = CreateContext(Guid.NewGuid().ToString());
            context.Users.Add(new User
            {
                Email = "exists@example.com",
                PasswordHash = "h",
                FirstName = "f",
                LastName = "l",
                Patronymic = "",
                RoleId = 2
            });
            context.SaveChanges();

            var controller = new AdminController(context);

            var model = new AdminController.AddTeacherModel
            {
                Email = "exists@example.com",
                Password = "p",
                FirstName = "f",
                LastName = "l",
                Patronymic = null
            };

            var result = controller.AddTeacher(model);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Email уже используется", bad.Value);
        }

        // Тест: валидный ввод -> Ok, пользователь добавлен (RoleId=2, пароль хешируется, Patronymic пустая строка)
        [Fact]
        public void ReturnsOk_AndAddsUser_WhenValidInput()
        {
            var context = CreateContext(Guid.NewGuid().ToString());
            var controller = new AdminController(context);

            var model = new AdminController.AddTeacherModel
            {
                Email = "new@example.com",
                Password = "password123",
                FirstName = "Ivan",
                LastName = "Ivanov",
                Patronymic = null
            };

            var result = controller.AddTeacher(model);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);

            var added = Assert.Single(context.Users);
            Assert.Equal("new@example.com", added.Email);
            Assert.Equal(2, added.RoleId);
            Assert.NotEqual("password123", added.PasswordHash);
            Assert.Equal("", added.Patronymic);
        }

        private class ThrowingContext : TestContext
        {
            public ThrowingContext(DbContextOptions<ExamProctoringSuiteContext> options) : base(options) { }
            public override int SaveChanges()
            {
                throw new Exception("fail");
            }
        }

        // Тест: исключение при сохранении контекста -> ответ 500
        [Fact]
        public void Returns500_WhenSaveChangesThrows()
        {
            var options = new DbContextOptionsBuilder<ExamProctoringSuiteContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new ThrowingContext(options);

            context.Users.Add(new User
            {
                Email = "throws@example.com",
                PasswordHash = "h",
                FirstName = "f",
                LastName = "l",
                Patronymic = "",
                RoleId = 2
            });

            var controller = new AdminController(context);

            var model = new AdminController.AddTeacherModel
            {
                Email = "throws2@example.com",
                Password = "p",
                FirstName = "f",
                LastName = "l",
                Patronymic = null
            };

            var result = controller.AddTeacher(model);

            var obj = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, obj.StatusCode);
        }
    }
}