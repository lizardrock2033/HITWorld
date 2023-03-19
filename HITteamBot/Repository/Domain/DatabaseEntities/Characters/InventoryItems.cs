using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Characters
{
    public class InventoryItems
    {
        public long Id { get; set; }
        public long CharacterId { get; set; }
        public short ItemId { get; set; }
        public short ItemTypeId { get; set; }
        public short Count { get; set; }
        public bool IsEquiped { get; set; }
        public long BoostId { get; set; }
    }
}