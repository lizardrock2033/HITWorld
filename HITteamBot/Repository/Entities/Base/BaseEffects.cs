using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Base
{
    public class BaseEffects
    {
        public BaseEffectTypes Type { get; set; }
        public int Power { get; set; }
        public int Duration { get; set; }
        public BaseEffectTargets AvailableTargets { get; set; }
        public string Description { get; set; }
    }

    public enum BaseEffectTypes
    {
        Heal,
        Damage
    }

    public enum BaseEffectTargets
    {
        Character,
        Enemy,
        Any
    }
}
