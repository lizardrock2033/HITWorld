using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Links.Images.Items.Consumables
{
    public class Consumables
    {
        public class Potions
        {
            // Маленькие банки
            public string Health_s = Program.AssetsDirectory + @"\Equipment\potion_01a.png";
            public string Mana_s = Program.AssetsDirectory + @"\Equipment\potion_01b.png";
            public string Stamina_s = Program.AssetsDirectory + @"\Equipment\potion_01c.png";

            // Средние банки
            public string Health_m = Program.AssetsDirectory + @"\Equipment\potion_02a.png";
            public string Mana_m = Program.AssetsDirectory + @"\Equipment\potion_02b.png";
            public string Stamina_m = Program.AssetsDirectory + @"\Equipment\potion_02c.png";

            // Большие банки
            public string Health_l = Program.AssetsDirectory + @"\Equipment\potion_03a.png";
            public string Mana_l = Program.AssetsDirectory + @"\Equipment\potion_03b.png";
            public string Stamina_l = Program.AssetsDirectory + @"\Equipment\potion_03c.png";
        }
    }
}
