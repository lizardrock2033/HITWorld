using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Items.Ammo
{
    public class Ammo
    {
        public List<Bullets> Bullets { get; set; }
        public List<Battaries> Battaries { get; set; }
        public List<Rockets> Rockets { get; set; }
        public List<Grenades> Grenades { get; set; }
    }
}
