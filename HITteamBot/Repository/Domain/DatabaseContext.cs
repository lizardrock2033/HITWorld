using HITteamBot.Repository.Entities.Characters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain
{
    public class DatabaseContext : DbContext
    {
        public DbSet<SPECIAL> SPECIAL { get; set; }
        public DbSet<Character> Character { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=sql.bsite.net\MSSQL2016;User ID=lizardrock_FaTDB;Password=88005553535Qwa;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
