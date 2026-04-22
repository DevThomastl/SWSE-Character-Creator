using System.Text.Json;
using SWSE_CharacterCreator.Data;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Services;

// seeds talents into the db
public static class TalentSeeder
{
    public static async Task SeedAsync(SagaDbContext context, IWebHostEnvironment env)
    {
        // stop if already seeded
        if (context.Talents.Any())
            return;

        // path to json file
        var filePath = Path.Combine(env.ContentRootPath, "Data", "Seed", "talents.json");

        // make sure file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine("TalentSeeder: talents.json not found.");
            return;
        }

        // read file
        var json = await File.ReadAllTextAsync(filePath);

        // allow case-insensitive json
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // convert json to objects
        var seedItems = JsonSerializer.Deserialize<List<TalentSeedModel>>(json, options) ?? new();

        // map to entity
        var talents = seedItems
            .Select(t => new Talent
            {
                Name = t.Name?.Trim() ?? string.Empty,
                TalentType = t.TalentType?.Trim() ?? string.Empty,
                TalentTree = t.TalentTree?.Trim() ?? string.Empty,
                Description = t.Description?.Trim() ?? string.Empty,
                Prerequisites = t.Prerequisites ?? new List<string>()
            })
            // remove invalid entries
            .Where(t =>
                !string.IsNullOrWhiteSpace(t.Name) &&
                !string.IsNullOrWhiteSpace(t.TalentType) &&
                !string.IsNullOrWhiteSpace(t.TalentTree) &&
                !string.IsNullOrWhiteSpace(t.Description))
            .ToList();

        // nothing valid found
        if (talents.Count == 0)
        {
            Console.WriteLine("TalentSeeder: No valid talents found in JSON.");
            return;
        }

        // save to db
        context.Talents.AddRange(talents);
        await context.SaveChangesAsync();

        Console.WriteLine($"TalentSeeder: Seeded {talents.Count} talents.");
    }

    // temp model for reading json
    private sealed class TalentSeedModel
    {
        public string? Name { get; set; }
        public string? TalentType { get; set; }
        public string? TalentTree { get; set; }
        public string? Description { get; set; }
        public List<string>? Prerequisites { get; set; }
    }
}