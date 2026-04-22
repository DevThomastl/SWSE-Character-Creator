using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Models.Dto;

// used when creating a character
public class CharacterCreateDto
{
    public string? EquipmentNotes { get; set; }

    public Dictionary<int, string>? SkillFocusSelections { get; set; }
    public List<string>? SpeciesGrantedFeats { get; set; }
    public List<string>? SpeciesChosenFeats { get; set; }

    public string Name { get; set; } = string.Empty;
    public Sex Sex { get; set; } = Sex.Unspecified;
    public int Level { get; set; } = 1;

    // optional bio info
    public int? Age { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }

    // selected species
    public int? SpeciesId { get; set; }

    // class name
    public string HeroicClassName { get; set; } = "Soldier";

    // ability scores input
    public AbilityScores AbilityScores { get; set; } = new();

    // selections
    public List<string>? BonusLanguages { get; set; }
    public List<int>? FeatIds { get; set; }
    public List<int>? TalentIds { get; set; }
    public List<int>? ForcePowerIds { get; set; }
    public List<int>? TrainedSkillIds { get; set; }
    public List<SkillFeatSelectionDto>? SkillFeatSelections { get; set; }

    // inventory items
    public List<InventoryEntry>? Inventory { get; set; }
}

// used when updating (same as create)
public sealed class CharacterUpdateDto : CharacterCreateDto
{
}

// used when returning character data
public sealed class CharacterReadDto
{
    public string? EquipmentNotes { get; set; }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Sex Sex { get; set; }
    public int Level { get; set; }

    // optional bio info
    public int? Age { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }

    // species info
    public int? SpeciesId { get; set; }
    public string? SpeciesName { get; set; }

    public List<string> BonusLanguages { get; set; } = new();
    public string HeroicClassName { get; set; } = "Soldier";

    // full stats
    public AbilityScores AbilityScores { get; set; } = new();
    public DerivedStats DerivedStats { get; set; } = new();

    // selections
    public List<int> FeatIds { get; set; } = new();
    public List<int> TalentIds { get; set; } = new();
    public List<int> ForcePowerIds { get; set; } = new();
    public List<int> TrainedSkillIds { get; set; } = new();
    public List<SkillFeatSelectionDto> SkillFeatSelections { get; set; } = new();

    public List<string> SpeciesGrantedFeats { get; set; } = new();
    public List<string> SpeciesChosenFeats { get; set; } = new();

    // inventory + other values
    public List<InventoryEntry> Inventory { get; set; } = new();
    public long Credits { get; set; }
    public int DarkSideScore { get; set; }
}

public sealed class SkillFeatSelectionDto
{
    public int FeatId { get; set; }
    public int SkillId { get; set; }
}