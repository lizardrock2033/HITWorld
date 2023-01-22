using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Items.Equipment;
using HITteamBot.Repository.Entities.Items.Chemicals;
using HITteamBot.Repository.Entities.Items.Ammo;
using HITteamBot.Repository.Entities.Items.Junk;
using HITteamBot.Repository.Entities.Locations;
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

namespace HITteamBot.Repository.Controllers.Characters
{
    public class CharactersController
    {
        public static async Task<string> CreateNewCharacter(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                string userDirectory = Program.GetUserDirectory(strings[0]);
                string userCharacterDirectory = userDirectory + $"\\Character";
                if (!Directory.Exists(userDirectory)) Directory.CreateDirectory(userDirectory);
                if (!Directory.Exists(userCharacterDirectory)) Directory.CreateDirectory(userCharacterDirectory);

                Character newCharacter = new Character()
                {
                    User = strings[0],
                    Name = strings[1],
                    Age = Byte.Parse(strings[2]),
                    Gender = strings[3],
                    Level = 1,
                    CurrentLocationType = LocationTypes.Settlement,
                    LifeState = LifeStates.Alive,
                    Activity = Activities.Waiting,
                    IsActive = true,
                    Equipment = new Equipment(),
                    Characteristics = new Characteristics() { Attributes = new SPECIAL() { IsSet = false } },
                    Inventory = new Inventory()
                    {
                        Cups = 50,
                        Chemicals = new Chemicals()
                        {
                            Stimpacks = new List<Stimpack>()
                        },
                        Ammo = new Ammo()
                        {
                            Bullets = new List<Bullets>(),
                            Battaries = new List<Battaries>(),
                            Rockets = new List<Rockets>(),
                            Grenades = new List<Grenades>()
                        },
                        Junk = new Junk()
                        {

                        },
                        Weapons = new List<Weapon>(),
                        Armor = new List<Armor>(),
                        Clothes = new List<Clothes>()
                    }
                };

                bool response = false;
                Task task = Task.Factory.StartNew(() => { response = SaveCharacter(newCharacter); });
                await task;

                if (response)
                {
                    return $"Персонаж {strings[1]} создан!\r\n" +
                        "Теперь необходимо распределить очки характеристик.\r\n" +
                        "Сейчас все ваши характеристики равны 1 и у вас есть 22 очка характеристик для распределения.\r\n" +
                        "Обратите внимание:\r\n" +
                        "- Значения характеристик не могут стать выше 10 при распределении.\r\n" +
                        "- Необходимо распределить сразу все 22 очка характеристик.\r\n" +
                        "- В дальнейшем, при получении уровня, можно будет повышать характеристики (но не выше 10).\r\n" +
                        "- Характеристики можно увеличить выше 10 при помощи предметов, химикатов и пр.\r\n\r\n" +
                        "Описание характеристик:\r\n\r\n" +
                        "- Сила (Strength) - переносимый вес, урон тяжелого оружия и урон в ближнем бою.\r\n\r\n" +
                        "- Восприятие (Perception) - меткость и внимательность.\r\n\r\n" +
                        "- Выносливость (Endurance) - здоровье и стойкость.\r\n\r\n" +
                        "- Харизма (Charisma) - торговля и общение.\r\n\r\n" +
                        "- Интеллект (Intellegence) - модификация оружия, получаемый опыт и крафт.\r\n\r\n" +
                        "- Ловкость (Agility) - урон в дальнем бою, скрытность и очки действия (ОД).\r\n\r\n" +
                        "- Удача (Luck) - криты, находимый хлам, везение.\r\n\r\n" +
                        "Чтобы распределить очки характеристик, необходимо написать /атрибуты и указать необходимое количество вливаемых очков для каждой характеристики в указанном выше порядке.\r\n\r\n" +
                        "Пример:\r\n" +
                        "/атрибуты 7 1 7 0 1 3 3";
                }
                else
                {
                    return "У вас уже есть персонаж";
                }
                

                //InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                //{
                //    new[]
                //    {
                //        InlineKeyboardButton.WithCallbackData("Человек", $"SetRace"),
                //        InlineKeyboardButton.WithCallbackData("Воин", $"SetClass")
                //    },
                //    new[] {
                //        InlineKeyboardButton.WithCallbackData("Эльф", $"SetRace"),
                //        InlineKeyboardButton.WithCallbackData("Убийца", $"SetClass")
                //    },
                //    new[] {
                //        InlineKeyboardButton.WithCallbackData("Дворф", $"SetRace"),
                //        InlineKeyboardButton.WithCallbackData("Лучник", $"SetClass")
                //    },
                //    new[] {
                //        InlineKeyboardButton.WithCallbackData("Орк", $"SetRace"),
                //        InlineKeyboardButton.WithCallbackData("Маг", $"SetClass")
                //    },
                //    new[] {
                //        InlineKeyboardButton.WithCallbackData("Людоящер", $"SetRace"),
                //        InlineKeyboardButton.WithCallbackData("Призыватель", $"SetClass")
                //    },
                //    new[] {
                //        InlineKeyboardButton.WithCallbackData("Подтвердить", $"SaveRaceAndClass{name}")
                //    }
                //});
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<string> SetCharacterAttributes(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                string userCharacterDirectory = Program.GetUserDirectory(strings[0]) + $"\\Character";
                Character character = GetCharacter(strings[0]);
                if (character.Characteristics.Attributes.IsSet) return "Вы уже распределили очки характеристик";

                if (character.IsActive)
                {
                    short sum = 0;
                    for (int i = 1; i < 8; i++)
                    {
                        if (Int16.Parse(strings[i]) > 9) throw new Exception();
                        sum += Int16.Parse(strings[i]);
                    }
                    if (sum != 22) throw new Exception();

                    character.Characteristics = new Characteristics()
                    {
                        Attributes = new SPECIAL()
                        {
                            Strength = (short)(Int16.Parse(strings[1]) + 1),
                            Perception = (short)(Int16.Parse(strings[2]) + 1),
                            Endurance = (short)(Int16.Parse(strings[3]) + 1),
                            Charisma = (short)(Int16.Parse(strings[4]) + 1),
                            Intellegence = (short)(Int16.Parse(strings[5]) + 1),
                            Agility = (short)(Int16.Parse(strings[6]) + 1),
                            Luck = (short)(Int16.Parse(strings[7]) + 1),
                            IsSet = true
                        }
                    };

                    character.Characteristics.Health = 100 + 10 * character.Characteristics.Attributes.Endurance;
                    character.Characteristics.ActionPoints = 100 + 10 * character.Characteristics.Attributes.Agility;
                    character.Characteristics.WeightLimit = (short)(100 + 5 * character.Characteristics.Attributes.Strength);
                    character.Characteristics.Experience = 0;
                    character.Characteristics.NextLevelOn = 500;
                    character.Characteristics.Rads = 0;

                    if (SaveCharacter(character))
                    {
                        return await GetCharacterInfo(strings[0]);
                    }
                    return "Ошибка";
                }
                else return "Персонаж не найден";
            }
            catch (Exception)
            {
                return "Правильно распределите очки! 22 свободных очка характеристик, характеристики не могут превышать 10.";
            }
        }

