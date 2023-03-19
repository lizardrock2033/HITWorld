using HITteamBot.Repository.Entities;
using HITteamBot.Repository.Domain.DatabaseEntities.Characters;
using HITteamBot.Repository.Domain.DatabaseEntities.Experience;
using HITteamBot.Repository.Entities.Items;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Ammo;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Armor;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Chemicals;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Junk;
using HITteamBot.Repository.Domain.DatabaseEntities.Items.Weapons;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Character> Character { get; set; }
        public DbSet<SPECIAL> SPECIAL { get; set; }
        public DbSet<Expline> Explne { get; set; }
        public DbSet<Chemicals> Chemicals { get; set; }
        public DbSet<Ammo> Ammo { get; set; }
        public DbSet<AmmoTypes> AmmoTypes { get; set; }
        public DbSet<Junk> Junk { get; set; }
        public DbSet<ItemTypes> ItemTypes { get; set; }
        public DbSet<InventoryItems> InventoryItems { get; set; }
        public DbSet<Weapon> Weapon { get; set; }
        public DbSet<WeaponTypes> WeaponTypes { get; set; }
        public DbSet<Armor> Armor { get; set; }
        public DbSet<ArmorTypes> ArmorTypes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=sql.bsite.net\MSSQL2016;User ID=lizardrock_FaTDB;Password=88005553535Qwa;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
