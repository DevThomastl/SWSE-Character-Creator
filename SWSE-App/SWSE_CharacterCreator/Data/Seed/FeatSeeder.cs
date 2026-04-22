using System.Text.Json;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Data.Seed;

public static class FeatSeeder
{
    public static void Seed(SagaDbContext context, IWebHostEnvironment env)
    {
        if (context.Feats.Any())
            return;

        var path = Path.Combine(env.ContentRootPath, "Data", "Seed", "feats.json");

        if (!File.Exists(path))
            throw new FileNotFoundException("feats.json not found", path);

        var json = File.ReadAllText(path);

        var feats = JsonSerializer.Deserialize<List<Feat>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (feats == null || feats.Count == 0)
            throw new InvalidOperationException("No feats loaded from feats.json");

        var orderedFeats = feats.OrderBy(f => f.Name).ToList();

        foreach (var feat in orderedFeats)
        {
            feat.Id = 0;
        }

        context.Feats.AddRange(orderedFeats);
        context.SaveChanges();
    }
}