        public static async Task<string> SetCharacterAvatar(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                string userCharacterDirectory = Program.GetUserDirectory(strings[0]) + $"\\Character";
                Character character = GetCharacter(strings[0]);
                if (character.IsActive)
                {
                    short sum = 0;
                    for (int i = 1; i < 8; i++)
                    {
                        if (Int16.Parse(strings[i]) > 9) throw new Exception();
                        sum += Int16.Parse(strings[i]);
                    }
                    if (sum != 22) throw new Exception();

                    character.Characteristics = new Characteristics()
                    {
                        Attributes = new SPECIAL()
                        {
                            Strength = (short)(Int16.Parse(strings[1]) + 1),
                            Perception = (short)(Int16.Parse(strings[2]) + 1),
                            Endurance = (short)(Int16.Parse(strings[3]) + 1),
                            Charisma = (short)(Int16.Parse(strings[4]) + 1),
                            Intellegence = (short)(Int16.Parse(strings[5]) + 1),
                            Agility = (short)(Int16.Parse(strings[6]) + 1),
                            Luck = (short)(Int16.Parse(strings[7]) + 1)
                        }
                    };

                    character.Characteristics.Health = 100 + 10 * character.Characteristics.Attributes.Endurance;
                    character.Characteristics.ActionPoints = 100 + 10 * character.Characteristics.Attributes.Agility;
                    character.Characteristics.WeightLimit = (short)(100 + 5 * character.Characteristics.Attributes.Strength);
                    character.Characteristics.Experience = 0;
                    character.Characteristics.NextLevelOn = 500;
                    character.Characteristics.Rads = 0;

                    if (SaveCharacter(character))
                    {
                        return await GetCharacterInfo(strings[0]);
                    }
                    return "Ошибка";
                }
                else return "Персонаж не найден";
            }
            catch (Exception)
            {
                return "Правильно распределите очки! 22 свободных очка характеристик, характеристики не могут превышать 10.";
            }
        }

