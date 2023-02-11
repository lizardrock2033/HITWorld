using System;
using System.Collections.Generic;
using System.Linq;
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
                    $"/addActionReward название exploring/trading/fight experience/caps/junk/item количество\r\n\r\n" +
                    $"/addActionConseq название exploring/trading/fight rads damage";
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

        public static async Task<string> AddConsequencesToAction(string query)
        {
            try
            {
                string[] data = query.Trim().Split(new char[] { ' ' });
                string path = Program.ActionsDirectory + $@"\{data[1]}\{data[0]}.json";
                if (System.IO.File.Exists(path))
                {
                    await Task.Factory.StartNew(() => {
                        Entities.Actions.Action action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path));
                        action.Consequences = new ActionConsequences()
                        {
                            Rads = Int16.Parse(data[2]),
                            Damage = Int16.Parse(data[3])
                        };
                        System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(action));
                    });

                    return $"Последствия Rads: {data[2]}, Damage: {data[3]} добавлены";
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

        public static async Task<ActionHistory> StartAction(string username, string path)
        {
            Entities.Actions.Action action = new Entities.Actions.Action();
            ActionHistory actionHistory = new ActionHistory();
            try
            {
                string historyPath = Program.HistoryDirectory + $@"\Actions\{DateTime.Now.ToString("dd-MM-yyyy")}.json";
                if (!System.IO.Directory.Exists(Program.HistoryDirectory + $@"\Actions")) System.IO.Directory.CreateDirectory(Program.HistoryDirectory + $@"\Actions");

                if (System.IO.File.Exists(path))
                {
                    await Task.Factory.StartNew(() => {
                        Random random = new Random();
                        action = JsonConvert.DeserializeObject<Entities.Actions.Action>(System.IO.File.ReadAllText(path));
                        Entities.Characters.Character character = Characters.CharactersController.GetCharacter(username).Result;
                        actionHistory = new ActionHistory()
                        {
                            Username = username,
                            ActionType = action.Type,
                            ActionName = action.Name,
                            StartDate = DateTime.Now,
                            FinishDate = DateTime.Now.AddMinutes(action.DurationInMinutes),
                            Rewards = action.Rewards,
                            IsRewarded = false,
                            Consequences = new ActionConsequences()
                            {
                                Rads = (short)(action.Consequences.Rads + character.Level * 3 - random.Next(character.Characteristics.Attributes.Luck * 10) - character.Characteristics.Attributes.Endurance * 3),
                                Damage = action.Consequences.Damage + character.Level - character.Characteristics.Attributes.Endurance * 2 - random.Next(character.Characteristics.Attributes.Luck * 5)
                            }
                        };

                        foreach (var rew in actionHistory.Rewards)
                        {
                            switch (rew.Type)
                            {
                                case ActionRewardType.Experience:
                                    rew.Amount += (long)(rew.Amount * (character.Characteristics.Attributes.Intellegence * 0.03f));
                                    break;
                                case ActionRewardType.Caps:
                                    rew.Amount += (long)(rew.Amount * (character.Characteristics.Attributes.Perception * 0.01f) + random.Next(character.Characteristics.Attributes.Luck + 1) * character.Level);
                                    break;
                                case ActionRewardType.Junk:
                                    break;
                                case ActionRewardType.Item:
                                    break;
                                default:
                                    break;
                            }
                        }

                        List<ActionHistory> histories = new List<ActionHistory>();
                        if (System.IO.File.Exists(historyPath)) histories = JsonConvert.DeserializeObject<List<ActionHistory>>(System.IO.File.ReadAllText(historyPath));
                        histories.Add(actionHistory);
                        System.IO.File.WriteAllText(historyPath, JsonConvert.SerializeObject(histories));
                    });
                }
            }
            catch (Exception)
            {
                
            }
            return actionHistory;
        }

        public static async Task<bool> IsInAction(string username)
        {
            bool response = false;
            List<ActionHistory> histories = new List<ActionHistory>();
            try
            {
                string historyPath = Program.HistoryDirectory + $@"\Actions\{DateTime.Now.ToString("dd-MM-yyyy")}.json";
                string yesterdayHistoryPath = Program.HistoryDirectory + $@"\Actions\{DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy")}.json";
                if (System.IO.File.Exists(historyPath))
                {
                    await Task.Factory.StartNew(() =>
                    {
                        histories = (List<ActionHistory>)JsonConvert.DeserializeObject<IEnumerable<ActionHistory>>(System.IO.File.ReadAllText(historyPath));
                        if (System.IO.File.Exists(yesterdayHistoryPath)) histories.AddRange((List<ActionHistory>)JsonConvert.DeserializeObject<IEnumerable<ActionHistory>>(System.IO.File.ReadAllText(yesterdayHistoryPath)));
                        response = histories.Any(s => s.Username == username && s.FinishDate >= DateTime.Now);
                    });
                }
            }
            catch (Exception)
            {

            }
            return response;
        }

        public static async void GiveOutLast2DaysRewards()
        {
            try
            {
                await GiveOutRewardsFromActions(DateTime.Now);
                await GiveOutRewardsFromActions(DateTime.Now.AddDays(-1));
            }
            catch (Exception)
            {

            }
        }

        public static async Task<bool> GiveOutRewardsFromActions(DateTime date)
        {
            bool response = false;
            List<ActionHistory> histories = new List<ActionHistory>();
            try
            {
                string historyPath = Program.HistoryDirectory + $@"\Actions\{date.ToString("dd-MM-yyyy")}.json";
                if (System.IO.File.Exists(historyPath))
                {
                    await Task.Factory.StartNew(() =>
                    {
                        histories = (List<ActionHistory>)JsonConvert.DeserializeObject<IEnumerable<ActionHistory>>(System.IO.File.ReadAllText(historyPath));
                        foreach (var history in histories.Where(s => s.IsRewarded == false && s.FinishDate <= DateTime.Now))
                        {
                            Entities.Characters.Character character = Characters.CharactersController.GetCharacter(history.Username).Result;
                            character.Characteristics.Health -= history.Consequences.Damage;
                            if (character.Characteristics.Health <= 0) character.LifeState = Entities.Characters.LifeStates.Unconscious;
                            character.Characteristics.Rads += history.Consequences.Rads;
                            if (character.Characteristics.Rads >= 1000) character.LifeState = Entities.Characters.LifeStates.Dead;
                            foreach (var rew in history.Rewards)
                            {
                                switch (rew.Type)
                                {
                                    case ActionRewardType.Experience:
                                        character.Characteristics.Experience += (int)rew.Amount;
                                        break;
                                    case ActionRewardType.Caps:
                                        character.Inventory.Caps += rew.Amount;
                                        break;
                                    case ActionRewardType.Junk:
                                        break;
                                    case ActionRewardType.Item:
                                        break;
                                    default:
                                        break;
                                }
                            }
                            history.IsRewarded = true;
                            Characters.CharactersController.SaveCharacter(character);
                        }
                        System.IO.File.WriteAllText(historyPath, JsonConvert.SerializeObject(histories));
                    });
                    response = true;
                }
            }
            catch (Exception)
            {

            }
            return response;
        }
    }
}
