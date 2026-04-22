using System.Text.Json;
using SWSE_CharacterCreator.Data;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Services;

// seeds force powers into the database
public static class ForcePowerSeeder
{
    public static async Task SeedAsync(SagaDbContext context, IWebHostEnvironment env)
    {
        // stop if already seeded
        if (context.ForcePowers.Any())
            return;

        // path to json file
        var filePath = Path.Combine(env.ContentRootPath, "Data", "Seed", "forcePowers.json");

        // make sure file exists
        if (!File.Exists(filePath))
        {
            Console.WriteLine("ForcePowerSeeder: forcePowers.json not found.");
            return;
        }

        // read json file
        var json = await File.ReadAllTextAsync(filePath);

        // allow case-insensitive json properties
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // convert json into objects
        var seedItems = JsonSerializer.Deserialize<List<ForcePowerSeedModel>>(json, options) ?? new();

        // map to entity
        var forcePowers = seedItems
            .Select(p => new ForcePower
            {
                Name = p.Name?.Trim() ?? string.Empty,
                PowerType = p.PowerType?.Trim() ?? string.Empty,
                PowerSubtype = string.IsNullOrWhiteSpace(p.PowerSubtype) ? null : p.PowerSubtype.Trim(),
                Form = string.IsNullOrWhiteSpace(p.Form) ? null : p.Form.Trim(),
                TimeRequirement = p.TimeRequirement?.Trim() ?? string.Empty,
                Description = p.Description?.Trim() ?? string.Empty
            })
            // remove invalid entries
            .Where(p =>
                !string.IsNullOrWhiteSpace(p.Name) &&
                !string.IsNullOrWhiteSpace(p.PowerType) &&
                !string.IsNullOrWhiteSpace(p.TimeRequirement) &&
                !string.IsNullOrWhiteSpace(p.Description))
            .ToList();

        // nothing valid found
        if (forcePowers.Count == 0)
        {
            Console.WriteLine("ForcePowerSeeder: No valid force powers found in JSON.");
            return;
        }

        // save to db
        context.ForcePowers.AddRange(forcePowers);
        await context.SaveChangesAsync();

        Console.WriteLine($"ForcePowerSeeder: Seeded {forcePowers.Count} force powers.");
    }

    // temp model for reading json
    private sealed class ForcePowerSeedModel
    {
        public string? Name { get; set; }
        public string? PowerType { get; set; }
        public string? PowerSubtype { get; set; }
        public string? Form { get; set; }
        public string? TimeRequirement { get; set; }
        public string? Description { get; set; }
    }
}