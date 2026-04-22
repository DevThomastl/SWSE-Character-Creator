using SWSE_CharacterCreator.Models.Entities;
using SWSE_CharacterCreator.Rules;

namespace SWSE_CharacterCreator.Services;

// handles stat calculations
public static class DerivedStatsCalculator
{
    // converts ability score to modifier
    private static int Mod(int score) => (int)Math.Floor((score - 10) / 2.0);

    public static DerivedStats ComputeLevel1(Character c, Species? species, HeroicClassDto heroic)
    {
        // get ability modifiers
        var strMod = Mod(c.AbilityScores.Strength);
        var dexMod = Mod(c.AbilityScores.Dexterity);
        var conMod = Mod(c.AbilityScores.Constitution);
        var wisMod = Mod(c.AbilityScores.Wisdom);

        // defenses = 10 + ability mod + class bonus
        var fort = 10 + conMod + heroic.FortBonus;
        var reflex = 10 + dexMod + heroic.RefBonus;
        var will = 10 + wisMod + heroic.WillBonus;

        // damage threshold is equal to fortitude defense
        var dt = fort;

        // hp = class base + con mod (min 1)
        var hp = heroic.BaseHitPointsLevel1 + conMod;
        if (hp < 1) hp = 1;

        return new DerivedStats
        {
            // species speed or default
            Speed = species?.Speed ?? 6,

            // starting BAB
            BaseAttackBonus = 1,

            FortitudeDefense = fort,
            ReflexDefense = reflex,
            WillDefense = will,

            DamageThreshold = dt,
            TotalHp = hp,

            
            ForcePoints = 0,
            DestinyPoints = 0
        };
    }
}