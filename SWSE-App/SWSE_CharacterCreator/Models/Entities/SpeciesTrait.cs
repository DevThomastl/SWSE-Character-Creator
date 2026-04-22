using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SWSE_CharacterCreator.Models.Entities;

public class SpeciesTrait
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(8000)]
    public string Description { get; set; } = string.Empty;

    
    public ICollection<SpeciesTraitSpecies> SpeciesLinks { get; set; } = new List<SpeciesTraitSpecies>();

}