namespace CobainStats.Models
{
    public class TelegramAuth
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public long AuthDate { get; set; }
        public string Hash { get; set; }
    }
}
