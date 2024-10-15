using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegram_bot {
    internal class Program {
        private static TelegramBotClient bot = new TelegramBotClient("7249903391:AAGgGlrYdHSmscEPaTqgR1g_m6v4yEPml5g");
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private bool is_first;
        private static async Task Main(string[] args) {
            var me = await bot.GetMeAsync();
            Console.WriteLine($"@{me.Username} is running... Press Enter to terminate");
            //await bot.SendTextMessageAsync(bot.GetUpdatesAsync().Id, "Жамкни!", replyMarkup: keyboard);
            // Начинаем получать обновления
            bot.StartReceiving(UpdateHandler, ErrorHandler, cancellationToken: cts.Token);
            Console.ReadLine(); // Ждем ввода пользователя
            cts.Cancel(); // Останавливаем бота
        }
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken) {
            //Thread.Sleep(1000);

            try {
                switch (update.Type) {
                    case UpdateType.Message:
                        var msg = update.Message;
                        if (msg != null && update.Type == UpdateType.Message && update.Message?.Text != null) {
                            Console.WriteLine($"Received message '{msg.Text}' from {msg.From}");
                            await bot.SendTextMessageAsync(msg.Chat, "CLICK BUTTON AND GET CAT!!!!!!!!!!!!!!!!!!!!!!!!!!", cancellationToken: cancellationToken, replyMarkup: get_buttons());
                            // отслыает с сообщением, нужно, чтобы была только кнопка. 
                        }

                        break;
                    case UpdateType.CallbackQuery:
                        await HandleButton(update.CallbackQuery!);
                        break;

                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }

            //if (update.Type == UpdateType.Message && update.Message?.Text != null && update.Message?.Text.ToLower().Trim() == "cat") {

            //    var url = new Url(get_url_my("https://cataas.com", "cat"));
            //    // Эхо: отправляем обратно полученное сообщение в чат
            //    await bot.SendTextMessageAsync(msg.Chat, "a",
            //        cancellationToken: cancellationToken);
            //}
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken) {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }


        private static string get_url_my(string url, params string[] path_segments) {
            var rnd = new Random();
            string hash = rnd.Next().ToString();
            foreach (var path_segment in path_segments) {
                url =url.AppendPathSegment(path_segment);
            }
            url = url.SetFragment(hash);
            url = url.SetQueryParams(new {
                api_key = hash,
                max_results = 10000,
                q = "?"
            });
            return url;
        }

        private static InlineKeyboardMarkup get_buttons() {
            InlineKeyboardButton my_btn = new InlineKeyboardButton("CLICK_ME");
            my_btn.CallbackData = "CAT";
            //var url = new Url(get_url_my("https://cataas.com", "cat"));
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(my_btn);
            return keyboard;
        }

        static async Task HandleButton(CallbackQuery query) {
            string? text = string.Empty;
            InlineKeyboardMarkup markup = new(Array.Empty<InlineKeyboardButton>());
            var url = new Url(get_url_my("https://cataas.com", "cat"));
            text = url;

            // Close the query to end the client-side loading animation
            try {
                await bot.AnswerCallbackQueryAsync(query.Id);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException) {

            }

            // Replace menu text and keyboard
            System.Console.WriteLine("url is : " + url);
            await bot.SendPhotoAsync(
                query.Message!.Chat.Id,
                photo: InputFile.FromString(text),
                replyMarkup: get_buttons()
            );
        }
    }
}
