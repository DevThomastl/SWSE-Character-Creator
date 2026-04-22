using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWSE_CharacterCreator.Data;
using SWSE_CharacterCreator.Models.Dto;
using SWSE_CharacterCreator.Models.Entities;
using SWSE_CharacterCreator.Services;

namespace SWSE_CharacterCreator.Controllers.Api;

// API controller for characters
[ApiController]
[Route("api/characters")]
public class CharactersApiController : ControllerBase
{
    private readonly SagaDbContext _db;
    private readonly SpeciesRulesService _speciesRules;

    public CharactersApiController(SagaDbContext db, SpeciesRulesService speciesRules)
    {
        _db = db;
        _speciesRules = speciesRules;
    }

    // GET all characters
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CharacterReadDto>>> GetAll()
    {
        var characters = await _db.Characters
            .Include(c => c.Species)
            .ThenInclude(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .ToListAsync();

        return characters.Select(ToReadDto).ToList();
    }

    // GET one character by id
    [HttpGet("{id}")]
    public async Task<ActionResult<CharacterReadDto>> Get(int id)
    {
        var character = await _db.Characters
            .Include(c => c.Species)
            .ThenInclude(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (character == null)
            return NotFound();

        return ToReadDto(character);
    }

    // CREATE new character
    [HttpPost]
    public async Task<ActionResult<CharacterReadDto>> Create(CharacterCreateDto dto)
    {
        var entity = new Character
        {
            Name = dto.Name,
            Sex = dto.Sex,
            Level = dto.Level,
            Age = dto.Age,
            HeightCm = dto.HeightCm,
            WeightKg = dto.WeightKg,

            SpeciesId = dto.SpeciesId,
            HeroicClassName = dto.HeroicClassName,
            EquipmentNotes = dto.EquipmentNotes,

            AbilityScores = dto.AbilityScores ?? new(),

            FeatIds = dto.FeatIds ?? new(),
            TalentIds = dto.TalentIds ?? new(),
            ForcePowerIds = dto.ForcePowerIds ?? new(),
            TrainedSkillIds = dto.TrainedSkillIds ?? new(),
            SkillFeatSelections = (dto.SkillFeatSelections ?? new())
                .Select(x => new SkillFeatSelection { FeatId = x.FeatId, SkillId = x.SkillId })
                .ToList(),

            SpeciesChosenFeats = dto.SpeciesChosenFeats ?? new(),

            BonusLanguages = dto.BonusLanguages ?? new(),
            Inventory = dto.Inventory ?? new()
        };

        entity.Species = await _db.Species
            .Include(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .FirstOrDefaultAsync(s => s.Id == entity.SpeciesId);

        CalculateDerivedStats(entity);

        _db.Characters.Add(entity);
        await _db.SaveChangesAsync();

        var created = await _db.Characters
            .Include(c => c.Species)
            .ThenInclude(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .FirstOrDefaultAsync(c => c.Id == entity.Id);

        if (created == null)
            return NotFound();

        return CreatedAtAction(nameof(Get), new { id = created.Id }, ToReadDto(created));
    }

    // UPDATE existing character
    [HttpPut("{id}")]
    public async Task<ActionResult<CharacterReadDto>> Update(int id, CharacterUpdateDto dto)
    {
        var entity = await _db.Characters
            .Include(c => c.Species)
            .ThenInclude(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (entity == null)
            return NotFound();

        entity.Name = dto.Name;
        entity.Sex = dto.Sex;
        entity.Level = dto.Level;
        entity.Age = dto.Age;
        entity.HeightCm = dto.HeightCm;
        entity.WeightKg = dto.WeightKg;

        entity.SpeciesId = dto.SpeciesId;
        entity.HeroicClassName = dto.HeroicClassName;
        entity.EquipmentNotes = dto.EquipmentNotes;

        entity.AbilityScores = dto.AbilityScores ?? new();

        entity.FeatIds = dto.FeatIds ?? new();
        entity.TalentIds = dto.TalentIds ?? new();
        entity.ForcePowerIds = dto.ForcePowerIds ?? new();
        entity.TrainedSkillIds = dto.TrainedSkillIds ?? new();
        entity.SkillFeatSelections = (dto.SkillFeatSelections ?? new())
            .Select(x => new SkillFeatSelection { FeatId = x.FeatId, SkillId = x.SkillId })
            .ToList();

        entity.SpeciesChosenFeats = dto.SpeciesChosenFeats ?? new();

        entity.BonusLanguages = dto.BonusLanguages ?? new();
        entity.Inventory = dto.Inventory ?? new();

        entity.Species = await _db.Species
            .Include(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .FirstOrDefaultAsync(s => s.Id == entity.SpeciesId);

        CalculateDerivedStats(entity);

        await _db.SaveChangesAsync();

        var updated = await _db.Characters
            .Include(c => c.Species)
            .ThenInclude(s => s.TraitLinks)
            .ThenInclude(link => link.SpeciesTrait)
            .FirstOrDefaultAsync(c => c.Id == entity.Id);

        if (updated == null)
            return NotFound();

        return Ok(ToReadDto(updated));
    }

    // DELETE character
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Characters.FindAsync(id);

        if (entity == null)
            return NotFound();

        _db.Characters.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // convert entity -> dto
    private CharacterReadDto ToReadDto(Character c)
    {
        var speciesGrantedFeats = new List<string>();

        if (c.Species != null)
        {
            var autoFeats = _speciesRules.GetAutomaticBonusFeatNames(c.Species);
            speciesGrantedFeats.AddRange(autoFeats);

            var conditionalRules = _speciesRules.GetConditionalBonusFeatRules(c.Species);
            foreach (var rule in conditionalRules)
            {
                if (ConditionMet(c, rule))
                    speciesGrantedFeats.Add(rule.Label);
            }
        }

        speciesGrantedFeats = speciesGrantedFeats
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var finalFeats = new List<string>();
        finalFeats.AddRange(GetFeatNamesFromIds(c.FeatIds));
        finalFeats.AddRange(speciesGrantedFeats);
        finalFeats.AddRange(c.SpeciesChosenFeats ?? new());

        finalFeats = finalFeats
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new CharacterReadDto
        {
            Id = c.Id,
            Name = c.Name,
            Sex = c.Sex,
            Level = c.Level,
            Age = c.Age,
            HeightCm = c.HeightCm,
            WeightKg = c.WeightKg,

            SpeciesId = c.SpeciesId,
            SpeciesName = c.Species?.Name,

            EquipmentNotes = c.EquipmentNotes,
            BonusLanguages = c.BonusLanguages,
            HeroicClassName = c.HeroicClassName,

            AbilityScores = c.AbilityScores,
            DerivedStats = c.DerivedStats,

            FeatIds = c.FeatIds,
            TalentIds = c.TalentIds,
            ForcePowerIds = c.ForcePowerIds,
            TrainedSkillIds = c.TrainedSkillIds,
            SkillFeatSelections = c.SkillFeatSelections
                .Select(x => new SkillFeatSelectionDto
                {
                    FeatId = x.FeatId,
                    SkillId = x.SkillId
                })
                .ToList(),

            SpeciesGrantedFeats = speciesGrantedFeats,
            SpeciesChosenFeats = c.SpeciesChosenFeats ?? new(),

            Inventory = c.Inventory,
            Credits = c.Credits,
            DarkSideScore = c.DarkSideScore
        };
    }

    private bool ConditionMet(Character c, ConditionalBonusFeatRuleDto rule)
    {
        if (!string.IsNullOrWhiteSpace(rule.RequiredTrainedSkill))
        {
            var trainedSkillId = GetSkillIdByName(rule.RequiredTrainedSkill);
            if (!trainedSkillId.HasValue || !c.TrainedSkillIds.Contains(trainedSkillId.Value))
                return false;
        }

        if (!string.IsNullOrWhiteSpace(rule.RequiredFeatName))
        {
            var featNames = GetFeatNamesFromIds(c.FeatIds);

            if (c.SpeciesChosenFeats != null)
                featNames.AddRange(c.SpeciesChosenFeats);

            if (!featNames.Any(f => f.Equals(rule.RequiredFeatName, StringComparison.OrdinalIgnoreCase)))
                return false;
        }

        if (rule.MinimumDexterity.HasValue && c.AbilityScores.Dexterity < rule.MinimumDexterity.Value)
            return false;

        if (rule.MinimumStrength.HasValue && c.AbilityScores.Strength < rule.MinimumStrength.Value)
            return false;

        if (rule.MinimumBaseAttackBonus.HasValue &&
            GetBaseAttackBonus(c.HeroicClassName) < rule.MinimumBaseAttackBonus.Value)
            return false;

        return true;
    }

    private int? GetSkillIdByName(string skillName)
    {
        var skill = _db.Skills.FirstOrDefault(s => s.Name == skillName);
        return skill?.Id;
    }

    private List<string> GetFeatNamesFromIds(List<int> featIds)
    {
        if (featIds == null || featIds.Count == 0)
            return new List<string>();

        var featNameById = _db.Feats
            .Where(f => featIds.Distinct().Contains(f.Id))
            .ToDictionary(f => f.Id, f => f.Name);

        return featIds
            .Where(id => featNameById.ContainsKey(id))
            .Select(id => featNameById[id])
            .ToList();
    }

    // class defense bonuses
    private static (int Fortitude, int Reflex, int Will) GetClassDefenseBonuses(string? heroicClassName)
    {
        if (string.IsNullOrWhiteSpace(heroicClassName))
            return (0, 0, 0);

        return heroicClassName.Trim().ToLowerInvariant() switch
        {
            "jedi" => (1, 1, 1),
            "noble" => (0, 1, 2),
            "scoundrel" => (0, 2, 1),
            "scout" => (1, 2, 0),
            "soldier" => (2, 1, 0),
            _ => (0, 0, 0)
        };
    }

    // starting HP by class
    private static int GetStartingHitPoints(string? heroicClassName)
    {
        if (string.IsNullOrWhiteSpace(heroicClassName))
            return 0;

        return heroicClassName.Trim().ToLowerInvariant() switch
        {
            "jedi" => 30,
            "noble" => 18,
            "scoundrel" => 18,
            "scout" => 24,
            "soldier" => 30,
            _ => 0
        };
    }

    // base attack bonus
    private static int GetBaseAttackBonus(string? heroicClassName)
    {
        if (string.IsNullOrWhiteSpace(heroicClassName))
            return 0;

        return heroicClassName.Trim().ToLowerInvariant() switch
        {
            "jedi" => 1,
            "soldier" => 1,
            _ => 0
        };
    }

    // ability modifier
    private static int GetModifier(int score)
    {
        return (score - 10) / 2;
    }

    // calculate derived stats
    private static void CalculateDerivedStats(Character c)
    {
        c.DerivedStats ??= new DerivedStats();
        c.AbilityScores ??= new AbilityScores();

        int level = c.Level;

        int dexMod = GetModifier(c.AbilityScores.Dexterity);
        int conMod = GetModifier(c.AbilityScores.Constitution);
        int wisMod = GetModifier(c.AbilityScores.Wisdom);

        var classBonus = GetClassDefenseBonuses(c.HeroicClassName);

        c.DerivedStats.ReflexDefense = 10 + level + dexMod + classBonus.Reflex;
        c.DerivedStats.FortitudeDefense = 10 + level + conMod + classBonus.Fortitude;
        c.DerivedStats.WillDefense = 10 + level + wisMod + classBonus.Will;

        c.DerivedStats.HitPoints = GetStartingHitPoints(c.HeroicClassName) + conMod;
        c.DerivedStats.BaseAttackBonus = GetBaseAttackBonus(c.HeroicClassName);

        c.DerivedStats.DamageThreshold = c.DerivedStats.FortitudeDefense;
        c.DerivedStats.Speed = c.Species?.Speed ?? 0;
        c.DerivedStats.ForcePoints = 5;
        c.DerivedStats.DestinyPoints = 1;
    }
}