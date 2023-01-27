using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HITteamBot.Repository.Entities.Actions;
using HITteamBot.Repository.Entities.Base;
using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

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
                await Task.Factory.StartNew(() => { System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(action)); });

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
                    await Task.Factory.StartNew(() => { System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(action)); });

                    return $"Награда {data[2]} за действие {data[0]} в количестве {data[3]} добавлена";
                }
                else return "Файл не найден";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<List<InlineKeyboardButton[]>> GetActionsInButtons(ActionType type)
        {
            List<InlineKeyboardButton[]> actions = new List<InlineKeyboardButton[]>();
            try
            {
                string path = Program.ActionsDirectory + $@"\{type}";
                await Task.Factory.StartNew(() => 
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        foreach (var act in System.IO.Directory.GetFiles(path))
                        {
                            Entities.Actions.Action action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(act));
                            actions.Add(new[] { InlineKeyboardButton.WithCallbackData($"{action.Name.Replace("_", " ")}", $"{(int)MainShedule.GameMenu}_{GameMenu.ActionInfo}_{(int)type}_{action.Name}") });
                        }

                        if (actions.Count == 0) actions.Add(new[] { InlineKeyboardButton.WithCallbackData("Задания на сегодня закончились", $"{(int)MainShedule.MainMenu}_66") });
                        actions.Add(new[] { InlineKeyboardButton.WithCallbackData("Назад", $"{(int)MainShedule.MainMenu}_66") });
                    }
                });
            }
            catch (Exception)
            {

            }
            return actions;
        }

        public static async Task<string> GetActionPath(string query)
        {
            try
            {
                string[] data = query.Trim().Split(new char[] { '_' });
                ActionType type = (ActionType)Int32.Parse(data[0]);
                string path = Program.ActionsDirectory + $@"\{type}\{string.Join('_', data[1..])}.json";
                return await Task.Factory.StartNew(() => {
                    if (System.IO.File.Exists(path)) return path;
                    else throw new Exception();
                });
            }
            catch (Exception)
            {
                return "не найдено";
            }
        }

        public static async Task<Entities.Actions.Action> GetAction(string path)
        {
            Entities.Actions.Action action = new Entities.Actions.Action();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    action = System.IO.File.Exists(path) ? JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path)) : new Entities.Actions.Action();
                });
            }
            catch (Exception)
            {

            }
            return action;
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

                if (System.IO.File.Exists(path))
                    await Task.Factory.StartNew(() => { action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path)); });
                
                if (action != null)
                {
                    info = $"{Emoji.Clipboard} *Задание:*   _{action.Name.Replace("_", " ")}_\r\n" +
                            $"{Emoji.FileCabinet} *Тип задания:*   _{Dictionaries.GetActionType(type)}_\r\n" +
                            $"{Emoji.Stopwatch} *Продолжительность:*   _{TimeSpan.FromMinutes(action.DurationInMinutes).ToString(@"hh\:mm")}ч._\r\n\r\n" +
                            $"{Emoji.Reward} *Награды:*\r\n\r\n";

                    foreach (var reward in action.Rewards)
                    {
                        string[] signAndType = Dictionaries.GetActionReward(reward.Type).Split(new char[] { '_' });
                        info += $"   *{signAndType[1]}:*   _{reward.Amount}_ {signAndType[0]}\r\n";
                    }
                }
            }
            catch (Exception)
            {
                return "Ошибка";
            }
            return info;
        }

        public static async Task<string> StartAction(string username, string path)
        {
            string response = "Ошибка отправки на задание";
            Entities.Actions.Action action = new Entities.Actions.Action();
            try
            {
                string historyPath = Program.HistoryDirectory + $@"\Actions\{DateTime.Now.ToString("dd-MM-yyyy")}.json";
                if (!System.IO.Directory.Exists(Program.HistoryDirectory + $@"\Actions")) System.IO.Directory.CreateDirectory(Program.HistoryDirectory + $@"\Actions");

                if (System.IO.File.Exists(path))
                {
                    await Task.Factory.StartNew(() => {
                        action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path));
                        ActionHistory actionHistory = new ActionHistory()
                        {
                            Username = username,
                            ActionType = action.Type,
                            ActionName = action.Name,
                            StartDate = DateTime.Now,
                            FinishDate = DateTime.Now.AddMinutes(action.DurationInMinutes),
                            Rewards = action.Rewards
                        };

                        Entities.Characters.Character character = Characters.CharactersController.GetCharacter(username);

                        foreach (var rew in actionHistory.Rewards)
                        {
                            switch (rew.Type)
                            {
                                case ActionRewardType.Experience:
                                    rew.Amount += rew.Amount * (int)(character.Characteristics.Attributes.Intellegence * 0.03);
                                    break;
                                case ActionRewardType.Caps:
                                    Random random = new Random();
                                    rew.Amount += (rew.Amount * (int)(character.Characteristics.Attributes.Perception * 0.01) + random.Next(character.Characteristics.Attributes.Luck + 1) * character.Level);
                                    break;
                                case ActionRewardType.Junk:
                                    break;
                                case ActionRewardType.Item:
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (System.IO.File.Exists(historyPath)) System.IO.File.AppendAllText(historyPath, JsonConvert.SerializeObject(actionHistory));
                        else System.IO.File.WriteAllText(historyPath, JsonConvert.SerializeObject(actionHistory));

                        response = $"*{character.Name}* отправился на _{action.Name.Replace("_", " ")}_! Вернется в {actionHistory.FinishDate.ToString("HH:mm")}";
                    });
                }
            }
            catch (Exception)
            {
                
            }
            return response;
        }
    }
}
