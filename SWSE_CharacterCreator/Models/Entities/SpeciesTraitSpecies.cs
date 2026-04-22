namespace SWSE_CharacterCreator.Models.Entities;

// many-to-many between Species and SpeciesTrait
public class SpeciesTraitSpecies
{
    public int SpeciesId { get; set; }
    public Species Species { get; set; } = null!;

    public int SpeciesTraitId { get; set; }
    public SpeciesTrait SpeciesTrait { get; set; } = null!;
}