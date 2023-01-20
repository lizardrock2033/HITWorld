using HITteamBot.Repository.Entities.Items.Consumables;
using HITteamBot.Repository.Entities.Items.Currency;
using HITteamBot.Repository.Links.Images.Items.Ammo;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Inventory
    {
        public Coins Coins { get; set; }
        public Crystals Crystals { get; set; }
        public Potions Potions { get; set; }
        public Arrows Arrows { get; set; }
    }
}
