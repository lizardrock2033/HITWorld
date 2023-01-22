using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Perks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HITteamBot.Repository.Controllers.Characters
{
    public class PerksController
    {
        public static async Task<string> AddNewPerk(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                Perk perk = new Perk()
                {
                    Name = strings[0],
                    Attribute = (SPECIALs)Enum.Parse(typeof(SPECIALs), strings[1]),
                    Cost = Int16.Parse(strings[2]),
                    Type = (PerkTypes)Enum.Parse(typeof(PerkTypes), strings[3]),
                    Description = string.Join(' ', strings[4..])
                };
                if (System.IO.File.Exists(Program.PerksDirectory + $@"\{perk.Name}.json")) return "Перк с таким названием уже существует";
                Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(Program.PerksDirectory + $@"\{perk.Name}.json", JsonConvert.SerializeObject(perk)); });
                await task;
                return $"Перк {perk.Name} добавлен";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<string> AddNewAction(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                Actions action = new Actions()
                {
                    Name = strings[0],
                    Target = (ActionTargets)Enum.Parse(typeof(ActionTargets), strings[1]),
                    Type = (ActionTypes)Enum.Parse(typeof(ActionTypes), strings[2]),
                    Power = Int64.Parse(strings[3]),
                    Description = string.Join(' ', strings[4..])
                };
                if (System.IO.File.Exists(Program.ActionsDirectory + $@"\{action.Name}.json")) return "Воздействие с таким названием уже существует";
                Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(Program.ActionsDirectory + $@"\{action.Name}.json", JsonConvert.SerializeObject(action)); });
                await task;
                return $"Воздействие {action.Name} добавлено";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<string> AddPerkToCharacter(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                Perk perk = new Perk() { Name = strings[2] };

                Character character = JsonConvert.DeserializeObject<Character>(System.IO.File.ReadAllText(Program.UsersDirectory + $@"\{strings[0]}\Character\{strings[1]}.json"));
                if (IsAvailable(character, perk))
                {
                    character.Perks.Add(perk);
                    Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(Program.UsersDirectory + $@"\{strings[0]}\Character\{strings[1]}.json", JsonConvert.SerializeObject(character)); });
                    await task;
                    return $"Перк {perk.Name} добавлен персонажу {character.Name}";
                }
                else
                {
                    return "Не хватает характеристик или перк уже взят!";
                }
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static bool IsAvailable(Character character, Perk perk)
        {
            try
            {
                Perk perkData = JsonConvert.DeserializeObject<Perk>(System.IO.File.ReadAllText(Program.PerksDirectory + $@"\{perk.Attribute.ToString()}\{perk.Name}.json"));
                System.Reflection.PropertyInfo field = typeof(SPECIAL).GetProperty($"{perk.Attribute.ToString()}");
                short value = (short)field.GetValue(character.Characteristics.Attributes);
                return character.Perks.Where(n => n.Name == perk.Name).Count() == 0 && value >= perk.Cost;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