        public static async Task<string> GetCharacterInfo(string username)
        {
            try
            {
                string userCharacterDirectory = Program.GetUserDirectory(username) + $"\\Character";
                if (Directory.Exists(userCharacterDirectory))
                {
                    Character character = new Character();
                    Task task = Task.Factory.StartNew(() => { character = GetCharacter(username); });
                    await task;
                    string info = "Персонаж не найден";
                    if (character.IsActive)
                    {
                        info = $"{(!string.IsNullOrEmpty(character.Portrait) ? character.Portrait : Emoji.Incognito)} *Имя:*   _{character.Name}_\r\n" +
                                $"{Emoji.DNA} *Возраст:*   {character.Age}\r\n" +
                                $"{(character.Gender.ToLower().Contains("женский") ? Emoji.WomanSign : Emoji.MenSign)} *Пол:*   _{character.Gender}_\r\n" +
                                $"{Emoji.Star} *Уровень:*   _{character.Level} ({character.Characteristics.Experience}/{character.Characteristics.NextLevelOn})_\r\n\r\n" +
                                $"{Emoji.Pager} *Характеристики:*\r\n\r\n" +
                                $"{Emoji.Heart} *HP:*   _{character.Characteristics.Health}_\r\n" +
                                $"{Emoji.Lightning} *AP:*   _{character.Characteristics.ActionPoints}_\r\n\r\n" +
                                $"{Emoji.Muscle} *Сила:*   _{character.Characteristics.Attributes.Strength}_\r\n" +
                                $"{Emoji.Eye} *Восприятие:*   _{character.Characteristics.Attributes.Perception}_\r\n" +
                                $"{Emoji.Lungs} *Выносливость:*   _{character.Characteristics.Attributes.Endurance}_\r\n" +
                                $"{Emoji.SpeakingHead} *Харизма:*   _{character.Characteristics.Attributes.Charisma}_\r\n" +
                                $"{Emoji.Brain} *Интеллект:*   _{character.Characteristics.Attributes.Intellegence}_\r\n" +
                                $"{Emoji.Leg} *Ловкость:*   _{character.Characteristics.Attributes.Agility}_\r\n" +
                                $"{Emoji.Clover} *Удача:*   _{character.Characteristics.Attributes.Luck}_\r\n";
                    }
                    
                    return info;
                }
                else return "Персонаж не найден";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static Character GetCharacter(string username)
        {
            List<Character> charactersList = new List<Character>();
            Character character = new Character();
            try
            {
                string characterPath = Program.GetUserDirectory(username) + $@"\Character";
                if (Directory.Exists(characterPath))
                {
                    foreach (var ch in Directory.GetFiles(characterPath))
                    {
                        charactersList.Add(JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(ch)));
                    }
                    character = charactersList.Where(i => i.IsActive).FirstOrDefault();
                }

            }
            catch (Exception)
            {

            }
            return character;
        }

        public static bool SaveCharacter(Character character)
        {
            try
            {
                string characterPath = Program.GetUserDirectory(character.User) + $@"\Character\{character.Name}.json";
                if (GetCharacter(character.User).IsActive) throw new Exception();
                if (Directory.Exists(Program.GetUserDirectory(character.User) + $@"\Character\"))
                    System.IO.File.WriteAllText(characterPath, JsonConvert.SerializeObject(character));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool DeleteCharacter(string username)
        {
            try
            {
                Character character = GetCharacter(username);
                character.IsActive = false;
                SaveCharacter(character);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
