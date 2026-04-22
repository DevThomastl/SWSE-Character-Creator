using SWSE_CharacterCreator.Data.Seed;

namespace SWSE_CharacterCreator.Data;

public static class DbSeeder
{
    public static void SeedAll(SagaDbContext context, IWebHostEnvironment env)
    {
        // Order matters if future seeders depend on earlier ones
        SpeciesSeeder.Seed(context, env);
        SpeciesTraitSeeder.Seed(context, env);
        FeatSeeder.Seed(context, env);
        SkillSeeder.Seed(context, env);
    }
}
