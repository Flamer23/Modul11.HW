using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace Modul11.HW
{
    class Program
    {
        private static ITelegramBotClient _botClient;

        private static ReceiverOptions _receiverOptions;

        static async Task Main()
        {

            _botClient = new TelegramBotClient("6434280774:AAEt8t61NTXER7AA9hF75fsXpVXKRMFTHwc");
            _receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                },
                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);

            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"{me.FirstName} запущен!");

            await Task.Delay(-1);
        }
        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        {
                            var message = update.Message;

                            var user = message.From;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                            var chat = message.Chat;

                            switch (message.Type)
                            {
                                case MessageType.Text:
                                    {
                                        if (message.Text == "/start")
                                        {
                                            // Тут создаем нашу клавиатуру
                                            var inlineKeyboard = new InlineKeyboardMarkup(
                                                new List<InlineKeyboardButton[]>() 
                                                {
                                        

                                        new InlineKeyboardButton[] 
                                        {
                                            InlineKeyboardButton.WithCallbackData("Подсчёт символов", "button1"),
                                            InlineKeyboardButton.WithCallbackData("Сложение", "button2"),
                                        },                                       
                                                });

                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Выбери действие!",
                                                replyMarkup: inlineKeyboard); 

                                            return;
                                        }

                                        string filePath = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\"), "Database.txt");

                                        using (StreamWriter fileStream = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath)) { }

                                        string[] poisk = System.IO.File.ReadAllLines(filePath);

                                        string id = $"{user.Id}";

                                        if (poisk.Contains($"{id} сложение"))
                                        {
                                            try
                                            {
                                                int count = 0;
                                                string[] temp = message.Text.Split(new Char[] { ' ' });
                                                for (int i = 0; i < temp.Length; i++)
                                                {
                                                    count += Convert.ToInt32(temp[i]);
                                                }
                                                await botClient.SendTextMessageAsync(chat.Id, $"Результат: {count}");

                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine(ex.Message);
                                                await botClient.SendTextMessageAsync(chat.Id, $"Используйте только целые числа");
                                            }
                                            
                                        }

                                        if (poisk.Contains($"{id} подсчёт"))
                                        {
                                            
                                            await botClient.SendTextMessageAsync(chat.Id, $"Количество символов: {message.Text.Length}");
                                        }

                                        return;
                                    }

                                default:
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Используй только текст!");
                                        return;
                                    }
                            }

                        }

                    case UpdateType.CallbackQuery:
                        {
                            var callbackQuery = update.CallbackQuery;

                            var user = callbackQuery.From;

                            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

                            var chat = callbackQuery.Message.Chat;

                            switch (callbackQuery.Data)
                            {

                                case "button1":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        await botClient.SendTextMessageAsync(chat.Id, $"Отправьте сообщение для подсчёта");

                                        string filePath = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\"), "Database.txt");

                                        using (StreamWriter fileStream = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath)) { }

                                        string[] poisk = System.IO.File.ReadAllLines(filePath);

                                        string id = $"{user.Id}";

                                        if (poisk.Contains($"{id} сложение"))
                                        {
                                            for(int i = 0; i < poisk.Length; i++)
                                            {
                                                if (poisk[i].Contains(id))
                                                {
                                                    poisk[i] = $"{user.Id} подсчёт";
                                                    break;
                                                }
                                            }
                                            System.IO.File.WriteAllLines(filePath, poisk);
                                        }
                                        else if (!poisk.Contains($"{id} подсчёт"))
                                        {
                                            using (StreamWriter fileStream = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath))
                                            {
                                                fileStream.WriteLine($"{user.Id} подсчёт");
                                            }
                                        }

                                        return;
                                    }

                                case "button2":
                                    {
                                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                        await botClient.SendTextMessageAsync(chat.Id, $"Отправьте числа через пробел для сложения");

                                        string filePath = Path.Combine(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\"), "Database.txt");

                                        string[] poisk = System.IO.File.ReadAllLines(filePath);

                                        string id = $"{user.Id}";

                                        if (poisk.Contains($"{id} подсчёт"))
                                        {
                                            for (int i = 0; i < poisk.Length; i++)
                                            {
                                                if (poisk[i].Contains(id))
                                                {
                                                    poisk[i] = $"{user.Id} сложение";
                                                    break;
                                                }
                                            }
                                            System.IO.File.WriteAllLines(filePath, poisk);
                                        }
                                        else if (!poisk.Contains($"{id} сложение"))
                                        {
                                            using (StreamWriter fileStream = System.IO.File.Exists(filePath) ? System.IO.File.AppendText(filePath) : System.IO.File.CreateText(filePath))
                                            {
                                                fileStream.WriteLine($"{user.Id} сложение");
                                            }
                                        }

                                        return;
                                    }

                                
                            }

                            return;
                        }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }

}
