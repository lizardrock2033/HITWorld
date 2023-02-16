using Microsoft.EntityFrameworkCore.Migrations;

namespace HITteamBot.Migrations
{
    public partial class @base : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPECIAL",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Strength = table.Column<short>(nullable: false),
                    Perception = table.Column<short>(nullable: false),
                    Endurance = table.Column<short>(nullable: false),
                    Charisma = table.Column<short>(nullable: false),
                    Intellegence = table.Column<short>(nullable: false),
                    Agility = table.Column<short>(nullable: false),
                    Luck = table.Column<short>(nullable: false),
                    IsSet = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPECIAL", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPECIAL");
        }
    }
}
