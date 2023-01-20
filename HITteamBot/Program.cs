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
using HITteamBot.Repository.Entities.Characters.Races;

namespace HITteamBot
{
    class Program
    {
        public static readonly string BaseDirectory = AppContext.BaseDirectory;
        public static readonly string DataDirectory = BaseDirectory + @"\Data";
        public static readonly string AssetsDirectory = DataDirectory + @"\Assets";
        public static readonly string UsersDirectory = DataDirectory + @"\Users";
        public static readonly string WorldDirectory = DataDirectory + @"\World";
        public static readonly string PlayersDirectory = WorldDirectory + @"\Players";
        public static readonly string RacesDirectory = WorldDirectory + @"\Races";
        public static readonly string ClassesDirectory = WorldDirectory + @"\Classes";
        public static readonly string ItemsDirectory = WorldDirectory + @"\Items";
        static ITelegramBotClient bot = new TelegramBotClient("5643667905:AAGeZiUGhEGUP9cAXEU7Llx9Bk6UvfuxCgc");

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
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && !string.IsNullOrEmpty(message.Text) && DateTime.Now.ToUniversalTime() - message.Date < TimeSpan.FromMinutes(5))
                {
                    string info;
                    switch (message.Text.ToLower().Replace("/", "").Replace("@hit_team_bot", "").Split(new char[] { ' ' })[0])
                    {
                        case "start":
                            Menu(botClient, message.Chat.Id, cancellationToken);
                            return;
                        case "menu":
                            _ = botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId, cancellationToken);
                            Menu(botClient, message.Chat.Id, cancellationToken);
                            return;
                        case "new":
                            string[] strings = message.Text.Replace("/new", "").Replace("@HIT_team_bot", "").Split(new char[] { ' ' });
                            CharactersController.CreateNewCharacter(botClient, message.Chat, message.From.Username, strings[1], Int16.Parse(strings[2]), strings.Length > 2 ? string.Join(' ', strings[3..]) : "", cancellationToken);
                            return;
                        case "addrace":
                            if (message.From.Username == "Lizardrock")
                            {
                                _ = botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId - 1, cancellationToken);
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await RaceController.AddNewRace(message.Text.Replace("/addRace", "").Replace("@HIT_team_bot", "")));
                            }
                            else
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        case "addraceability":
                            if (message.From.Username == "Lizardrock")
                            {
                                _ = botClient.DeleteMessageAsync(message.Chat.Id, message.MessageId - 1, cancellationToken);
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await RaceController.SetRaceAbility(message.Text.Replace("/addRaceAbility", "").Replace("@HIT_team_bot", "")));
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

                    if (callback.Contains("ContinueWith"))
                    {
                        CharactersController.ContinueJourneyWithCharacter(botClient, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.From.Username, callback.Replace("ContinueWith", ""), cancellationToken);
                        return;
                    }

                    if (callback.Contains("SetRace"))
                    {
                        CharacterRaces race = (CharacterRaces)Enum.Parse(typeof(CharacterRaces), callback.Replace("SetRace", ""));
                        _ = botClient.SendTextMessageAsync(chat.Id, Dictionaries.GetRace(race) + "...");
                        CharactersController.SetCharacterRace(botClient, chat.Id, update.CallbackQuery.From.Username, "name", race, cancellationToken);
                        return;
                    }

                    if (callback.Contains("SetClass"))
                    {
                        CharactersController.SetCharacterClass(botClient, chat.Id, update.CallbackQuery.From.Username, callback.Replace("SetClass", ""), cancellationToken);
                        return;
                    }

                    switch (callback)
                    {
                        case "NewCharacter":
                            _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                            _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId - 1, cancellationToken);
                            await botClient.SendTextMessageAsync(chat.Id, "Придумайте персонажу имя, возраст и предисторию (по желанию) и начните сообщение с /new.\r\n\r\nПример:\r\n/new Гэндальф 413 Был послан богами чтобы навести порядок в Средиземье...");
                            return;
                        case "Characters":
                            _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                            _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId - 1, cancellationToken);
                            CharactersController.GetUserCharacters(botClient, chat, update.CallbackQuery.From.Username, cancellationToken);
                            return;

                        // Админка
                        case "Settings":
                            if (update.CallbackQuery.From.Username == "Lizardrock")
                            {
                                _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                                _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId - 1, cancellationToken);
                                Settings(botClient, chat, cancellationToken);
                            }
                            else
                                _ = botClient.SendTextMessageAsync(chat.Id, "У вас нет прав (на настройку тоже)");
                            return;
                        case "NewRace":
                            if (update.CallbackQuery.From.Username == "Lizardrock")
                            {
                                _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                                _ = botClient.SendTextMessageAsync(chat.Id, "/addRace имя сил лвк вын инт мдр хар хп мп вын");
                            }
                            else
                                _ = botClient.SendTextMessageAsync(chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        case "AddRaceAbility":
                            if (update.CallbackQuery.From.Username == "Lizardrock")
                            {
                                _ = botClient.DeleteMessageAsync(chat.Id, update.CallbackQuery.Message.MessageId, cancellationToken);
                                _ = botClient.SendTextMessageAsync(chat.Id, "/addRaceAbility имя_расы название 100 описание");
                            }
                            else
                                _ = botClient.SendTextMessageAsync(chat.Id, "У вас нет прав (на это тоже)");
                            return;
                        default:
                            CharactersController.GetCharacterInfo(botClient, chat.Id, update.CallbackQuery.From.Username, callback, cancellationToken);
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
            if (!Directory.Exists(UsersDirectory)) Directory.CreateDirectory(UsersDirectory);
            if (!Directory.Exists(WorldDirectory)) Directory.CreateDirectory(WorldDirectory);
            if (!Directory.Exists(PlayersDirectory)) Directory.CreateDirectory(PlayersDirectory);
            if (!Directory.Exists(RacesDirectory)) Directory.CreateDirectory(RacesDirectory);
            if (!Directory.Exists(ClassesDirectory)) Directory.CreateDirectory(ClassesDirectory);
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
                string logo = AssetsDirectory + @"\hitWorldLogo.png";
                using (var stream = System.IO.File.Open(logo, FileMode.Open))
                {
                    await botClient.SendStickerAsync(chatId, stream);
                }

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Создать персонажа", MainMenu.NewCharacter.ToString()),
                        InlineKeyboardButton.WithCallbackData("Мои персонажи", MainMenu.Characters.ToString())
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
                        InlineKeyboardButton.WithCallbackData("Создать расу", SettingsMenu.NewRace.ToString()),
                        InlineKeyboardButton.WithCallbackData("Создать класс", SettingsMenu.NewClass.ToString())
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить расовый навык", SettingsMenu.AddRaceAbility.ToString())
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
        Characters,
        Settings,
        Info
    }

    enum SettingsMenu
    {
        NewRace,
        NewClass,
        AddRaceAbility
    }
}
