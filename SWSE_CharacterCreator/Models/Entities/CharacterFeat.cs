using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Models.Entities;

public class CharacterFeat
{
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;

    public int FeatId { get; set; }
    public Feat Feat { get; set; } = null!;
}