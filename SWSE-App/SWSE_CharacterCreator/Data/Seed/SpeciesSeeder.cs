using System.Text.Json;
using System.Text.Json.Serialization;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Data.Seed;

public static class SpeciesSeeder
{
    // Species that dont start with the Basic language
    private static readonly HashSet<string> SpeciesWithoutBasic = new(StringComparer.OrdinalIgnoreCase)
    {
        "Wookiee",
        "Ewok",
        "Gamorrean",
        "Jawa",
        "Tusken Raider",
        "Hutt",
        "Yuuzhan Vong",
        "Killik"
    };

    
    private sealed class SpeciesSeedDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public string Size { get; set; } = string.Empty;

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("strengthModifier")]
        public int StrengthModifier { get; set; }

        [JsonPropertyName("dexterityModifier")]
        public int DexterityModifier { get; set; }

        [JsonPropertyName("constitutionModifier")]
        public int ConstitutionModifier { get; set; }

        [JsonPropertyName("intelligenceModifier")]
        public int IntelligenceModifier { get; set; }

        [JsonPropertyName("wisdomModifier")]
        public int WisdomModifier { get; set; }

        [JsonPropertyName("charismaModifier")]
        public int CharismaModifier { get; set; }

        [JsonPropertyName("languages")]
        public List<string> Languages { get; set; } = new();

        [JsonPropertyName("bonusLanguageChoices")]
        public List<string> BonusLanguageChoices { get; set; } = new();
    }

    public static void Seed(SagaDbContext context, IWebHostEnvironment env)
    {
        // path to species.json
        var path = Path.Combine(env.ContentRootPath, "Data", "Seed", "species.json");

        // Make sure file exists
        if (!File.Exists(path))
            throw new FileNotFoundException("species.json not found", path);

        var json = File.ReadAllText(path);

        // JSON into DTO list
        var dtos = JsonSerializer.Deserialize<List<SpeciesSeedDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        
        if (dtos == null || dtos.Count == 0)
            throw new InvalidOperationException("No species loaded from species.json");

        foreach (var d in dtos)
        {
            
            var languages = d.Languages ?? new List<string>();

            // Add Basic if species should have it and doesn't already
            if (!SpeciesWithoutBasic.Contains(d.Name) &&
                !languages.Any(l => l.Equals("Basic", StringComparison.OrdinalIgnoreCase)))
            {
                languages.Add("Basic");
            }

           
            var cleanedLanguages = languages
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(l => l)
                .ToList();

           
            var cleanedBonusLanguages = (d.BonusLanguageChoices ?? new List<string>())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(l => l)
                .ToList();

            // Check if species already exists in DB
            var existing = context.Species.FirstOrDefault(s => s.Name == d.Name.Trim());

            if (existing == null)
            {
                // Create new species
                context.Species.Add(new Species
                {
                    Name = d.Name.Trim(),
                    Size = d.Size.Trim(),
                    Speed = d.Speed,
                    StrMod = d.StrengthModifier,
                    DexMod = d.DexterityModifier,
                    ConMod = d.ConstitutionModifier,
                    IntMod = d.IntelligenceModifier,
                    WisMod = d.WisdomModifier,
                    ChaMod = d.CharismaModifier,
                    Languages = cleanedLanguages,
                    BonusLanguageChoices = cleanedBonusLanguages,
                    IsActive = true
                });
            }
            else
            {
                // Update existing species
                existing.Size = d.Size.Trim();
                existing.Speed = d.Speed;
                existing.StrMod = d.StrengthModifier;
                existing.DexMod = d.DexterityModifier;
                existing.ConMod = d.ConstitutionModifier;
                existing.IntMod = d.IntelligenceModifier;
                existing.WisMod = d.WisdomModifier;
                existing.ChaMod = d.CharismaModifier;
                existing.Languages = cleanedLanguages;
                existing.BonusLanguageChoices = cleanedBonusLanguages;
                existing.IsActive = true;
            }
        }

        // Save all changes to DB
        context.SaveChanges();
    }
}