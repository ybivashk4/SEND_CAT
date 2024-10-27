using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;
using System.Runtime.InteropServices.JavaScript;
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
        private static string? cat_says;
        private static bool is_cat_says;
        private static List<string?> list_of_tags = new List<string?>();
        private static List<string?> list_of_colors = new List<string?>();
        private static async Task Main(string[] args) {


            is_cat_says = false;
            list_of_tags.Add("cute");
            list_of_tags.Add("angry");
            list_of_tags.Add("evil");
            list_of_tags.Add("small");

            list_of_colors.Add("black");
            list_of_colors.Add("orange");
            list_of_colors.Add("white");


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
                        if (msg != null && update.Type == UpdateType.Message && update.Message?.Text != null && !is_cat_says) {
                            Console.WriteLine($"Received message '{msg.Text}' from {msg.From}");
                            await bot.SendTextMessageAsync(msg.Chat, "GIVE CAT", cancellationToken: cancellationToken, replyMarkup: get_first_button());
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

        private static InlineKeyboardMarkup get_first_button() {
            InlineKeyboardButton my_btn = new InlineKeyboardButton("GET CAT");
            my_btn.CallbackData = "first_input";
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(my_btn);
            return keyboard;
        }

        private static InlineKeyboardMarkup get_buttons() {
            InlineKeyboardButton btn_out = new InlineKeyboardButton("JUST GET CAT");
            InlineKeyboardButton btn_says = new InlineKeyboardButton("your cat would say...");
            InlineKeyboardButton btn_tag = new InlineKeyboardButton("choose your fighter");
            InlineKeyboardButton btn_color = new InlineKeyboardButton("choose color");

            InlineKeyboardButton[] all_buttons = { btn_out, btn_says, btn_tag, btn_color };

            btn_out.CallbackData = "CAT_OUT";
            btn_says.CallbackData = "cat_says";
            btn_tag.CallbackData = "cat_tags";
            btn_color.CallbackData = "cat_color";
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(all_buttons);
            return keyboard;
        }
        private static InlineKeyboardMarkup get_tags_buttons() {
            List<InlineKeyboardButton> all_buttons = new List<InlineKeyboardButton>();
            foreach (var tag in list_of_tags) {
                InlineKeyboardButton btn = new InlineKeyboardButton(tag);
                btn.CallbackData = tag;
                all_buttons.Add(btn);
            }


            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(all_buttons.ToArray());
            return keyboard;
        }

        private static InlineKeyboardMarkup get_colors_buttons() {
            List<InlineKeyboardButton> all_buttons = new List<InlineKeyboardButton>();
            foreach (var color in list_of_colors) {
                InlineKeyboardButton btn = new InlineKeyboardButton(color);
                btn.CallbackData = color;
                all_buttons.Add(btn);
            }


            var keyboard = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(all_buttons.ToArray());
            return keyboard;
        }

        static async Task HandleButton(CallbackQuery query) {
            string? text = string.Empty;

            InlineKeyboardMarkup markup = new(Array.Empty<InlineKeyboardButton>());

            if (string.Equals(query.Data , "first_input")) {
                callback_query(query);

                try {
                    await bot.SendTextMessageAsync(query.Message.Chat.Id, "setting your CAT",
                         replyMarkup: get_buttons());
                }
                catch (Exception e) {
                    System.Console.WriteLine(e);
                }
            }
            else if (string.Equals(query.Data, "CAT_OUT")) {
                callback_query(query);

                var url = new Url(get_url_my("https://cataas.com", "cat"));
                text = url;
                await bot.SendPhotoAsync(
                    query.Message!.Chat.Id,
                    photo: InputFile.FromString(text),
                    replyMarkup: get_first_button()
                );
                System.Console.WriteLine("url is : " + url);
            }
            else if (string.Equals(query.Data, "cat_says")) {
                callback_query(query);

                is_cat_says = true;
                try {
                    wait_for_message(cts.Token);
                    while (is_cat_says) {
                        await Task.Delay(3000);
                    }
                    var url = new Url(get_url_my("https://cataas.com", "cat", "says", cat_says));
                    cat_says = "";
                    text = url;
                    await bot.SendPhotoAsync(
                        query.Message!.Chat.Id,
                        photo: InputFile.FromString(text),
                        replyMarkup: get_first_button()
                    );
                }
                catch (Exception e) {
                    System.Console.WriteLine(e);
                }
            }
            else if (string.Equals(query.Data, "cat_tags")) {
                callback_query(query);
                try {
                    await bot.SendTextMessageAsync(query.Message.Chat.Id, "choose tags",
                        replyMarkup: get_tags_buttons());
                }
                catch (Exception e) {
                    System.Console.WriteLine(e);
                }
            }
            else if (list_of_tags.Contains(query.Data)) {
                callback_query(query);
                var cur_tag = list_of_tags.Find(x => x == query.Data);
                if (!(cur_tag is null)) {
                    var url = new Url(get_url_my("https://cataas.com", "cat", cur_tag));
                    text = url;
                    await bot.SendPhotoAsync(
                        query.Message!.Chat.Id,
                        photo: InputFile.FromString(text),
                        replyMarkup: get_first_button()
                    );
                }
            }
            else if (string.Equals(query.Data, "cat_color")) {
                callback_query(query);
                try {
                    await bot.SendTextMessageAsync(query.Message.Chat.Id, "choose colors",
                        replyMarkup: get_colors_buttons());
                }
                catch (Exception e) {
                    System.Console.WriteLine(e);
                }
            }
            else if (list_of_colors.Contains(query.Data)) {
                callback_query(query);
                var cur_color = list_of_colors.Find(x => x == query.Data);
                if (!(cur_color is null)) {
                    var url = new Url(get_url_my("https://cataas.com", "cat", cur_color));
                    text = url;
                    await bot.SendPhotoAsync(
                        query.Message!.Chat.Id,
                        photo: InputFile.FromString(text),
                        replyMarkup: get_first_button()
                    );
                }
            }

        }

        static async Task callback_query(CallbackQuery query) {
            try {
                await bot.AnswerCallbackQueryAsync(query.Id);
            }
            catch (Exception e) {
                System.Console.WriteLine(e);
            }
        }

        private static async Task wait_for_message(CancellationToken cts_token) {
            string? message = null;
            if (is_cat_says) {
                while (!cts_token.IsCancellationRequested) {
                    var updates = await bot.GetUpdatesAsync();
                    if (!(updates is null)) {
                        foreach (var update in updates) {
                            if (!(update is null) && !(update.Message is null)) {
                                is_cat_says = false;
                                cat_says = update.Message.Text;
                                break;
                            }
                        }
                    }

                    await Task.Delay(3000);

                }
            }
        }
    }
}
