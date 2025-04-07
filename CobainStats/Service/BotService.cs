using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CobainStats.Service
{
    public class BotService
    {
        private readonly TelegramBotClient _botClient;
        private readonly string _callbackBaseUrl = "https://cobainstats.bandura.monster";

        public BotService(string botToken)
        {
            _botClient = new TelegramBotClient(botToken);
        }

        public void Start()
        {
            var cancellationToken = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken.Token
            );

            Console.WriteLine("✅ Telegram bot started");
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            try
            {
                if (update.Message != null && update.Message.Text?.StartsWith("/start login_") == true)
                {
                    var token = update.Message.Text.Substring("/start login_".Length);

                    var keyboard = new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithCallbackData("✅ Да, войти", $"confirm_{token}")
                    );

                    await bot.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "Вы хотите войти на сайт?",
                        replyMarkup: keyboard,
                        cancellationToken: ct
                    );
                }

                else if (update.CallbackQuery != null && update.CallbackQuery.Data?.StartsWith("confirm_") == true)
                {
                    var token = update.CallbackQuery.Data.Substring("confirm_".Length);
                    var telegramId = update.CallbackQuery.From.Id;

                    var callbackUrl = $"{_callbackBaseUrl}/Auth/TelegramLoginCallback?token={token}&id={telegramId}";

                    using var client = new HttpClient();
                    await client.GetAsync(callbackUrl);

                    await bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "✅ Вход подтверждён! Вернитесь на сайт.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in update handler: {ex.Message}");
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"❌ Bot error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
