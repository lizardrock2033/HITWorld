using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Items.Weapons
{
    public class Weapon
    {
        public long Id { get; set; }
        public short WeaponTypeId { get; set; }
        public string Name { get; set; }
        public short AmmoId { get; set; }
        public int Damage { get; set; }
        public short FireRate { get; set; }
        public short FireDistance { get; set; }
        public bool IsHeavy { get; set; }
        public short RarityId { get; set; }
    }
}