namespace SWSE_CharacterCreator.Models.Entities;

public class ForcePower
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string PowerType { get; set; } = string.Empty;

    public string? PowerSubtype { get; set; }

    public string? Form { get; set; }

    public string TimeRequirement { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}