using HITteamBot.Repository.Domain.DatabaseEntities.Characters;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Chemicals;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Ammo;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Junk;
using HITteamBot.Repository.Entities.Locations;
using HITteamBot.Repository.Entities.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using HITteamBot.Repository.Entities.Actions;
using HITteamBot.Repository.Domain;
using HITteamBot.Repository.Entities;
using HITteamBot.Repository.Domain.Helpers.Characters;
using HITteamBot.Repository.Entities.Characters;
using System.Reflection;

namespace HITteamBot.Repository.Controllers.Characters
{
    public class CharactersController
    {
        #region Создание персонажа
        public static async void CreateNewCharacter(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, List<CharacterData> CharacterDatas)
        {
            try
            {
                _ = botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Так, _{message.From.FirstName}_, сейчас проверю, есть ли у нас свободная койка для еще одного поселенца. Скоро вернусь.",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);

                RequestData<Message> requestData = new RequestData<Message>() { Data = message };
                ResponseData<Character> responseData = await SaveNewCharacter(requestData);
                Character createdCharacter = new Character();
                if (!responseData.IsError) createdCharacter = responseData.Data;
                else throw new Exception(responseData.ErrorText);

                CharacterData characterData = new CharacterData()
                {
                    UserId = message.From.Id,
                    ChatId = message.Chat.Id,
                    CharacterId = createdCharacter.Id,
                    SkillPoints = 22,
                    SPECIAL = new SPECIAL()
                    {
                        Id = createdCharacter.SPECIALsId,
                        Strength = 1,
                        Perception = 1,
                        Endurance = 1,
                        Charisma = 1,
                        Intellegence = 1,
                        Agility = 1,
                        Luck = 1,
                        IsSet = false
                    }
                };
                CharacterDatas.Add(characterData);

                string text = $"Нашел свободное местечко. Так что добро пожаловать в Сэнкчуари Хиллз {Emoji.Settlement}!\r\n" +
                                    $"Но прежде чем присоединиться к поселению, [{message.From.FirstName}](tg://user?id={message.From.Id}), заполни анктету, чтобы я мог понять будет ли нам польза от твоих навыков.\r\n\r\n" +
                                    $"{Emoji.Rosette} Очков:  _{characterData.SkillPoints}_";

                InlineKeyboardMarkup inlineKeyboard = GetInlineStats(characterData);

                await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: text,
                        replyMarkup: inlineKeyboard,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: ex.Message,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
        }

        public static async void ChangeStats(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken, SPECIALs stat, BasicCommands act, List<CharacterData> CharacterDatas)
        {
            try
            {
                CharacterData characterData = CharacterDatas.Where(m => m.ChatId == callbackQuery.Message.Chat.Id && m.UserId == callbackQuery.From.Id).FirstOrDefault();
                if (characterData == null) throw new Exception("Кажется, я где то потерял твою анкету, приятель...");
                if (characterData.SkillPoints <= 0 && act == BasicCommands.Add) return;
                PropertyInfo field = typeof(SPECIAL).GetProperty(stat.ToString());
                short value = (short)field.GetValue(characterData.SPECIAL);
                if (value + (short)(act == BasicCommands.Add ? 1 : -1) > 10 || value + (short)(act == BasicCommands.Add ? 1 : -1) <= 0) return;
                value += (short)(act == BasicCommands.Add ? 1 : -1);
                field.SetValue(characterData.SPECIAL, value);
                characterData.SkillPoints += (short)(act == BasicCommands.Add ? -1 : 1);

                string text = $"Нашел свободное местечко. Так что добро пожаловать в Сэнкчуари Хиллз {Emoji.Settlement}!\r\n" +
                                    $"Но прежде чем присоединиться к поселению, [{callbackQuery.From.FirstName}](tg://user?id={callbackQuery.From.Id}), заполни анктету, чтобы я мог понять будет ли нам польза от твоих навыков.\r\n\r\n" +
                                    $"{Emoji.Rosette} Очков:  _{characterData.SkillPoints}_";

                await botClient.EditMessageTextAsync(
                            chatId: callbackQuery.Message.Chat.Id,
                            messageId: callbackQuery.Message.MessageId,
                            text: text,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            replyMarkup: GetInlineStats(characterData),
                            cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: ex.Message,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
        }

        public static async void SaveStats(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken, List<CharacterData> CharacterDatas)
        {
            try
            {
                CharacterData characterData = CharacterDatas.Where(m => m.ChatId == callbackQuery.Message.Chat.Id && m.UserId == callbackQuery.From.Id).FirstOrDefault();
                if (characterData == null) throw new Exception("Кажется, я где то потерял твою анкету, приятель...");
                if (characterData.SkillPoints != 0) throw new Exception($"Хэй, [{callbackQuery.From.FirstName}](tg://user?id={callbackQuery.From.Id}), заполни анкету до конца, я уверен ты способен на большее.");
                characterData.SPECIAL.IsSet = true;

                RequestData<SPECIAL> requestData = new RequestData<SPECIAL>() { Data = characterData.SPECIAL };
                ResponseData<int> responseData = await SaveCharacterStats(requestData);
                if (responseData.IsError) throw new Exception(responseData.ErrorText);

                await botClient.SendTextMessageAsync(
                            chatId: callbackQuery.Message.Chat.Id,
                            text: $"Отлично, [{callbackQuery.From.FirstName}](tg://user?id={callbackQuery.From.Id})! На этом всё. Хотя постой, забыл самое главное...\r\n" +
                                    $"Как мне тебя называть? Здесь у тебя начнется новая жизнь, неплохо было бы подобрать новое имя.",
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                            cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: ex.Message,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
        }

        public static async void GetStatsInfo(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            try
            {
                await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: $"Описание характеристик:\r\n" +
                                $"{Emoji.Muscle} *Сила (Strength)* - переносимый вес, урон тяжелого оружия и урон в ближнем бою.\r\n" +
                                $"{Emoji.Eye} *Восприятие (Perception)* - меткость и внимательность.\r\n" +
                                $"{Emoji.Lungs} *Выносливость (Endurance)* - здоровье и стойкость.\r\n" +
                                $"{Emoji.SpeakingHead} *Харизма (Charisma)* - торговля и общение.\r\n" +
                                $"{Emoji.Brain} *Интеллект (Intellegence)* - модификация оружия, получаемый опыт и крафт.\r\n" +
                                $"{Emoji.Leg} *Ловкость (Agility)* - урон в дальнем бою, скрытность и очки действия (ОД).\r\n" +
                                $"{Emoji.Clover} *Удача (Luck)* - криты, находимый хлам, везение.",
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: ex.Message,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
        }

        public static InlineKeyboardMarkup GetInlineStats(CharacterData characterData)
        {
            return new InlineKeyboardMarkup(new[] {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Strength}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.Muscle} СИЛ: {characterData.SPECIAL.Strength}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Strength}_{(int)BasicCommands.Add}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Perception}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.Eye} ВСП: {characterData.SPECIAL.Perception}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Perception}_{(int)BasicCommands.Add}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Endurance}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.Lungs} ВЫН: {characterData.SPECIAL.Endurance}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Endurance}_{(int)BasicCommands.Add}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Charisma}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.SpeakingHead} ХАР: {characterData.SPECIAL.Charisma}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Charisma}_{(int)BasicCommands.Add}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Intellegence}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.Brain} ИНТ: {characterData.SPECIAL.Intellegence}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Intellegence}_{(int)BasicCommands.Add}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Agility}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.Leg} ЛВК: {characterData.SPECIAL.Agility}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Agility}_{(int)BasicCommands.Add}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("-", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Luck}_{(int)BasicCommands.Deny}"),
                        InlineKeyboardButton.WithCallbackData($"{Emoji.Clover} УДЧ: {characterData.SPECIAL.Luck}", "empty"),
                        InlineKeyboardButton.WithCallbackData("+", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)SPECIALs.Luck}_{(int)BasicCommands.Add}")
                    },
                    new[] { InlineKeyboardButton.WithCallbackData($"{Emoji.Check} Принять", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)BasicCommands.Save}") },
                    new[] { InlineKeyboardButton.WithCallbackData($"{Emoji.Info} О характеристиках", $"{characterData.UserId}_{(int)MainShedule.GameMenu}_{(int)GameMenu.Statistics}_{(int)BasicCommands.GetInfo}") }
            });
        }

        #endregion

        #region Controller => Helper
        #region Создание персонажа
        private static async Task<ResponseData<Character>> SaveNewCharacter(RequestData<Message> requestData)
        {
            ResponseData<Character> responseData = new ResponseData<Character>();
            try
            {
                Message message = requestData.Data;
                Users user = new Users()
                {
                    ChatId = message.Chat.Id,
                    UserId = message.From.Id,
                    Username = message.From.Username
                };
                RequestData<Users> requestForNewCharacter = new RequestData<Users>() { Data = user };
                ResponseData<Character> responseFromCreating = await CharactersHelper.CreateNewCharacter(requestForNewCharacter);

                if (!responseFromCreating.IsError) responseData.Data = responseFromCreating.Data;
                else throw new Exception(responseFromCreating.ErrorText);
            }
            catch (Exception ex)
            {
                responseData.IsError = true;
                responseData.ErrorText = ex.Message;
            }
            return responseData;
        }

        private static async Task<ResponseData<int>> SaveCharacterStats(RequestData<SPECIAL> requestData)
        {
            ResponseData<int> responseData = new ResponseData<int>();
            try
            {
                RequestData<SPECIAL> requestForStatsSaving = new RequestData<SPECIAL>() { Data = requestData.Data };
                ResponseData<int> responseFromStatsSaving = await CharactersHelper.SetCharacterStats(requestData);
            }
            catch (Exception ex)
            {
                responseData.IsError = true;
                responseData.ErrorText = ex.Message;
            }
            return responseData;
        }

        public static async void SaveCharacterName(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken, List<CharacterData> CharacterDatas)
        {
            try
            {
                CharacterData characterData = CharacterDatas.Where(m => m.ChatId == callbackQuery.Message.Chat.Id && m.UserId == callbackQuery.From.Id).FirstOrDefault();
                RequestData<CharacterData> requestData = new RequestData<CharacterData>() { Data = characterData };
                ResponseData<int> responseData = await CharactersHelper.SetCharacterName(requestData);
                if (responseData.IsError) throw new Exception(responseData.ErrorText);

                string text = $"Ну что ж, [{characterData.CharacterName}](tg://user?id={callbackQuery.From.Id}), приветствую тебя в Сэнкчуари, мой друг! Удачи в приключениях! " +
                                    $"Как освоишься тут, мне потребуется твоя помощь с одним поселением.";
                CharacterDatas.Remove(characterData);

                await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: text,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: ex.Message,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        cancellationToken: cancellationToken);
            }
        }
        #endregion

        #endregion

        //#region Настройки персонажа
        //public static async Task<string> SetCharacterAvatar(string query)
        //{
        //    try
        //    {
        //        string[] strings = query.Trim().Split(new char[] { ' ' });
        //        string userCharacterDirectory = BaseController.GetUserDirectory(strings[0]) + $"\\Character";
        //        Character character = await GetCharacter(strings[0]);
        //        if (character.IsActive)
        //        {
        //            character.Avatar = strings[1];

        //            bool isOK = false;
        //            Task task = Task.Factory.StartNew(() => { isOK = SaveCharacter(character); });
        //            await task;
        //            if (isOK) return "Аватар сохранён";
        //            return "Ошибка";
        //        }
        //        else return "Персонаж не найден";
        //    }
        //    catch (Exception)
        //    {
        //        return "Ошибка";
        //    }
        //}

        //#endregion

        //#region Получение информации о персонаже
        //public static async Task<string> GetCharacterFullInfo(string username)
        //{
        //    string info = "Хэй, я тебя раньше не видел.";
        //    try
        //    {
        //        string userCharacterDirectory = BaseController.GetUserDirectory(username) + $"\\Character";
        //        if (Directory.Exists(userCharacterDirectory))
        //        {
        //            Character character = await GetCharacter(username);
        //            info = $"{(!string.IsNullOrEmpty(character.Avatar) ? character.Avatar : Emoji.Incognito)} *Имя:*   _{character.Name}_\r\n" +
        //                        $"{Emoji.DNA} *Возраст:*   {character.Age}\r\n" +
        //                        $"{(character.Gender.ToLower().Contains("женский") ? Emoji.WomanSign : Emoji.MenSign)} *Пол:*   _{character.Gender}_\r\n" +
        //                        $"{Emoji.Star} *Уровень:*   _{character.Level} ({character.Characteristics.Experience}/{character.Characteristics.NextLevelOn})_\r\n\r\n" +
        //                        $"{Emoji.Pager} *Характеристики:*\r\n\r\n" +
        //                        $"{Emoji.Heart} *HP:*   _{character.Characteristics.CurrentHealth}/{character.Characteristics.Health}_\r\n" +
        //                        $"{Emoji.Lightning} *AP:*   _{character.Characteristics.CurrentAP}/{character.Characteristics.ActionPoints}_\r\n" +
        //                        $"{Emoji.WeightLifter} *Лимит веса:*   _{character.Characteristics.CurrentWL}/{character.Characteristics.WeightLimit}_\r\n" +
        //                        $"{Emoji.RadioactiveSign} *Радиация:*   _{Dictionaries.GetRadContamination(character.Characteristics.RadContamination)} ({character.Characteristics.Rads})_\r\n\r\n" +
        //                        $"{Emoji.Muscle} *Сила:*   _{character.Characteristics.Attributes.Strength}_\r\n" +
        //                        $"{Emoji.Eye} *Восприятие:*   _{character.Characteristics.Attributes.Perception}_\r\n" +
        //                        $"{Emoji.Lungs} *Выносливость:*   _{character.Characteristics.Attributes.Endurance}_\r\n" +
        //                        $"{Emoji.SpeakingHead} *Харизма:*   _{character.Characteristics.Attributes.Charisma}_\r\n" +
        //                        $"{Emoji.Brain} *Интеллект:*   _{character.Characteristics.Attributes.Intellegence}_\r\n" +
        //                        $"{Emoji.Leg} *Ловкость:*   _{character.Characteristics.Attributes.Agility}_\r\n" +
        //                        $"{Emoji.Clover} *Удача:*   _{character.Characteristics.Attributes.Luck}_\r\n";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        info = ex.Message;
        //    }
        //    return info;
        //}

        //public static async Task<string> GetCharacterAttributesInfo(string username)
        //{
        //    string info;
        //    try
        //    {
        //        Character character = await GetCharacter(username);
        //        info = $"{(!string.IsNullOrEmpty(character.Avatar) ? character.Avatar : Emoji.Incognito)} _{character.Name}_\r\n" +
        //                        $"{Emoji.Pager} *Характеристики:*\r\n\r\n" +
        //                        $"{Emoji.Star} *Уровень:*   _{character.Level} ({character.Characteristics.Experience}/{character.Characteristics.NextLevelOn})_\r\n" +
        //                        $"{Emoji.Heart} *HP:*   _{character.Characteristics.CurrentHealth}/{character.Characteristics.Health}_\r\n" +
        //                        $"{Emoji.Lightning} *AP:*   _{character.Characteristics.CurrentAP}/{character.Characteristics.ActionPoints}_\r\n\r\n" +
        //                        $"{Emoji.Muscle} *Сила:*   _{character.Characteristics.Attributes.Strength}_\r\n" +
        //                        $"{Emoji.Eye} *Восприятие:*   _{character.Characteristics.Attributes.Perception}_\r\n" +
        //                        $"{Emoji.Lungs} *Выносливость:*   _{character.Characteristics.Attributes.Endurance}_\r\n" +
        //                        $"{Emoji.SpeakingHead} *Харизма:*   _{character.Characteristics.Attributes.Charisma}_\r\n" +
        //                        $"{Emoji.Brain} *Интеллект:*   _{character.Characteristics.Attributes.Intellegence}_\r\n" +
        //                        $"{Emoji.Leg} *Ловкость:*   _{character.Characteristics.Attributes.Agility}_\r\n" +
        //                        $"{Emoji.Clover} *Удача:*   _{character.Characteristics.Attributes.Luck}_\r\n";

        //        using (DatabaseContext dbContext = new DatabaseContext())
        //        {
        //            dbContext.SPECIAL.Add(character.Characteristics.Attributes);
        //            dbContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        info = ex.Message;
        //    }
        //    return info;
        //}

        //public static async Task<string> GetCharacterStateInfo(string username)
        //{
        //    string info;
        //    try
        //    {
        //        Character character = await GetCharacter(username);
        //        if (character == null || string.IsNullOrEmpty(character?.User)) throw new Exception("не найден");
        //        if (character.LifeState == LifeStates.Unconscious)
        //        {
        //            info = $"{(!string.IsNullOrEmpty(character.Avatar) ? character.Avatar : Emoji.Incognito)} _{character.Name}_\r\n" +
        //                            $"{Emoji.Star} *Уровень:*   _{character.Level} ({character.Characteristics.Experience}/{character.Characteristics.NextLevelOn})_\r\n" +
        //                            $"{Emoji.Heart} *HP:*   _{character.Characteristics.CurrentHealth}/{character.Characteristics.Health}  {Emoji.Exclamation} Без сознания {Emoji.Exclamation}_\r\n" +
        //                            $"{Emoji.Lightning} *AP:*   _{character.Characteristics.CurrentAP}/{character.Characteristics.ActionPoints}_\r\n" +
        //                            $"{Emoji.RadioactiveSign} *РАД:*   _{Dictionaries.GetRadContamination(character.Characteristics.RadContamination)} ({character.Characteristics.Rads})_\r\n" +
        //                            $"{Emoji.Clipboard} _Неизвестно..._";
        //        }
        //        else
        //        {
        //            ActionHistory action = await ActionsController.CurrentAction(username);
        //            string where = action == null || string.IsNullOrEmpty(action.Username) ? "В поселении" : $"{action.ActionName.Replace("_", " ")}, закончит в {action.FinishDate.ToString("HH:mm")}";
        //            info = $"{(!string.IsNullOrEmpty(character.Avatar) ? character.Avatar : Emoji.Incognito)} _{character.Name}_\r\n" +
        //                            $"{Emoji.Star} *Уровень:*   _{character.Level} ({character.Characteristics.Experience}/{character.Characteristics.NextLevelOn})_\r\n" +
        //                            $"{Emoji.Heart} *HP:*   _{character.Characteristics.CurrentHealth}/{character.Characteristics.Health}_\r\n" +
        //                            $"{Emoji.Lightning} *AP:*   _{character.Characteristics.CurrentAP}/{character.Characteristics.ActionPoints}_\r\n" +
        //                            $"{Emoji.RadioactiveSign} *РАД:*   _{Dictionaries.GetRadContamination(character.Characteristics.RadContamination)} ({character.Characteristics.Rads})_\r\n" +
        //                            $"{Emoji.Clipboard} _{where}_";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        info = ex.Message;
        //    }
        //    return info;
        //}

        //#endregion

        //#region Действия с персонажем
        //public static async Task<string> UseChemical(string username, ChemicalsInfo chemical)
        //{
        //    string response = "";
        //    try
        //    {
        //        Character character = await GetCharacter(username);
        //        await Task.Factory.StartNew(() =>
        //        {
        //            switch (chemical)
        //            {
        //                case ChemicalsInfo.Stimpack:
        //                    if (character.Inventory.Chemicals.Stimpacks.Count > 0)
        //                    {
        //                        character.Characteristics.CurrentHealth =
        //                            character.Characteristics.CurrentHealth + character.Inventory.Chemicals.Stimpacks.Effect.Power > character.Characteristics.Health ?
        //                            character.Characteristics.Health :
        //                            character.Characteristics.CurrentHealth + character.Inventory.Chemicals.Stimpacks.Effect.Power;

        //                        character.Inventory.Chemicals.Stimpacks.Count--;
        //                        response = $"_{character.Name}_ использовал стимулятор.\r\n" +
        //                                    $"{Emoji.Heart} Здоровье:   _{character.Characteristics.CurrentHealth}/{character.Characteristics.Health}_\r\n" +
        //                                    $"Осталось стимуляторов:   _{character.Inventory.Chemicals.Stimpacks.Count}_";
        //                    }
        //                    else throw new Exception($"_{character.Name}_, у тебя закончились стимуляторы.");
        //                    break;
        //                case ChemicalsInfo.Buffout:
        //                    break;
        //                case ChemicalsInfo.Mentats:
        //                    break;
        //                case ChemicalsInfo.Psyho:
        //                    break;
        //                case ChemicalsInfo.MedX:
        //                    break;
        //                case ChemicalsInfo.RadAway:
        //                    break;
        //                case ChemicalsInfo.RadX:
        //                    break;
        //                default:
        //                    break;
        //            }
        //        });
        //        SaveCharacter(character);
        //    }
        //    catch (Exception ex)
        //    {
        //        response = ex.Message;
        //    }
        //    return response;
        //}

        //public static async Task<Character> DamageCharacter(string username, int damage, short rads)
        //{
        //    Character character = new Character();
        //    try
        //    {
        //        character = await GetCharacter(username);
        //        character.Characteristics.CurrentHealth = character.Characteristics.CurrentHealth - damage < 0 ? 0 : character.Characteristics.CurrentHealth - damage;
        //        character.Characteristics.Rads += rads;

        //        if (character.Characteristics.CurrentHealth == 0 || character.Characteristics.Rads >= 1000) await RenderUnconscious(character);
        //        SaveCharacter(character);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return character;
        //}

        //public static async Task<Character> RenderUnconscious(Character character)
        //{
        //    try
        //    {
        //        character.Characteristics.CurrentHealth = 0;
        //        character.LifeState = LifeStates.Unconscious;
        //        character.Activity = Activities.Sliping;
        //        character.CurrentLocationType = LocationTypes.Unknown;
        //        await ActionsController.DropCurrentAction(character.User);
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return character;
        //}

        //public static async Task<string> RadAwayBlessing()
        //{
        //    string response = "";
        //    try
        //    {
        //        await Task.Factory.StartNew(() =>
        //        {
        //            foreach (var user in Directory.GetDirectories(Program.UsersDirectory))
        //            {
        //                DirectoryInfo dir = new DirectoryInfo(user);
        //                Character character = GetCharacter(dir.Name).Result;
        //                character.Characteristics.Rads = 0;
        //                character.Characteristics.RadContamination = RadContamination.Clear;
        //                SaveCharacter(character);
        //            }
        //        });
        //        response = $"Внимание, жители Сэнкчуари Хиллз!\r\n" +
        //            $"Как вы могли заметить, у воды сегодня странный вкус. Не волнуйтесь, Мэтт случайно опрокинул в водоочиститель ведро антирадина.\r\n" +
        //            $"Думаю, эта случайная оплошность даже поправила ваше здоровье, излечив от радиации.";
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return response;
        //}
        //#endregion

        //#region Получение/сохранение персонажа

        //public static async Task<Character> GetCharacter(string username)
        //{
        //    List<Character> charactersList = new List<Character>();
        //    Character character = new Character();
        //    try
        //    {
        //        string characterPath = BaseController.GetUserDirectory(username) + $@"\Character";
        //        if (Directory.Exists(characterPath))
        //        {
        //            await Task.Factory.StartNew(() =>
        //            {
        //                foreach (var ch in Directory.GetFiles(characterPath))
        //                {
        //                    charactersList.Add(JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(ch)));
        //                }
        //                character = charactersList.Where(i => i.IsActive).FirstOrDefault() ?? new Character();
        //            });
        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }
        //    return character;
        //}
    }
}
