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
                    Inventory = new Inventory()
                    {
                        Cups = 50,
                        Chemicals = new Chemicals()
                        {
                            Stimpacks = new List<Stimpack>(),
                            Buffouts = new List<Buffout>(),
                            Mentats = new List<Mentats>(),
                            Psyhos = new List<Psyho>(),
                            RadAways = new List<RadAway>(),
                            RadXes = new List<RadX>()
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

                Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(userCharacterDirectory + $@"\{strings[1]}.json", JsonConvert.SerializeObject(newCharacter)); });
                await task;
                return $"Персонаж {strings[1]} создан!";

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

        public static async void GetCharacterInfo(ITelegramBotClient botClient, long chatId, string username, string name, CancellationToken cancellationToken)
        {
            try
            {
                string characterPath = Program.GetUserDirectory(username) + $@"\Character\{name}.json";
                if (System.IO.File.Exists(characterPath))
                {
                    Character character = JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(characterPath));
                    string info = $"Имя персонажа: >>> {character.Name}\r\n" +
                        $"Возраст:   >>>   {character.Age}\r\n" +
                        $"Уровень:   >>>   {character.Level}\r\n" +
                        $"Текущая локация:   >>>   {character.CurrentLocationType.ToString()}\r\n";

                    //InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData($"Продолжить за ", $"ContinueWith"));
                    await botClient.SendTextMessageAsync(chatId: chatId, text: info, /*replyMarkup: inlineKeyboard,*/ cancellationToken: cancellationToken);
                }
            }
            catch (Exception)
            {

            }
        }

        public static Character GetCharacter(string username, string charactersName)
        {
            Character character = new Character();
            try
            {
                string characterPath = Program.GetUserDirectory(username) + $@"\Character\{charactersName}.json";
                if (System.IO.File.Exists(characterPath))
                {
                    character = JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(characterPath));
                }
            }
            catch (Exception)
            {

            }
            return character;
        }

        public static void SaveCharacter(Character character)
        {
            try
            {
                string characterPath = Program.GetUserDirectory(character.User) + $@"\Character\{character.Name}.json";
                if (System.IO.File.Exists(characterPath))
                {
                    System.IO.File.WriteAllText(characterPath, JsonConvert.SerializeObject(character));
                }
            }
            catch (Exception)
            {

            }
        }

        public static bool DeleteCharacter(string username, string charactersName)
        {
            try
            {
                Character character = GetCharacter(username, charactersName);
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
