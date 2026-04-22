using System.Text.Json;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Data.Seed;

// seeds skills into the db
public static class SkillSeeder
{
    public static void Seed(SagaDbContext context, IWebHostEnvironment env)
    {
        // stop if already seeded
        if (context.Skills.Any())
            return;

        // path to skills.json
        var path = Path.Combine(env.ContentRootPath, "Data", "Seed", "skills.json");

        // make sure file exists
        if (!File.Exists(path))
            throw new FileNotFoundException("skills.json not found", path);

        // read file
        var json = File.ReadAllText(path);

        // convert json to list of skills
        var skills = JsonSerializer.Deserialize<List<Skill>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // make sure data loaded
        if (skills == null || skills.Count == 0)
            throw new InvalidOperationException("No skills loaded");

        // save to db
        context.Skills.AddRange(skills);
        context.SaveChanges();
    }
}