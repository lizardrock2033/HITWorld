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
                                InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", $"{(int)MainShedule.MainMenu}_{MainMenu.NewCharacter}") });
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

                    switch (callback[0])
                    {
                        // MainShedule.MainMenu
                        // Создание и вызов персонажа
                        case "0":
                            switch (callback[1])
                            {
                                case "NewCharacter":
                                    await botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "Придумайте персонажу имя, возраст и пол и начните сообщение с /создать.\r\n\r\n" +
                                                                                    "Пример:\r\n/создать Глория 34 женский");
                                    return;
                                case "Character":
                                    string charInfo = await CharactersController.GetCharacterInfo(update.CallbackQuery.From.Username);
                                    if (charInfo.Contains("не найден"))
                                    {
                                        InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", $"{(int)MainShedule.MainMenu}_{MainMenu.NewCharacter}") });
                                        _ = botClient.SendTextMessageAsync(chatId: callbackMessage.Chat.Id, text: charInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                                    }
                                    else Play(botClient, callbackMessage, charInfo, cancellationToken);
                                    return;
                                case "CharacterSettings":
                                    CharacterSettings(botClient, callbackMessage, cancellationToken);
                                    return;
                                case "Avatar":
                                    _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "Вы можете выбрать любой эмодзи в качестве аватара вашего персонажа. " +
                                                                                "Для этого начните сообщение с /аватар и пришлите любой понравившийся эмодзи.\r\n\r\n" +
                                                                                "Пример:\r\n/аватар 🧔🏻‍♀️");
                                    return;
                                case "Back":
                                    string backToInfo = await CharactersController.GetCharacterInfo(update.CallbackQuery.From.Username);
                                    if (backToInfo.Contains("не найден"))
                                    {
                                        InlineKeyboardMarkup newCharacter = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", $"{(int)MainShedule.MainMenu}_{MainMenu.NewCharacter}") });
                                        _ = botClient.SendTextMessageAsync(chatId: callbackMessage.Chat.Id, text: backToInfo, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: newCharacter, cancellationToken: cancellationToken);
                                    }
                                    else Play(botClient, callbackMessage, backToInfo, cancellationToken);
                                    return;
                            }
                            return;

                        // MainShedule.GameMenu
                        // Геймплей
                        case "1":
                            switch (callback[1])
                            {
                                case "ActionsList":
                                    ActionsListGet(botClient, callbackMessage, ActionType.Exploring, cancellationToken);
                                    return;
                                case "ActionInfo":
                                    ActionInfo(botClient, callbackMessage, string.Join(' ', callback[2..]), cancellationToken);
                                    return;
                                case "StartAction":
                                    await botClient.SendTextMessageAsync(callbackMessage.Chat.Id, update.CallbackQuery.Data.Replace("StartAction", ""));
                                    return;
                                case "Back":
                                    ActionsListGet(botClient, callbackMessage, ActionType.Exploring, cancellationToken);
                                    return;
                            }
                            return;

                        // MainShedule.SettingsMenu
                        // Системные настройки и заведение новых данных (с правами доступа)
                        case "2":
                            switch (callback[1])
                            {
                                case "Settings":
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        Settings(botClient, callbackMessage, cancellationToken);
                                    return;
                                case "NewPerk":
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "/newPerk название ключевой_аттрибут стоимость тип описание");
                                    return;
                                case "NewEffect":
                                    if (BaseController.CheckPermissions(update.CallbackQuery.From.Username, PermissionsType.Moderator).Result)
                                        _ = botClient.SendTextMessageAsync(callbackMessage.Chat.Id, "/newEffect waitForIt");
                                    return;
                                case "NewAction":
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
                    using var stream = System.IO.File.Open(logo, FileMode.Open);
                    await botClient.SendStickerAsync(chatId, stream);
                }

                InlineKeyboardMarkup inlineKeyboardWithPerms = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Мой персонаж", $"{(int)MainShedule.MainMenu}_{MainMenu.Character}"),
                        InlineKeyboardButton.WithCallbackData("Инфо", MainMenu.Info.ToString())
                    },
                    new[] { InlineKeyboardButton.WithCallbackData("Настройки (только с доступом)", $"{(int)MainShedule.SettingsMenu}_{MainMenu.Settings}") }
                });

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Мой персонаж", $"{(int)MainShedule.MainMenu}_{MainMenu.Character}") });

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
                    InlineKeyboardButton.WithCallbackData("Задания", $"{(int)MainShedule.GameMenu}_{GameMenu.ActionsList}"),
                    InlineKeyboardButton.WithCallbackData("Сломать бота", "woop")
                },
                new[] { InlineKeyboardButton.WithCallbackData("Найстройки персонажа", $"{(int)MainShedule.MainMenu}_{GameMenu.CharacterSettings}") }
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

        public static async void ActionsListGet(ITelegramBotClient botClient, Message message, ActionType type, CancellationToken cancellationToken)
        {
            List<InlineKeyboardButton> actions = new List<InlineKeyboardButton>();
            foreach (var action in await ActionsController.GetActions(type))
            {
                actions.Add(InlineKeyboardButton.WithCallbackData($"{action.Name.Replace("_", " ")}", $"{(int)MainShedule.GameMenu}_{GameMenu.ActionInfo}_{(int)type}_{action.Name}"));
            }
            if (actions.Count() == 0) actions.Add(InlineKeyboardButton.WithCallbackData("Нечем заняться", $"{(int)MainShedule.MainMenu}_{MainMenu.Back}"));
            else actions.Add(InlineKeyboardButton.WithCallbackData("Назад", $"{(int)MainShedule.MainMenu}_{MainMenu.Back}"));

            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(actions);

            await botClient.EditMessageReplyMarkupAsync(
                        chatId: message.Chat.Id,
                        messageId: message.MessageId,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
        }

        public static async void ActionInfo(ITelegramBotClient botClient, Message message, string query, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Отправиться", $"{(int)MainShedule.GameMenu}_{GameMenu.StartAction}_"),
                InlineKeyboardButton.WithCallbackData("Назад", $"{(int)MainShedule.GameMenu}_{MainMenu.Back}")
            });
            await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: await ActionsController.GetActionInfo(query),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
        }

        public static async void StartAction(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            
        }

        public static async void CharacterSettings(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup newAvatar = new InlineKeyboardMarkup(new[] 
            {
                InlineKeyboardButton.WithCallbackData("Новый аватар", $"{(int)MainShedule.MainMenu}_{MainMenu.Avatar}"),
                InlineKeyboardButton.WithCallbackData("Назад", $"{(int)MainShedule.MainMenu}_{MainMenu.Back}")
            });

            await botClient.EditMessageReplyMarkupAsync(
                        chatId: message.Chat.Id,
                        messageId: message.MessageId,
                        replyMarkup: newAvatar,
                        cancellationToken: cancellationToken);
        }

        public static async void Settings(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Создать перк", $"{(int)MainShedule.SettingsMenu}_{SettingsMenu.NewPerk}"),
                    InlineKeyboardButton.WithCallbackData("Создать воздействие", $"{(int)MainShedule.SettingsMenu}_{SettingsMenu.NewEffect}")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Создать действие", $"{(int)MainShedule.SettingsMenu}_{SettingsMenu.NewAction}")
                }
            });

            await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Настройки",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: inlineKeyboard,
                        cancellationToken: cancellationToken);
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
        Settings,
        Info,
        Back
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
        NewPerk,
        NewEffect,
        AddEffectToPerk,
        NewAction,
        AddRewardsToAction
    }
}
