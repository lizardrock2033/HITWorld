using System;
using System.Collections.Generic;
using System.Text;
using HITteamBot.Repository.Entities.Characters;

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
    }
}
