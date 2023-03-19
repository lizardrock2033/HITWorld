using System;
using System.Collections.Generic;
using System.Text;
using HITteamBot.Repository.Entities.Base;

namespace HITteamBot.Repository.Entities.Items.Chemicals
{
    public class Stimpack
    {
        public int Count { get; set; }
        public BaseEffects Effect { get; set; }
    }
}
