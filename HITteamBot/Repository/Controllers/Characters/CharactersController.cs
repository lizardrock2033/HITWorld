using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Characters.Classes;
using HITteamBot.Repository.Entities.Characters.Races;
using HITteamBot.Repository.Entities.Items.Consumables;
using HITteamBot.Repository.Entities.Items.Currency;
using HITteamBot.Repository.Entities.Items.Equipment;
using HITteamBot.Repository.Entities.Locations;
using HITteamBot.Repository.Links.Images.Items.Ammo;
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

namespace HITteamBot.Repository.Controllers.Characters
{
    public class CharactersController
    {
        public static async void CreateNewCharacter(ITelegramBotClient botClient, Chat chat, string username, string name, short age, string story, CancellationToken cancellationToken)
        {
            try
            {
                string userDirectory = Program.GetUserDirectory(username);
                string userCharacterDirectory = userDirectory + $"\\Characters";
                string charDirectory = userCharacterDirectory + $"\\{name}";
                if (!Directory.Exists(userDirectory)) Directory.CreateDirectory(userDirectory);
                if (!Directory.Exists(userCharacterDirectory)) Directory.CreateDirectory(userCharacterDirectory);
                if (!Directory.Exists(charDirectory)) Directory.CreateDirectory(charDirectory);

                Character newCharacter = new Character()
                {
                    User = username,
                    Name = name,
                    Age = age,
                    Story = story,
                    Level = 1,
                    CurrentLocation = Locations.Village,
                    IsActive = true,
                    Equipment = new Equipment(),
                    Inventory = new Inventory() { Coins = new Coins() { Copper = 15 }, Crystals = new Crystals(), Potions = new Potions(), Arrows = new Arrows() }
                };

                System.IO.File.WriteAllText(charDirectory + $@"\{name}.json", JsonConvert.SerializeObject(newCharacter));

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Человек", $"SetRace{CharacterRaces.Human}"),
                        InlineKeyboardButton.WithCallbackData("Воин", $"SetClass{CharacterClasses.Warrior}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("Эльф", $"SetRace{CharacterRaces.Elf}"),
                        InlineKeyboardButton.WithCallbackData("Убийца", $"SetClass{CharacterClasses.Assassin}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("Дворф", $"SetRace{CharacterRaces.Dwarf}"),
                        InlineKeyboardButton.WithCallbackData("Лучник", $"SetClass{CharacterClasses.Archer}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("Орк", $"SetRace{CharacterRaces.Orc}"),
                        InlineKeyboardButton.WithCallbackData("Маг", $"SetClass{CharacterClasses.Mage}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("Людоящер", $"SetRace{CharacterRaces.Lizardman}"),
                        InlineKeyboardButton.WithCallbackData("Призыватель", $"SetClass{CharacterClasses.Summoner}")
                    },
                    new[] {
                        InlineKeyboardButton.WithCallbackData("Подтвердить", $"SaveRaceAndClass{name}")
                    }
                });
                await botClient.SendTextMessageAsync(chatId: chat.Id, text: $"Персонаж {name} создан! Осталось только выбрать расу и класс!", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }

        public static async void SetCharacterRace(ITelegramBotClient botClient, long chatId, string username, string name, CharacterRaces race, CancellationToken cancellationToken)
        {

        }

        public static async void SetCharacterClass(ITelegramBotClient botClient, long chatId, string username, string name, CancellationToken cancellationToken)
        {

        }

        public static async void GetUserCharacters(ITelegramBotClient botClient, Chat chat, string username, CancellationToken cancellationToken)
        {
            try
            {
                string userCharactersDirectory = Program.GetUserDirectory(username) + $@"\Characters";
                if (Directory.Exists(userCharactersDirectory))
                {
                    string[] characters = Directory.GetDirectories(userCharactersDirectory);
                    InlineKeyboardButton[][] keyboardButtons = new InlineKeyboardButton[characters.Length][];

                    for (int i = 0; i < characters.Length; i++)
                    {
                        string charPath = Directory.GetFiles(characters[i]).FirstOrDefault();
                        Character character = JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(charPath));
                        keyboardButtons[i] = new[] { InlineKeyboardButton.WithCallbackData($"{character.Name}", $"{character.Name}") };
                    }

                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(keyboardButtons);

                    await botClient.SendTextMessageAsync(
                                chatId: chat.Id,
                                text: "Ваши персонажи:",
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken);
                }
                else if (!Directory.Exists(userCharactersDirectory) || Directory.GetDirectories(userCharactersDirectory).Length == 0)
                {
                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Создать персонажа", MainMenu.NewCharacter.ToString()) });
                    await botClient.SendTextMessageAsync(
                                chatId: chat.Id,
                                text: "У вас нет персонажей!",
                                replyMarkup: inlineKeyboard,
                                cancellationToken: cancellationToken);
                }
            }
            catch (Exception)
            {

            }
        }

        public static async void GetCharacterInfo(ITelegramBotClient botClient, long chatId, string username, string name, CancellationToken cancellationToken)
        {
            try
            {
                string characterPath = Program.GetUserDirectory(username) + $@"\Characters\{name}\{name}.json";
                if (System.IO.File.Exists(characterPath))
                {
                    Character character = JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(characterPath));
                    string info = $"Имя персонажа: >>> {character.Name}\r\n" +
                        $"Возраст:   >>>   {character.Age}\r\n" +
                        $"Раса:   >>>   {character.Race.ToString()}\r\n" +
                        $"Класс:   >>>   {character.Class.ToString()}\r\n" +
                        $"Уровень:   >>>   {character.Level}\r\n" +
                        $"Текущая локация:   >>>   {character.CurrentLocation.ToString()}\r\n\r\n" +
                        $"{character.Story}";

                    InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData($"Продолжить за {character.Name}", $"ContinueWith{character.Name}"));
                    await botClient.SendTextMessageAsync(chatId: chatId, text: info, replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
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
                string characterPath = Program.GetUserDirectory(username) + $@"\Characters\{charactersName}\{charactersName}.json";
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
                string characterPath = Program.GetUserDirectory(character.User) + $@"\Characters\{character.Name}\{character.Name}.json";
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

        public static async void ContinueJourneyWithCharacter(ITelegramBotClient botClient, long chatId, string username, string name, CancellationToken cancellationToken)
        {
            try
            {
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData($"Классно!", $"soska"));
                await botClient.SendTextMessageAsync(chatId: chatId, text: $"Продолжаем путешествие за {name}!", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
            }
            catch (Exception)
            {

            }
        }
    }
}
