namespace SWSE_CharacterCreator.Models.Entities;
using System.Collections.Generic;



public class Species
{

    

    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Size { get; set; } = null!;
    public int Speed { get; set; }

    // Ability score modifiers
    public int StrMod { get; set; }
    public int DexMod { get; set; }
    public int ConMod { get; set; }
    public int IntMod { get; set; }
    public int WisMod { get; set; }
    public int ChaMod { get; set; }

    public bool IsActive { get; set; } = true;


    public List<string> Languages { get; set; } = new();
    public List<string> BonusLanguageChoices { get; set; } = new();

    public ICollection<SpeciesTraitSpecies> TraitLinks { get; set; } = new List<SpeciesTraitSpecies>();

}
