using CobainStats.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CobainStats.Controllers
{
    public class AuthController: Controller
    {
        private const string TelegramBotToken = "7731702375:AAGh7QiVVOqWzn2W_ApYVSO68V04EdpYL1g";
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> TelegramCallback([FromQuery] TelegramAuth model)
        {
            if (!IsValidTelegramAuth(model, TelegramBotToken))
                return Unauthorized("Invalid Telegram auth data");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.TelegramId == model.Id);
            if (user == null)
            {
                user = new User
                {
                    TelegramId = model.Id,
                    UserName = model.Username ?? $"user_{model.Id}",
                    FirstName = model.FirstName,
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest("Cannot create user");
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
            return RedirectToAction("Index", "Home");
        }

        private static bool IsValidTelegramAuth(TelegramAuth model, string botToken)
        {
            var data = new SortedDictionary<string, string>
        {
            { "auth_date", model.AuthDate.ToString() },
            { "first_name", model.FirstName },
            { "id", model.Id.ToString() }
        };

            if (!string.IsNullOrEmpty(model.Username))
                data["username"] = model.Username;

            var dataCheckString = string.Join("\n", data.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(botToken));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
            var hashHex = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return hashHex == model.Hash.ToLower();
        }
    }
}
