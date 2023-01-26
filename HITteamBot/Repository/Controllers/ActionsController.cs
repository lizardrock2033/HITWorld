using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HITteamBot.Repository.Entities.Actions;
using HITteamBot.Repository.Entities.Base;
using Newtonsoft.Json;

namespace HITteamBot.Repository.Controllers
{
    public class ActionsController
    {
        public static async Task<string> AddNewAction(string query)
        {
            try
            {
                string[] data = query.Trim().Split(new char[] { ' ' });
                Entities.Actions.Action action = new Entities.Actions.Action()
                {
                    Name = data[0],
                    Type = (ActionType)Enum.Parse(typeof(ActionType), data[1]),
                    DurationInMinutes = Int16.Parse(data[2])
                };
                string path = Program.ActionsDirectory + $@"\{action.Type.ToString()}\{action.Name}.json";
                if (!System.IO.Directory.Exists(Program.ActionsDirectory + $@"\{action.Type.ToString()}")) System.IO.Directory.CreateDirectory(Program.ActionsDirectory + $@"\{action.Type.ToString()}");
                if (System.IO.File.Exists(path)) return "Действие с таким названием уже существует";
                Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(action)); });
                await task;
                return $"Действие {action.Name} сохранено\r\n\r\n" +
                    $"/addActionReward название exploring/trading/fight experience/caps/junk/item количество";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<string> AddRewardToAction(string query)
        {
            try
            {
                string[] data = query.Trim().Split(new char[] { ' ' });
                string path = Program.ActionsDirectory + $@"\{data[1]}\{data[0]}.json";
                if (System.IO.File.Exists(path))
                {
                    Entities.Actions.Action action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path));
                    if (action.Rewards == null || action.Rewards?.Count == 0) action.Rewards = new List<ActionReward>();
                    action.Rewards.Add(new ActionReward()
                    {
                        Type = (ActionRewardType)Enum.Parse(typeof(ActionRewardType), data[2]),
                        Amount = Int64.Parse(data[3])
                    });
                    Task task = Task.Factory.StartNew(() => { System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(action)); });
                    await task;
                    return $"Награда {data[2]} за действие {data[0]} в количестве {data[3]} добавлена";
                }
                else return "Файл не найден";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<List<Entities.Actions.Action>> GetActions(ActionType type)
        {
            List<Entities.Actions.Action> actionsList = new List<Entities.Actions.Action>();
            try
            {
                string path = Program.ActionsDirectory + $@"\{type}";
                Task task = Task.Factory.StartNew(() => 
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        foreach (var act in System.IO.Directory.GetFiles(path))
                        {
                            actionsList.Add(JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(act)));
                        }
                    }
                });
                await task;
            }
            catch (Exception)
            {

            }
            return actionsList;
        }

        public static async Task<string> GetActionInfo(string query)
        {
            Entities.Actions.Action action = new Entities.Actions.Action();
            string info = "Информация о задании не найдена";
            try
            {
                string[] data = query.Trim().Split(new char[] { ' ' });
                ActionType type = (ActionType)Enum.Parse(typeof(ActionType), data[0]);
                string path = Program.ActionsDirectory + $@"\{type}\{string.Join('_', data[1..])}.json";
                Task task = Task.Factory.StartNew(() =>
                {
                    if (System.IO.File.Exists(path))
                        action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path));
                });

                await task;
                if (action != null)
                {
                    info = $"{Emoji.Clipboard}*Задание:*   _{action.Name.Replace("_", " ")}_\r\n" +
                            $"{Emoji.FileCabinet}*Тип задания:*   _{Dictionaries.GetActionType(type)}_\r\n" +
                            $"{Emoji.Stopwatch}*Продолжительность:*   _{TimeSpan.FromMinutes(action.DurationInMinutes).ToString(@"hh\:mm")}ч._\r\n\r\n" +
                            $"{Emoji.Reward}*Награды:*\r\n\r\n";

                    foreach (var reward in action.Rewards)
                    {
                        info += $"*{Dictionaries.GetActionReward(reward.Type)}:*   _{reward.Amount}_\r\n";
                    }
                }
            }
            catch (Exception)
            {
                return "Ошибка";
            }
            return info;
        }

        //public static async Task<string> StartAction(string query)
        //{

        //}
    }
}
