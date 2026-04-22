namespace SWSE_CharacterCreator.Models.Entities;

public class Talent
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string TalentType { get; set; } = string.Empty;

    public string TalentTree { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> Prerequisites { get; set; } = new();
}