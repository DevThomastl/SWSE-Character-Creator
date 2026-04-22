using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWSE_CharacterCreator.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeciesChosenFeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpeciesChosenFeatsJson",
                table: "Characters",
                type: "TEXT",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpeciesChosenFeatsJson",
                table: "Characters");
        }
    }
}
