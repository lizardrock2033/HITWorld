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
using HITteamBot.Repository.Entities.Base;
using HITteamBot.Repository.Controllers.Base;

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
                    switch (message.Text.ToLower().Replace("/", "").Replace($"@{botClient.GetMeAsync().Result.FirstName.ToLower()}", "").Split(new char[] { ' ' })[0])
                    {
                        case "start":
                            Menu(botClient, message.Chat.Id, message.From.Username, cancellationToken);
                            return;
                        case "menu":
                            Menu(botClient, message.Chat.Id, message.From.Username, cancellationToken);
                            return;
                        case "character":
                            string charInfo = await CharactersController.GetCharacterInfo(message.From.Username);
                            if (charInfo.Contains("не найден"))
                            {
                                InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", MainMenu.NewCharacter.ToString()) });
                                _ = botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: charInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                            }
                            else
                                Play(botClient, message, charInfo, cancellationToken);
                            return;
                        case "создать":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.CreateNewCharacter(message.From.Username + message.Text.Replace("/создать", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")), Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            return;
                        case "атрибуты":
                            _ = botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: await CharactersController.SetCharacterAttributes(message.From.Username + message.Text.Replace("/атрибуты", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")), Telegram.Bot.Types.Enums.ParseMode.Markdown, cancellationToken: cancellationToken);
                            return;
                        case "аватар":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.SetCharacterAvatar(message.From.Username + message.Text.Replace("/аватар", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;


                        //case "таймер":
                        //    string[] query = message.Text.Replace("/таймер", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "").Trim().Split(new char[] { ' ' });
                        //    TimerCallback timerCallback = new TimerCallback(ActionsController.Action);
                        //    Events.Add(new EventsTimer()
                        //    {
                        //        Username = message.From.Username,
                        //        TimerName = query[0],
                        //        Timer = BaseController.SetTimer(timerCallback, string.Join(' ', query[3..]), Int32.Parse(query[1]) * 1000, Int32.Parse(query[2]) * 1000)
                        //    });

                        //    return;
                        //case "сбростаймер":
                        //    string[] query2 = message.Text.Replace("/сбростаймер", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "").Trim().Split(new char[] { ' ' });
                        //    Events.Where(s => s.TimerName == query2[0] && s.Username == message.From.Username).FirstOrDefault().Timer.Dispose();
                        //    Events.Remove(Events.Where(s => s.TimerName == query2[0] && s.Username == message.From.Username).FirstOrDefault());
                        //    return;

                        // Админская настройка
                        case "newperk":
                            if (BaseController.CheckPermissions(message.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await PerksController.AddNewPerk(message.Text.Replace("/newPerk", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;
                        case "neweffect":
                            if (BaseController.CheckPermissions(message.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await PerksController.AddNewEffect(message.Text.Replace("/newEffect", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;
                        case "addperk":
                            if (BaseController.CheckPermissions(message.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await PerksController.AddPerkToCharacter(message.Text.Replace("/addPerk", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;

                        case "newaction":
                            if (BaseController.CheckPermissions(message.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await ActionsController.AddNewAction(message.Text.Replace("/newAction", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;
                        case "addactionreward":
                            if (BaseController.CheckPermissions(message.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(message.Chat.Id, await ActionsController.AddRewardToAction(message.Text.Replace("/addActionReward", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;
                        default:
                            break;
                    }
                }

                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    string callback = Regex.Replace(update.CallbackQuery.Data, "[0-9]", "");
                    Chat chat = update.CallbackQuery.Message.Chat;
                    switch (callback)
                    {
                        case "NewCharacter":
                            await botClient.SendTextMessageAsync(chat.Id, "Придумайте персонажу имя, возраст и пол и начните сообщение с /создать.\r\n\r\n" +
                                                                            "Пример:\r\n/создать Глория 34 женский");
                            return;
                        case "Character":
                            string charInfo = await CharactersController.GetCharacterInfo(update.CallbackQuery.From.Username);
                            if (charInfo.Contains("не найден"))
                            {
                                InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", MainMenu.NewCharacter.ToString()) });
                                _ = botClient.SendTextMessageAsync(chatId: chat.Id, text: charInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                            }
                            else
                                Play(botClient, update.CallbackQuery.Message, charInfo, cancellationToken);
                            return;
                        case "CharacterSettings":
                            CharacterSettings(botClient, update.CallbackQuery.Message, cancellationToken);
                            return;
                        case "Avatar":
                            _ = botClient.SendTextMessageAsync(chat.Id, "Вы можете выбрать любой эмодзи в качестве аватара вашего персонажа. " +
                                                                        "Для этого начните сообщение с /аватар и пришлите любой понравившийся эмодзи.\r\n\r\n" +
                                                                        "Пример:\r\n/аватар 🧔🏻‍♀️");
                            return;
                        case "ActionsList":
                            ActionsListGet(botClient, update.CallbackQuery.Message, "Exploring", cancellationToken);
                            return;
                        case "StartAction":
                            await botClient.SendTextMessageAsync(chat.Id, update.CallbackQuery.Data.Replace("StartAction", ""));
                            return;





                        // Модератор + Администратор
                        case "Settings":
                            if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                Settings(botClient, chat, cancellationToken);
                            return;
                        case "NewPerk":
                            if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(chat.Id, "/newPerk название ключевой_аттрибут стоимость тип описание");
                            return;
                        case "NewAction":
                            if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                _ = botClient.SendTextMessageAsync(chat.Id, "/newAction название Exploring/Trading/Fight продолжительность_в_минутах");
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

        public static async void CreateDirectories()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);
                if (!Directory.Exists(AssetsDirectory)) Directory.CreateDirectory(AssetsDirectory);
                if (!Directory.Exists(ObjectsDirectory)) Directory.CreateDirectory(ObjectsDirectory);
                if (!Directory.Exists(UsersDirectory)) Directory.CreateDirectory(UsersDirectory);
                if (!Directory.Exists(PerksDirectory)) Directory.CreateDirectory(PerksDirectory);
                if (!Directory.Exists(ActionsDirectory)) Directory.CreateDirectory(ActionsDirectory);
                if (!Directory.Exists(ItemsDirectory)) Directory.CreateDirectory(ItemsDirectory);
            });
            await task;

            if (!System.IO.File.Exists(DataDirectory + $@"\Permissions.json"))
            {
                await BaseController.SetPermissions("Lizardrock Player");
                await BaseController.SetPermissions("Lizardrock Moderator");
                await BaseController.SetPermissions("Lizardrock Administrator");
            }
        }

        public static async void Menu(ITelegramBotClient botClient, long chatId, string username, CancellationToken cancellationToken)
        {
            try
            {
                string logo = AssetsDirectory + @"\vaultboy.png";
                if (System.IO.File.Exists(logo))
                {
                    using (var stream = System.IO.File.Open(logo, FileMode.Open))
                    {
                        await botClient.SendStickerAsync(chatId, stream);
                    }
                }

                InlineKeyboardMarkup inlineKeyboardWithPerms = new InlineKeyboardMarkup(new[]
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

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Мой персонаж", MainMenu.Character.ToString())
                    },
                    //new[]
                    //{
                    //    InlineKeyboardButton.WithCallbackData("Настройки (только с доступом)", MainMenu.Settings.ToString())
                    //}
                });

                await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Добро пожаловать в HIT_World!",
                            replyMarkup: (await BaseController.CheckPermissions(username, PermissionsType.Moderator) || await BaseController.CheckPermissions(username, PermissionsType.Administrator)) ? inlineKeyboardWithPerms : inlineKeyboard,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void Play(ITelegramBotClient botClient, Message message, string charInfo, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Занятия", $"{GameMenu.ActionsList}"),
                    InlineKeyboardButton.WithCallbackData("Плейсхолдер", "woop")
                },
                new[] {
                    InlineKeyboardButton.WithCallbackData("Найстройки персонажа", $"{GameMenu.CharacterSettings}")
                }
            });

            if (message.Text.ToLower().Replace("/", "").Replace($"@{botClient.GetMeAsync().Result.FirstName.ToLower()}", "") == "character") await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: charInfo,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);

            else await botClient.EditMessageTextAsync(
                        chatId: message.Chat.Id,
                        messageId: message.MessageId,
                        text: charInfo,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
        }

        public static async void ActionsListGet(ITelegramBotClient botClient, Message message, string type, CancellationToken cancellationToken)
        {
            List<InlineKeyboardButton> actions = new List<InlineKeyboardButton>();
            foreach (var action in ActionsController.GetActions(type))
            {
                actions.Add(InlineKeyboardButton.WithCallbackData($"{action.Name.Replace("_", " ")}", $"StartAction{(int)action.Type}"));
            }
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(actions);

            await botClient.EditMessageReplyMarkupAsync(
                        chatId: message.Chat.Id,
                        messageId: message.MessageId,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
        }

        public static async void CharacterSettings(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup newAvatar = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Новый аватар", MainMenu.Avatar.ToString()) });

            await botClient.EditMessageReplyMarkupAsync(
                        chatId: message.Chat.Id,
                        messageId: message.MessageId,
                        replyMarkup: newAvatar,
                        cancellationToken: cancellationToken);
        }

        public static async void Settings(ITelegramBotClient botClient, Chat chat, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Создать перк", SettingsMenu.NewPerk.ToString()),
                    InlineKeyboardButton.WithCallbackData("Создать воздействие", SettingsMenu.NewEffect.ToString())
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Создать действие", SettingsMenu.NewAction.ToString())
                }
            });

            await botClient.SendTextMessageAsync(
                        chatId: chat.Id,
                        text: "Настройки",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
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

    enum GameMenu
    {
        ActionsList,
        CharacterSettings
    }

    enum SettingsMenu
    {
        NewPerk,
        NewEffect,
        AddEffectToPerk,
        NewAction,
        AddRewardsToAction
    }
}
