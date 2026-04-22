namespace SWSE_CharacterCreator.Rules;

public sealed class HeroicClassesRoot
{
    public int BaseForcePoints { get; set; }
    public string ForcePointsFormula { get; set; } = "5 + floor(characterLevel / 2)";
    public List<HeroicClassDto> HeroicClasses { get; set; } = new();
}

public sealed class HeroicClassDto
{
    public string Name { get; set; } = "Soldier";
    public int BaseHitPointsLevel1 { get; set; }
    public int FortBonus { get; set; }
    public int RefBonus { get; set; }
    public int WillBonus { get; set; }
    public int TrainedSkillsAtLevel1 { get; set; }
    public List<string> ClassSkills { get; set; } = new();
    public List<string> StartingFeatNames { get; set; } = new();
    public string StartingCreditsExpression { get; set; } = "3d4x250";
}