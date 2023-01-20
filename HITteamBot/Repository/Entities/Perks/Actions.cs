using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Perks
{
    public class Actions
    {
        public string Name { get; set; }
        public ActionTargets Target { get; set; }
        public ActionTypes Type { get; set; }
        public long Power { get; set; }
        public string Description { get; set; }
    }

    public enum ActionTargets
    {
        Character,
        Enemy,
        Any
    }

    public enum ActionTypes
    {
        Damage,
        Heal,
        Approval,
        Other
    }
}
