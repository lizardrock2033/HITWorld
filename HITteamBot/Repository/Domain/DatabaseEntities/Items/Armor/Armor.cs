using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Items.Armor
{
    public class Armor
    {
        public long Id { get; set; }
        public short ArmorTypeId { get; set; }
        public string Name { get; set; }
        public short DefenceValue { get; set; }
        public short RarityId { get; set; }
    }
}
