using Microsoft.AspNetCore.Identity;

namespace CobainStats.Models
{
    public class User : IdentityUser
    {
        public long TelegramId { get; set; }
        public string FirstName { get; set; }
    }
}
