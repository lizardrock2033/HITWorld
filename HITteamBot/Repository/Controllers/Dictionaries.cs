using System;
using System.Collections.Generic;
using System.Text;
using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Actions;

namespace HITteamBot.Repository.Controllers
{
    public class Dictionaries
    {
        public static string GetRadContamination(RadContamination level)
        {
            Dictionary<RadContamination, string> locales = new Dictionary<RadContamination, string>()
            {
                { RadContamination.Clear, "Чист" },
                { RadContamination.Light, "Легкий" },
                { RadContamination.Normal, "Нормальный" },
                { RadContamination.High, "Высокий" },
                { RadContamination.VeryHigh, "Очень высокий" },
                { RadContamination.Lethal, "Летальный" }
            };
            return locales[level];
        }

        public static string GetActionType(ActionType type)
        {
            Dictionary<ActionType, string> locales = new Dictionary<ActionType, string>()
            {
                { ActionType.Exploring, "Исследование" },
                { ActionType.Trading, "Торговля" },
                { ActionType.Fight, "Бой" }
            };
            return locales[type];
        }

        public static string GetActionReward(ActionRewardType type)
        {
            Dictionary<ActionRewardType, string> locales = new Dictionary<ActionRewardType, string>()
            {
                { ActionRewardType.Experience, "Опыт" },
                { ActionRewardType.Caps, "Крышки" },
                { ActionRewardType.Junk, "Хлам" },
                { ActionRewardType.Item, "Предмет" }
            };
            return locales[type];
        }
    }
}
