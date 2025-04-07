using Microsoft.AspNetCore.Mvc;

namespace CobainStats.Controllers
{
    public class LoginController : Controller
    {
        // Хранилище токенов и статуса логина
        public static Dictionary<string, string> PendingLogins = new();   // token -> returnUrl
        public static HashSet<string> ConfirmedLogins = new();           // подтверждённые токены

        private readonly string _botUsername;

        public LoginController(IConfiguration config)
        {
            _botUsername = config["Telegram:BotUsername"];
        }

        // Генерация токена + редирект в Telegram
        [HttpGet]
        public IActionResult StartLogin(string returnUrl = "/")
        {
            var token = Guid.NewGuid().ToString("N");
            PendingLogins[token] = returnUrl;

            // Передаём токен на страницу ожидания
            return View("Pending", model: token);
        }

        // Проверка статуса логина (используется JS)
        [HttpGet]
        public IActionResult CheckLogin(string token)
        {
            if (ConfirmedLogins.Contains(token) && PendingLogins.TryGetValue(token, out var url))
            {
                ConfirmedLogins.Remove(token);
                PendingLogins.Remove(token);
                return Content(url);
            }

            return NotFound();
        }

        // View ожидания входа через Telegram
        [HttpGet]
        public IActionResult Pending(string token)
        {
            return View(model: token);
        }

        // Вызывается ботом при подтверждении входа
        public static void ConfirmLogin(string token)
        {
            ConfirmedLogins.Add(token);
        }
        public static bool ValidateToken(string token, out string returnUrl)
        {
            if (PendingLogins.TryGetValue(token, out returnUrl))
            {
                return true;
            }

            return false;
        }

    }
}
