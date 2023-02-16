using HITteamBot.Repository.Entities.Characters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.EntitesDB
{
    public class DatabaseContext : DbContext
    {
        public DbSet<SPECIAL> SPECIAL { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=sql.bsite.net\MSSQL2016;User ID=lizardrock_FaTDB;Password=FQhfn160308;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
