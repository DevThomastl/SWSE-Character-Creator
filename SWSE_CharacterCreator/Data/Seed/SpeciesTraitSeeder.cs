using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Data.Seed;

// seeds species traits + optional links to species
public static class SpeciesTraitSeeder
{
    // matches json structure
    private sealed class TraitDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        // optional list of species names to link
        [JsonPropertyName("speciesNames")]
        public List<string>? SpeciesNames { get; set; }
    }

    public static void Seed(SagaDbContext context, IWebHostEnvironment env)
    {
        // stop if already seeded
        if (context.SpeciesTraits.Any())
            return;

        // path to json
        var path = Path.Combine(env.ContentRootPath, "Data", "Seed", "speciestraits.json");

        // if file doesn't exist, just skip
        if (!File.Exists(path))
            return;

        // read file
        var json = File.ReadAllText(path);

        // convert json to objects
        var traits = JsonSerializer.Deserialize<List<TraitDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // nothing loaded
        if (traits == null || traits.Count == 0)
            return;

        // create trait entities
        var traitEntities = traits
            .Select(t => new SpeciesTrait
            {
                Title = t.Title.Trim(),
                Description = t.Description.Trim()
            })
            .OrderBy(t => t.Title)
            .ToList();

        // save traits
        context.SpeciesTraits.AddRange(traitEntities);
        context.SaveChanges();

        // link traits to species (if provided)
        foreach (var dto in traits)
        {
            // skip if no species listed
            if (dto.SpeciesNames == null || dto.SpeciesNames.Count == 0)
                continue;

            // find trait
            var trait = context.SpeciesTraits.SingleOrDefault(x => x.Title == dto.Title.Trim());
            if (trait == null) continue;

            foreach (var speciesName in dto.SpeciesNames)
            {
                var sName = speciesName.Trim();

                // find species
                var species = context.Species.SingleOrDefault(s => s.Name == sName);
                if (species == null) continue;

                // check if link already exists
                var exists = context.SpeciesTraitSpecies.Any(l =>
                    l.SpeciesId == species.Id && l.SpeciesTraitId == trait.Id);

                // add link if missing
                if (!exists)
                {
                    context.SpeciesTraitSpecies.Add(new SpeciesTraitSpecies
                    {
                        SpeciesId = species.Id,
                        SpeciesTraitId = trait.Id
                    });
                }
            }
        }

        // save links
        context.SaveChanges();
    }
}