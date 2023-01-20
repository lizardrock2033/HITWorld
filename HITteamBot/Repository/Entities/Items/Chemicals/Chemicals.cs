using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Items.Chemicals
{
    public class Chemicals
    {
        public List<Stimpack> Stimpacks { get; set; }
        public List<Buffout> Buffouts { get; set; }
        public List<Mentats> Mentats { get; set; }
        public List<Psyho> Psyhos { get; set; }
        public List<RadAway> RadAways { get; set; }
        public List<RadX> RadXes { get; set; }
    }
}
