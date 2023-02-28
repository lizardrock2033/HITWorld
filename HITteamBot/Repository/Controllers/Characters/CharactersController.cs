using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Items.Equipment;
using HITteamBot.Repository.Entities.Items.Chemicals;
using HITteamBot.Repository.Entities.Items.Ammo;
using HITteamBot.Repository.Entities.Items.Junk;
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

namespace HITteamBot.Repository.Controllers.Characters
{
    public class CharactersController
    {
        #region Создание персонажа
        public static async Task<ResponseData<Character>> CreateNewCharacter(RequestData<Message> requestData)
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
                ResponseData<Character> responseFromCreating = new ResponseData<Character>();
                await Task.Factory.StartNew(() => { responseFromCreating = CharactersHelper.CreateNewCharacter(requestForNewCharacter); });

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

        //public static async Task<string> SetCharacterAttributes(string query)
        //{
        //    try
        //    {
        //        string[] strings = query.Trim().Split(new char[] { ' ' });
        //        string userCharacterDirectory = BaseController.GetUserDirectory(strings[0]) + $"\\Character";
        //        Character character = await GetCharacter(strings[0]);
        //        if (character.Characteristics.Attributes.IsSet) return "Вы уже распределили очки характеристик";

        //        if (character.IsActive)
        //        {
        //            short sum = 0;
        //            for (int i = 1; i < 8; i++)
        //            {
        //                if (Int16.Parse(strings[i]) > 9) throw new Exception();
        //                sum += Int16.Parse(strings[i]);
        //            }
        //            if (sum != 22) throw new Exception();

        //            character.Characteristics = new Characteristics()
        //            {
        //                Attributes = new SPECIAL()
        //                {
        //                    Strength = (short)(Int16.Parse(strings[1]) + 1),
        //                    Perception = (short)(Int16.Parse(strings[2]) + 1),
        //                    Endurance = (short)(Int16.Parse(strings[3]) + 1),
        //                    Charisma = (short)(Int16.Parse(strings[4]) + 1),
        //                    Intellegence = (short)(Int16.Parse(strings[5]) + 1),
        //                    Agility = (short)(Int16.Parse(strings[6]) + 1),
        //                    Luck = (short)(Int16.Parse(strings[7]) + 1),
        //                    IsSet = true
        //                }
        //            };

        //            character.Characteristics.Health = character.Characteristics.CurrentHealth = 100 + 10 * character.Characteristics.Attributes.Endurance;
        //            character.Characteristics.ActionPoints = character.Characteristics.CurrentAP = 100 + 10 * character.Characteristics.Attributes.Agility;
        //            character.Characteristics.WeightLimit = (short)(100 + 5 * character.Characteristics.Attributes.Strength);
        //            character.Characteristics.CurrentWL = 0;
        //            character.Characteristics.Experience = 0;
        //            character.Characteristics.NextLevelOn = 500;
        //            character.Characteristics.Rads = 0;
        //            character.Characteristics.RadContamination = RadContamination.Clear;

        //            if (SaveCharacter(character))
        //            {
        //                return await GetCharacterFullInfo(strings[0]);
        //            }
        //            return "Ошибка";
        //        }
        //        else return "Персонаж не найден";
        //    }
        //    catch (Exception)
        //    {
        //        return "Правильно распределите очки! 22 свободных очка характеристик, характеристики не могут превышать 10.";
        //    }
        //}

        //#endregion

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

        #endregion
    }
}
