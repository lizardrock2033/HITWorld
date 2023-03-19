using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Items.Ammo
{
    public class Ammo
    {
        public short Id { get; set; }
        public short TypeId { get; set; }
        public string Name { get; set; }
    }
}
