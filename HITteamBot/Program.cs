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
using HITteamBot.Repository.Entities.Actions;

namespace HITteamBot
{
    class Program
    {
        public static readonly string BaseDirectory = AppContext.BaseDirectory;
        public static readonly string DataDirectory = BaseDirectory + "Data";
        public static readonly string AssetsDirectory = DataDirectory + @"\Assets";
        public static readonly string HistoryDirectory = DataDirectory + @"\History";
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
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && !string.IsNullOrEmpty(message.Text) && DateTime.Now.ToUniversalTime() - message.Date < TimeSpan.FromMinutes(3) && message.Text[0] == '/')
                {
                    switch (message.Text.ToLower().Replace("/", "").Replace($"@{botClient.GetMeAsync().Result.FirstName.ToLower()}", "").Split(new char[] { ' ' })[0])
                    {
                        // Стартовая часть
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
                                InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", $"{(int)MainShedule.MainMenu}_{(int)MainMenu.NewCharacter}") });
                                _ = botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: charInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                            }
                            else Play(botClient, message, charInfo, cancellationToken);
                            return;

                        // Создание персонажа
                        case "создать":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.CreateNewCharacter(message.From.Username + message.Text.Replace("/создать", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")), Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            return;
                        case "атрибуты":
                            _ = botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: await CharactersController.SetCharacterAttributes(message.From.Username + message.Text.Replace("/атрибуты", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")), Telegram.Bot.Types.Enums.ParseMode.Markdown, cancellationToken: cancellationToken);
                            return;
                        case "аватар":
                            _ = botClient.SendTextMessageAsync(message.Chat.Id, await CharactersController.SetCharacterAvatar(message.From.Username + message.Text.Replace("/аватар", "").Replace($"@{botClient.GetMeAsync().Result.FirstName}", "")));
                            return;

                        // Настройка и создание элементов (с правами доступа)
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
                    Message callbackMessage = update.CallbackQuery.Message;
                    string[] callback = update.CallbackQuery.Data.Split(new char[] { '_' });

                    switch ((MainShedule)Enum.Parse(typeof(MainShedule), callback[0]))
                    {
                        // Создание и вызов персонажа
                        case MainShedule.MainMenu:
                            switch ((MainMenu)Enum.Parse(typeof(MainMenu), callback[1]))
                            {
                                case MainMenu.NewCharacter:
                                    await botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "Придумайте персонажу имя, возраст и пол и начните сообщение с /создать.\r\n\r\n" +
                                                                                    "Пример:\r\n/создать Глория 34 женский");
                                    return;
                                case MainMenu.Character:
                                    string charInfo = await CharactersController.GetCharacterInfo(update.CallbackQuery.From.Username);
                                    if (charInfo.Contains("не найден"))
                                    {
                                        InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", $"{(int)MainShedule.MainMenu}_{(int)MainMenu.NewCharacter}") });
                                        _ = botClient.SendTextMessageAsync(chatId: callbackMessage.Chat.Id, text: charInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                                    }
                                    else Play(botClient, callbackMessage, charInfo, cancellationToken);
                                    return;
                                case MainMenu.Avatar:
                                    _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "Вы можете выбрать любой эмодзи в качестве аватара вашего персонажа. " +
                                                                                "Для этого начните сообщение с /аватар и пришлите любой понравившийся эмодзи.\r\n\r\n" +
                                                                                "Пример:\r\n/аватар 🧔🏻‍♀️");
                                    return;
                                default:
                                    string backToInfo = await CharactersController.GetCharacterInfo(update.CallbackQuery.From.Username);
                                    if (backToInfo.Contains("не найден"))
                                    {
                                        InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", $"{(int)MainShedule.MainMenu}_{(int)MainMenu.NewCharacter}") });
                                        _ = botClient.SendTextMessageAsync(chatId: callbackMessage.Chat.Id, text: backToInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                                    }
                                    else Play(botClient, callbackMessage, backToInfo, cancellationToken);
                                    return;
                            }

                        // Геймплей
                        case MainShedule.GameMenu:
                            switch ((GameMenu)Enum.Parse(typeof(GameMenu), callback[1]))
                            {
                                case GameMenu.ActionsList:
                                    ActionsListGet(botClient, callbackMessage, ActionType.Exploring, cancellationToken);
                                    return;
                                case GameMenu.ActionInfo:
                                    ActionInfo(botClient, callbackMessage, string.Join(' ', callback[2..]), cancellationToken);
                                    return;
                                case GameMenu.StartAction:
                                    StartAction(botClient, update.CallbackQuery, update.CallbackQuery.Data.Replace("1_2_", ""), cancellationToken);
                                    return;
                                case GameMenu.CharacterSettings:
                                    CharacterSettings(botClient, callbackMessage, cancellationToken);
                                    return;
                                default:
                                    await botClient.DeleteMessageAsync(callbackMessage.Chat.Id, callbackMessage.MessageId);
                                    Message msg = callbackMessage;
                                    msg.MessageId--;
                                    ActionsListGet(botClient, msg, ActionType.Exploring, cancellationToken);
                                    return;
                            }

                        // Системные настройки и заведение новых данных (с правами доступа)
                        case MainShedule.SettingsMenu:
                            switch ((SettingsMenu)Enum.Parse(typeof(SettingsMenu), callback[1]))
                            {
                                case SettingsMenu.Settings:
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        Settings(botClient, callbackMessage, cancellationToken);
                                    return;
                                case SettingsMenu.NewPerk:
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "/newPerk название ключевой_аттрибут стоимость тип описание");
                                    return;
                                case SettingsMenu.NewEffect:
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "/newEffect waitForIt", Telegram.Bot.Types.Enums.ParseMode.Markdown);
                                    return;
                                case SettingsMenu.NewAction:
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "/newAction название Exploring/Trading/Fight продолжительность_в_минутах");
                                    return;
                            }
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
            try
            {
                Task task = Task.Factory.StartNew(() =>
                {
                    if (!Directory.Exists(DataDirectory)) Directory.CreateDirectory(DataDirectory);
                    if (!Directory.Exists(AssetsDirectory)) Directory.CreateDirectory(AssetsDirectory);
                    if (!Directory.Exists(HistoryDirectory)) Directory.CreateDirectory(HistoryDirectory);
                    if (!Directory.Exists(ObjectsDirectory)) Directory.CreateDirectory(ObjectsDirectory);
                    if (!Directory.Exists(UsersDirectory)) Directory.CreateDirectory(UsersDirectory);
                    if (!Directory.Exists(PerksDirectory)) Directory.CreateDirectory(PerksDirectory);
                    if (!Directory.Exists(ActionsDirectory)) Directory.CreateDirectory(ActionsDirectory);
                    if (!Directory.Exists(ItemsDirectory)) Directory.CreateDirectory(ItemsDirectory);
                });
                await task;

                if (!System.IO.File.Exists(ObjectsDirectory + $@"\Permissions.json"))
                {
                    await BaseController.SetPermissions("Lizardrock Player");
                    await BaseController.SetPermissions("Lizardrock Moderator");
                    await BaseController.SetPermissions("Lizardrock Administrator");
                }
            }
            catch (Exception)
            {

            }
        }

        public static async void Menu(ITelegramBotClient botClient, long chatId, string username, CancellationToken cancellationToken)
        {
            try
            {
                string logo = AssetsDirectory + @"\vaultboy.png";
                if (System.IO.File.Exists(logo))
                {
                    using var stream = System.IO.File.Open(logo, FileMode.Open);
                    await botClient.SendStickerAsync(chatId, stream);
                }

                InlineKeyboardMarkup inlineKeyboardWithPerms = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Мой персонаж", $"{(int)MainShedule.MainMenu}_{(int)MainMenu.Character}"),
                        InlineKeyboardButton.WithCallbackData("Инфо", MainMenu.Info.ToString())
                    },
                    new[] { InlineKeyboardButton.WithCallbackData("Настройки (только с доступом)", $"{(int)MainShedule.SettingsMenu}_{(int)SettingsMenu.Settings}") }
                });

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Мой персонаж", $"{(int)MainShedule.MainMenu}_{(int)MainMenu.Character}") });

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
            try
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Ежедневные задания", $"{(int)MainShedule.GameMenu}_{(int)GameMenu.ActionsList}"),
                    InlineKeyboardButton.WithCallbackData("Сломать бота", "woop")
                },
                new[] { InlineKeyboardButton.WithCallbackData("Найстройки персонажа", $"{(int)MainShedule.GameMenu}_{(int)GameMenu.CharacterSettings}") }
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
            catch (Exception)
            {

            }
        }

        public static async void ActionsListGet(ITelegramBotClient botClient, Message message, ActionType type, CancellationToken cancellationToken)
        {
            try
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(await ActionsController.GetActionsInButtons(type));
                await botClient.EditMessageReplyMarkupAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void ActionInfo(ITelegramBotClient botClient, Message message, string query, CancellationToken cancellationToken)
        {
            string[] data = query.Trim().Split(new char[] { ' ' });
            try
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Отправиться", $"{(int)MainShedule.GameMenu}_{(int)GameMenu.StartAction}_{data[0]}_{string.Join('_', data[1..])}"),
                InlineKeyboardButton.WithCallbackData("Назад", $"{(int)MainShedule.GameMenu}_66")
            });
                await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: await ActionsController.GetActionInfo(query),
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void StartAction(ITelegramBotClient botClient, CallbackQuery callbackQuery, string query, CancellationToken cancellationToken)
        {
            try
            {
                string path = await ActionsController.GetActionPath(query);
                Repository.Entities.Actions.Action action = await ActionsController.GetAction(path);
                TimerCallback timerCallback = new TimerCallback(Notify);
                NotifyData notifyData = new NotifyData()
                {
                    BotClient = botClient,
                    Chat = callbackQuery.Message.Chat,
                    Username = callbackQuery.From.Username,
                    Message = $"[{callbackQuery.From.FirstName}](tg://user?id={callbackQuery.From.Id}), ваш персонаж закончил задание!",
                    CancellationToken = cancellationToken
                };

                EventsTimer timer = new EventsTimer()
                {
                    Username = callbackQuery.From.Username,
                    TimerName = action.Name,
                    Timer = BaseController.SetTimer(timerCallback, notifyData, action.DurationInMinutes)
                };

                notifyData.Timer = timer;
                Events.Add(timer);

                await botClient.SendTextMessageAsync(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: await ActionsController.StartAction(callbackQuery.From.Username, path),
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void Notify(object obj)
        {
            try
            {
                NotifyData notifyData = (NotifyData)obj;
                BaseController.RemoveTimer(notifyData.Timer);
                await notifyData.BotClient.SendTextMessageAsync(
                                chatId: notifyData.Chat.Id,
                                text: notifyData.Message,
                                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                                cancellationToken: notifyData.CancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void CharacterSettings(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            try
            {
                InlineKeyboardMarkup newAvatar = new InlineKeyboardMarkup(new[] {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Новый аватар", $"{(int)MainShedule.MainMenu}_{(int)MainMenu.Avatar}")
                },
                new[] { InlineKeyboardButton.WithCallbackData("Назад", $"{(int)MainShedule.MainMenu}_66") }});

                await botClient.EditMessageReplyMarkupAsync(
                            chatId: message.Chat.Id,
                            messageId: message.MessageId,
                            replyMarkup: newAvatar,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void Settings(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            try
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Создать перк", $"{(int)MainShedule.SettingsMenu}_{(int)SettingsMenu.NewPerk}"),
                        InlineKeyboardButton.WithCallbackData("Создать воздействие", $"{(int)MainShedule.SettingsMenu}_{(int)SettingsMenu.NewEffect}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Создать действие", $"{(int)MainShedule.SettingsMenu}_{(int)SettingsMenu.NewAction}")
                    }
                });

                await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Настройки",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: inlineKeyboard,
                            cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception.Message));
            await Task.Delay(190000);
            Main(null);
        }
    }

    enum MainShedule
    {
        MainMenu,
        GameMenu,
        SettingsMenu
    }

    enum MainMenu
    {
        NewCharacter,
        Character,
        Avatar,
        Info
    }

    enum GameMenu
    {
        ActionsList,
        ActionInfo,
        StartAction,
        CharacterSettings
    }

    enum SettingsMenu
    {
        Settings,
        NewPerk,
        NewEffect,
        AddEffectToPerk,
        NewAction,
        AddRewardsToAction
    }
}
