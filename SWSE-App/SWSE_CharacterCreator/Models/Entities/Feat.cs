using System.ComponentModel.DataAnnotations;

namespace SWSE_CharacterCreator.Models.Entities;

public class Feat
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    // Allow null (for JSON)
    [MaxLength(500)]
    public string? Prerequisite { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;
}