using HITteamBot.Repository.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Items.Chemicals
{
    public class Buffout
    {
        public int Count { get; set; }
        public BaseEffects Effect { get; set; }
    }
}
