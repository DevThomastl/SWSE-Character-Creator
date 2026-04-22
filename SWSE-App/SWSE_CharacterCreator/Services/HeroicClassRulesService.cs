using System.Text.Json;
using SWSE_CharacterCreator.Rules;

namespace SWSE_CharacterCreator.Services;

// loads and provides heroic class rules
public sealed class HeroicClassRulesService
{
    private readonly IWebHostEnvironment _env;
    private HeroicClassesRoot? _cache; // cache so we only load once

    public HeroicClassRulesService(IWebHostEnvironment env)
    {
        _env = env;
    }

    // get all rules
    // will load from file as a secondary measure
    public HeroicClassesRoot GetRules()
    {
        if (_cache != null) return _cache;

        // json file path
        var path = Path.Combine(_env.ContentRootPath, "Data", "Seed", "heroic_classes.json");

        // makes sure file exists
        if (!File.Exists(path))
            throw new FileNotFoundException("heroic_classes.json not found", path);

        // reads the file
        var json = File.ReadAllText(path);

        // convert json to object
        var root = JsonSerializer.Deserialize<HeroicClassesRoot>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // makes sure the data loaded
        if (root == null || root.HeroicClasses == null || root.HeroicClasses.Count == 0)
            throw new InvalidOperationException("No heroic classes loaded from heroic_classes.json");

        _cache = root;
        return _cache;
    }

    // get one class by name
    public HeroicClassDto GetByNameOrThrow(string? name)
    {
        var rules = GetRules();

        // soldier is the default if value is empty
        var className = string.IsNullOrWhiteSpace(name) ? "Soldier" : name.Trim();

        var heroic = rules.HeroicClasses.SingleOrDefault(h =>
            h.Name.Equals(className, StringComparison.OrdinalIgnoreCase));

        // throws if it is not found
        if (heroic == null)
            throw new InvalidOperationException(
                $"Invalid HeroicClassName '{className}'. Must be one of: {string.Join(", ", rules.HeroicClasses.Select(h => h.Name))}");

        return heroic;
    }

    // gets all classes
    public List<HeroicClassDto> GetAll()
    {
        return GetRules().HeroicClasses;
    }
}