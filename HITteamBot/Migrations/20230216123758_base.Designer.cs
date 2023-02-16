﻿// <auto-generated />
using HITteamBot.Repository.EntitesDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HITteamBot.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230216123758_base")]
    partial class @base
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.32")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HITteamBot.Repository.Entities.Characters.SPECIAL", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Agility")
                        .HasColumnType("smallint");

                    b.Property<short>("Charisma")
                        .HasColumnType("smallint");

                    b.Property<short>("Endurance")
                        .HasColumnType("smallint");

                    b.Property<short>("Intellegence")
                        .HasColumnType("smallint");

                    b.Property<bool>("IsSet")
                        .HasColumnType("bit");

                    b.Property<short>("Luck")
                        .HasColumnType("smallint");

                    b.Property<short>("Perception")
                        .HasColumnType("smallint");

                    b.Property<short>("Strength")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("SPECIAL");
                });
#pragma warning restore 612, 618
        }
    }
}