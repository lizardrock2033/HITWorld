using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Perks
{
    public class Effect
    {
        public string Name { get; set; }
        public EffectTarget Target { get; set; }
        public EffectType Type { get; set; }
        public long Power { get; set; }
        public string Description { get; set; }
    }

    public enum EffectTarget
    {
        Character,
        Enemy,
        Any
    }

    public enum EffectType
    {
        Damage,
        Heal,
        Approval,
        Other
    }
}
