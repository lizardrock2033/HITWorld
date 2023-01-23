using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;
using System.Text;
using System.Linq;
using HITteamBot.Repository.Controllers;
using HITteamBot.Repository.Controllers.Characters;
using HITteamBot.Repository.Entities.System;

namespace HITteamBot
{
    class Program
    {
        public static readonly string BaseDirectory = AppContext.BaseDirectory;
        public static readonly string DataDirectory = BaseDirectory + "Data";
        public static readonly string AssetsDirectory = DataDirectory + @"\Assets";
        public static readonly string ObjectsDirectory = DataDirectory + @"\Objects";
        public static readonly string UsersDirectory = ObjectsDirectory + @"\Users";
        public static readonly string PerksDirectory = ObjectsDirectory + @"\Perks";
        public static readonly string ActionsDirectory = ObjectsDirectory + @"\Actions";
        public static readonly string ItemsDirectory = ObjectsDirectory + @"\Items";
        static ITelegramBotClient bot = new TelegramBotClient("5643667905:AAGeZiUGhEGUP9cAXEU7Llx9Bk6UvfuxCgc");
        public static List<EventsTimer> Events = new List<EventsTimer>();
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(bot.GetMeAsync().Result.FirstName + " запущен...");
                CreateDirectories();

                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { }, // receive all update types
                };
                bot.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
                Console.ReadLine();
            }
            catch (Exception)
            {

            }
        }
        
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            try
            {
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && !string.IsNullOrEmpty(message.Text) && DateTime.Now.ToUniversalTime() - message.Date < TimeSpan.FromMinutes(5) && message.Text[0] == '/')
                {
                    string info;
                    switch (message.Text.ToLower().Replace("/", "").Replace($"@{botClient.GetMeAsync().Result.FirstName.ToLower()}", "").Split(new char[] { ' ' })[0])
                    {
                        case "start":
                            Menu(botClient, message.Chat.Id, cancellationToken);
                            return;
                        case "menu":
                            Menu(botClient, message.Chat.Id, cancellationToken);
                            return;
                        case "создать":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.CreateNewCharacter(message.From.Username + message.Text.Replace("/создать", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")), Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            return;
                        case "атрибуты":
                            InlineKeyboardMarkup newAvatar = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Новый аватар", MainMenu.Avatar.ToString()) });
                            _ = botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: await CharactersController.SetCharacterAttributes(message.From.Username + message.Text.Replace("/атрибуты", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")), Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newAvatar, cancellationToken: cancellationToken);
                            return;
                        case "аватар":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.SetCharacterAvatar(message.From.Username + message.Text.Replace("/аватар", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;

                        case "вылазка":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.SetCharacterAvatar(message.From.Username + message.Text.Replace("/вылазка", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;


                        case "таймер":
                            string[] query = message.Text.Replace("/таймер", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "").Trim().Split(new char[] { ' ' });
                            TimerCallback timerCallback = new TimerCallback(ActionsController.Action);
                            Events.Add(new EventsTimer()
                            {
                                Username = message.From.Username,
                                TimerName = query[0],
                                Timer = BaseController.SetTimer(timerCallback, string.Join(' ', query[3..]), Int32.Parse(query[1]) * 1000, Int32.Parse(query[2]) * 1000)
                            });
                            
                            return;
                        case "сбростаймер":
                            string[] query2 = message.Text.Replace("/сбростаймер", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "").Trim().Split(new char[] { ' ' });
                            Events.Where(s => s.TimerName == query2[0] && s.Username == message.From.Username).FirstOrDefault().Timer.Dispose();
                            Events.Remove(Events.Where(s => s.TimerName == query2[0] && s.Username == message.From.Username).FirstOrDefault());
                            return;

                        // Админская настройка
                        case "newperk":
                            if (message.From.Username == "Lizardrock")
                            {
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await PerksController.AddNewPerk(message.Text.Replace("/newPerk", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            }
                            else
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        case "newaction":
                            if (message.From.Username == "Lizardrock")
                            {
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await PerksController.AddNewAction(message.Text.Replace("/newAction", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            }
                            else
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        case "addperk":
                            if (message.From.Username == "Lizardrock")
                            {
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await PerksController.AddPerkToCharacter(message.Text.Replace("/addPerk", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            }
                            else
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        default:
                            info = "чего?";
                            break;
                    }
                    await botClient.SendTextMessageAsync(message.Chat, info);
                }

                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    string callback = update.CallbackQuery.Data;
                    Chat chat = update.CallbackQuery.Message.Chat;

                    switch (callback)
                    {
                        case "NewCharacter":
                            await botClient.SendTextMessageAsync(chat.Id, "Придумайте персонажу имя, возраст и пол и начните сообщение с /создать.\r\n\r\n" +
                                                                            "Пример:\r\n/создать Глория 34 женский");
                            return;
                        case "Character":
                            InlineKeyboardMarkup newAvatar = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Новый аватар", MainMenu.Avatar.ToString()) });
                            InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", MainMenu.NewCharacter.ToString()) });
                            string charInfo = await CharactersController.GetCharacterInfo(update.CallbackQuery.From.Username);
                            _ = botClient.SendTextMessageAsync(chatId: chat.Id, text: charInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (charInfo.Contains("не найден") ? newCharacter : newAvatar), cancellationToken: cancellationToken);
                            return;
                        case "Avatar":
                            _ = botClient.SendTextMessageAsync(chat.Id, "Вы можете выбрать любой эмодзи в качестве аватара вашего персонажа. " +
                                                                        "Для этого начните сообщение с /аватар и пришлите любой понравившийся эмодзи.\r\n\r\n" +
                                                                        "Пример:\r\n/аватар 🧔🏻‍♀️");
                            return;

                        // Админка
                        case "Settings":
                            if (update.CallbackQuery.From.Username == "Lizardrock")
                            {
                                Settings(botClient, chat, cancellationToken);
                            }
                            else
                                _ = botClient.SendTextMessageAsync(chat.Id, "У вас нет прав (на настройку тоже)");
                            return;
                        case "NewPerk":
                            if (update.CallbackQuery.From.Username == "Lizardrock")
                            {
                                _ = botClient.SendTextMessageAsync(chat.Id, "/newPerk название ключевой_аттрибут стоимость тип описание");
                            }
                            else
                                _ = botClient.SendTextMessageAsync(chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        case "NewAction":
                            if (update.CallbackQuery.From.Username == "Lizardrock")
                            {
                                _ = botClient.SendTextMessageAsync(chat.Id, "/newAction название_воздействия цель тип сила описание");
                            }
                            else
                                _ = botClient.SendTextMessageAsync(chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        default:
                            return;
                    }

                }
            }
            catch (Exception)
            {

            }
        }

        public static void CreateDirectories()
        {
            if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);
            if (!Directory.Exists(AssetsDirectory)) Directory.CreateDirectory(AssetsDirectory);
            if (!Directory.Exists(ObjectsDirectory)) Directory.CreateDirectory(ObjectsDirectory);
            if (!Directory.Exists(UsersDirectory)) Directory.CreateDirectory(UsersDirectory);
            if (!Directory.Exists(PerksDirectory)) Directory.CreateDirectory(PerksDirectory);
            if (!Directory.Exists(ActionsDirectory)) Directory.CreateDirectory(ActionsDirectory);
            if (!Directory.Exists(ItemsDirectory)) Directory.CreateDirectory(ItemsDirectory);
        }

        public static string GetUserDirectory(string username)
        {
            return UsersDirectory + $@"\{username}";
        }

        public static async void Menu(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            try
            {
                string logo = AssetsDirectory + @"\vaultboy.png";
                using (var stream = System.IO.File.Open(logo, FileMode.Open))
                {
                    await botClient.SendStickerAsync(chatId, stream);
                }

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Мой персонаж", MainMenu.Character.ToString())
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Настройки (только с доступом)", MainMenu.Settings.ToString())
                    }
                });

                await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Добро пожаловать в HIT_World!",
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void Settings(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Создать перк", SettingsMenu.NewPerk.ToString()),
                        InlineKeyboardButton.WithCallbackData("Создать воздействие", SettingsMenu.NewAction.ToString())
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить че нибудь", "s")
                    }
                });

            await botClient.SendTextMessageAsync(
                        chatId: chat.Id,
                        text: "Настройки",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception.Message));
            await Task.Delay(310000);
            Main(null);
        }
    }

    enum MainMenu
    {
        NewCharacter,
        Character,
        Avatar,
        Settings,
        Info
    }

    enum SettingsMenu
    {
        NewPerk,
        NewAction,
        AddActionToPerk
    }
}
