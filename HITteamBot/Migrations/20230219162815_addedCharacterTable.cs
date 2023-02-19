using Microsoft.EntityFrameworkCore.Migrations;

namespace HITteamBot.Migrations
{
    public partial class addedCharacterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    Experience = table.Column<int>(nullable: false),
                    Level = table.Column<short>(nullable: false),
                    SPECIALsId = table.Column<long>(nullable: false),
                    Health = table.Column<int>(nullable: false),
                    CurrentHealth = table.Column<int>(nullable: false),
                    ActionPoints = table.Column<int>(nullable: false),
                    CurrentAP = table.Column<int>(nullable: false),
                    Rads = table.Column<short>(nullable: false),
                    EquipmentId = table.Column<long>(nullable: false),
                    Caps = table.Column<long>(nullable: false),
                    IsAlive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Character");
        }
    }
}
