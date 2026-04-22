using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWSE_CharacterCreator.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqliteCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Prerequisite = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForcePowers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PowerType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PowerSubtype = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Form = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TimeRequirement = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForcePowers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Speed = table.Column<int>(type: "INTEGER", nullable: false),
                    StrMod = table.Column<int>(type: "INTEGER", nullable: false),
                    DexMod = table.Column<int>(type: "INTEGER", nullable: false),
                    ConMod = table.Column<int>(type: "INTEGER", nullable: false),
                    IntMod = table.Column<int>(type: "INTEGER", nullable: false),
                    WisMod = table.Column<int>(type: "INTEGER", nullable: false),
                    ChaMod = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Languages = table.Column<string>(type: "TEXT", nullable: false),
                    BonusLanguageChoices = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesTraits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesTraits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Talents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TalentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TalentTree = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Prerequisites = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Sex = table.Column<byte>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: true),
                    HeightCm = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    WeightKg = table.Column<decimal>(type: "TEXT", precision: 6, scale: 2, nullable: true),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: true),
                    EquipmentNotes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    HeroicClassName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HeroicClassId = table.Column<int>(type: "INTEGER", nullable: true),
                    DarkSideScore = table.Column<int>(type: "INTEGER", nullable: false),
                    Credits = table.Column<long>(type: "INTEGER", nullable: false),
                    AbilityScores_Strength = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityScores_Dexterity = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityScores_Constitution = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityScores_Intelligence = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityScores_Wisdom = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityScores_Charisma = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_TotalHp = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_DamageThreshold = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_ForcePoints = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_DestinyPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_Speed = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_HitPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_BaseAttackBonus = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_FortitudeDefense = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_ReflexDefense = table.Column<int>(type: "INTEGER", nullable: false),
                    DerivedStats_WillDefense = table.Column<int>(type: "INTEGER", nullable: false),
                    FeatIdsCsv = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    TalentIdsCsv = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ForcePowerIdsCsv = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    TrainedSkillIdsCsv = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    SkillFeatSelectionsJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    BonusLanguages = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesTraitSpecies",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpeciesTraitId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesTraitSpecies", x => new { x.SpeciesId, x.SpeciesTraitId });
                    table.ForeignKey(
                        name: "FK_SpeciesTraitSpecies_SpeciesTraits_SpeciesTraitId",
                        column: x => x.SpeciesTraitId,
                        principalTable: "SpeciesTraits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeciesTraitSpecies_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInventory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterInventory_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventory_CharacterId",
                table: "CharacterInventory",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_SpeciesId",
                table: "Characters",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Feats_Name",
                table: "Feats",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ForcePowers_Name",
                table: "ForcePowers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                table: "Species",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesTraits_Title",
                table: "SpeciesTraits",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesTraitSpecies_SpeciesTraitId",
                table: "SpeciesTraitSpecies",
                column: "SpeciesTraitId");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Name",
                table: "Talents",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterInventory");

            migrationBuilder.DropTable(
                name: "Feats");

            migrationBuilder.DropTable(
                name: "ForcePowers");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "SpeciesTraitSpecies");

            migrationBuilder.DropTable(
                name: "Talents");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "SpeciesTraits");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
