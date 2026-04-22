using Microsoft.EntityFrameworkCore;
using SWSE_CharacterCreator.Models.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SWSE_CharacterCreator.Data;

// main db context
public class SagaDbContext : DbContext
{
    public SagaDbContext(DbContextOptions<SagaDbContext> options)
        : base(options)
    {
    }

    // db tables
    public DbSet<Species> Species => Set<Species>();
    public DbSet<Feat> Feats => Set<Feat>();
    public DbSet<Talent> Talents => Set<Talent>();
    public DbSet<ForcePower> ForcePowers => Set<ForcePower>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Skill> Skills { get; set; }
    public DbSet<SpeciesTrait> SpeciesTraits => Set<SpeciesTrait>();
    public DbSet<SpeciesTraitSpecies> SpeciesTraitSpecies => Set<SpeciesTraitSpecies>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // handles converting List<string> to json and then back
        var stringListConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => string.IsNullOrWhiteSpace(v)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        base.OnModelCreating(modelBuilder);

        // ---------- Species ----------
        modelBuilder.Entity<Species>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Size).IsRequired().HasMaxLength(20);

            // store language lists as json
            e.Property(x => x.Languages).HasConversion(stringListConverter);
            e.Property(x => x.BonusLanguageChoices).HasConversion(stringListConverter);

            // make sure species names are unique
            e.HasIndex(x => x.Name).IsUnique();
        });

        // Feat Entity
        modelBuilder.Entity<Feat>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Prerequisite).HasMaxLength(400);
            e.Property(x => x.Description).IsRequired().HasMaxLength(2000);

            e.HasIndex(x => x.Name);
        });

        // Talent Entity
        modelBuilder.Entity<Talent>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.TalentType).IsRequired().HasMaxLength(100);
            e.Property(x => x.TalentTree).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).IsRequired().HasMaxLength(4000);

            // store prerequisites list as json
            e.Property(x => x.Prerequisites).HasConversion(stringListConverter);

            e.HasIndex(x => x.Name);
        });

        // Character Entity
        modelBuilder.Entity<Character>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.EquipmentNotes).HasMaxLength(4000);

            
            e.Property(x => x.HeightCm).HasPrecision(6, 2);
            e.Property(x => x.WeightKg).HasPrecision(6, 2);

            // store bonus languages as json
            e.Property(x => x.BonusLanguages).HasConversion(stringListConverter);

            // link character to species
            e.HasOne(x => x.Species)
                .WithMany()
                .HasForeignKey(x => x.SpeciesId)
                .OnDelete(DeleteBehavior.SetNull);

            
            e.OwnsOne(x => x.AbilityScores);
            e.OwnsOne(x => x.DerivedStats);

            // stores inventory in a separate table
            e.OwnsMany(x => x.Inventory, inv =>
            {
                inv.WithOwner().HasForeignKey("CharacterId");

                inv.Property<int>("Id");
                inv.HasKey("Id");

                inv.Property(x => x.Name).IsRequired().HasMaxLength(120);
                inv.Property(x => x.Quantity);

                inv.ToTable("CharacterInventory");
            });
        });

        // SpeciesTrait Entity
        modelBuilder.Entity<SpeciesTrait>(e =>
        {
            e.Property(x => x.Title).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).IsRequired().HasMaxLength(8000);

            e.HasIndex(x => x.Title).IsUnique();
        });

        // Species Traits Entity
        modelBuilder.Entity<SpeciesTraitSpecies>(e =>
        {
            // composite key for join table
            e.HasKey(x => new { x.SpeciesId, x.SpeciesTraitId });

            e.HasOne(x => x.Species)
                .WithMany(s => s.TraitLinks)
                .HasForeignKey(x => x.SpeciesId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.SpeciesTrait)
                .WithMany(t => t.SpeciesLinks)
                .HasForeignKey(x => x.SpeciesTraitId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ForcePower Entity
        modelBuilder.Entity<ForcePower>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.PowerType).IsRequired().HasMaxLength(100);
            e.Property(x => x.PowerSubtype).HasMaxLength(100);
            e.Property(x => x.Form).HasMaxLength(100);
            e.Property(x => x.TimeRequirement).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).IsRequired().HasMaxLength(4000);

            e.HasIndex(x => x.Name);
        });
    }
}