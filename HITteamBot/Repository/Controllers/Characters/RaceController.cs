using HITteamBot.Repository.Entities.Characters.Races;
using HITteamBot.Repository.Entities.Characters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HITteamBot.Repository.Controllers.Characters
{
    public class RaceController
    {
        public static async Task<string> AddNewRace(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                Race newRace = new Race()
                {
                    Name = strings[0],
                    BaseAttributes = new Attributes()
                    {
                        Strength = Int16.Parse(strings[1]),
                        Agility = Int16.Parse(strings[2]),
                        Constitution = Int16.Parse(strings[3]),
                        Intelligence = Int16.Parse(strings[4]),
                        Wisdom = Int16.Parse(strings[5]),
                        Charisma = Int16.Parse(strings[6])
                    },
                    BaseCharacteristics = new Characteristics()
                    {
                        Health = Int32.Parse(strings[7]),
                        Mana = Int32.Parse(strings[8]),
                        Stamina = Int32.Parse(strings[9])
                    },
                    RaceAbilities = new List<Ability>()
                };

                if (System.IO.File.Exists(Program.RacesDirectory + $@"\{newRace.Name}.json")) return "Раса с таким названием уже существует";

                Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(Program.RacesDirectory + $@"\{newRace.Name}.json", JsonConvert.SerializeObject(newRace)); });
                await task;
                return $"Раса {newRace.Name} добавлена";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<string> SetRaceAbility(string query)
        {
            try
            {
                string[] strings = query.Trim().Split(new char[] { ' ' });
                Ability ability = new Ability()
                {
                    Name = strings[1],
                    LearningProgress = Byte.Parse(strings[2]),
                    Description = string.Join(' ', strings[3..]),
                    IsRaceAbility = true
                };

                Race race = JsonConvert.DeserializeObject<Race>(System.IO.File.ReadAllText(Program.RacesDirectory + $@"\{strings[0]}.json"));
                race.RaceAbilities.Add(ability);

                Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(Program.RacesDirectory + $@"\{strings[0]}.json", JsonConvert.SerializeObject(race)); });
                await task;
                return $"Расовый навык {ability.Name} добавлен";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }
    }
}
