using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Items.Chemicals
{
    public class Chemicals
    {
        public Stimpack Stimpacks { get; set; }
        public Buffout Buffouts { get; set; }
        public Mentats Mentats { get; set; }
        public Psyho Psyhos { get; set; }
        public MedX MedXes { get; set; }
        public RadAway RadAways { get; set; }
        public RadX RadXes { get; set; }
    }
}
