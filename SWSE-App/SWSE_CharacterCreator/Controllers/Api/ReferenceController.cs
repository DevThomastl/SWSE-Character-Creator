using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWSE_CharacterCreator.Data;
using SWSE_CharacterCreator.Services;

namespace SWSE_CharacterCreator.Controllers.Api;

// API for reference data
[ApiController]
[Route("api/reference")]
public class ReferenceController : ControllerBase
{
    private readonly SagaDbContext _db;
    private readonly HeroicClassRulesService _heroicRules;
    private readonly SpeciesRulesService _speciesRules;

    // inject db + rules services
    public ReferenceController(
        SagaDbContext db,
        HeroicClassRulesService heroicRules,
        SpeciesRulesService speciesRules)
    {
        _db = db;
        _heroicRules = heroicRules;
        _speciesRules = speciesRules;
    }

    // GET all species
    [HttpGet("species")]
    public async Task<IActionResult> GetSpecies()
    {
        var species = await _db.Species
            .Where(s => s.IsActive)
            .Include(s => s.TraitLinks)
                .ThenInclude(link => link.SpeciesTrait)
            .OrderBy(s => s.Name)
            .ToListAsync();

        var payload = species.Select(s => new
        {
            s.Id,
            s.Name,
            s.Size,
            s.Speed,
            s.StrMod,
            s.DexMod,
            s.ConMod,
            s.IntMod,
            s.WisMod,
            s.ChaMod,
            s.Languages,
            s.BonusLanguageChoices,

            BonusClassSkills = _speciesRules.GetBonusClassSkills(s),
            BonusFeatCount = _speciesRules.GetBonusFeatCount(s),
            BonusTrainedSkillCount = _speciesRules.GetBonusTrainedSkillCount(s),
            AutomaticBonusFeatNames = _speciesRules.GetAutomaticBonusFeatNames(s),
            ConditionalBonusFeatRules = _speciesRules.GetConditionalBonusFeatRules(s),

            // THIS was missing
            ChoiceBonusFeatRules = _speciesRules.GetChoiceBonusFeatRules(s),

            Traits = s.TraitLinks
                .OrderBy(l => l.SpeciesTrait!.Title)
                .Select(l => new
                {
                    l.SpeciesTraitId,
                    l.SpeciesTrait!.Title,
                    l.SpeciesTrait.Description
                })
                .ToList()
        }).ToList();

        return Ok(payload);
    }

    // GET all skills
    [HttpGet("skills")]
    public async Task<IActionResult> GetSkills()
    {
        var skills = await _db.Skills
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                s.Id,
                s.Name
            })
            .ToListAsync();

        return Ok(skills);
    }

    // GET heroic classes
    [HttpGet("heroicclasses")]
    public IActionResult GetHeroicClasses()
    {
        var classes = _heroicRules.GetAll()
            .Select(h => new
            {
                h.Name,
                h.TrainedSkillsAtLevel1,
                h.ClassSkills
            })
            .ToList();

        return Ok(classes);
    }

    // GET feats
    [HttpGet("feats")]
    public async Task<ActionResult<IEnumerable<object>>> GetFeats()
    {
        var feats = await _db.Feats
            .Select(f => new
            {
                f.Id,
                f.Name,
                f.Prerequisite,
                f.Description
            })
            .ToListAsync();

        return Ok(feats);
    }

    // GET talents
    [HttpGet("talents")]
    public async Task<ActionResult<IEnumerable<object>>> GetTalents()
    {
        var talents = await _db.Talents
            .Select(t => new
            {
                id = t.Id,
                name = t.Name,
                talentType = t.TalentType,
                talentTree = t.TalentTree,
                description = t.Description,
                prerequisites = t.Prerequisites
            })
            .ToListAsync();

        return Ok(talents);
    }

    // GET force powers
    [HttpGet("forcepowers")]
    public async Task<ActionResult<IEnumerable<object>>> GetForcePowers()
    {
        var forcePowers = await _db.ForcePowers
            .OrderBy(p => p.Name)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                powerType = p.PowerType,
                powerSubtype = p.PowerSubtype,
                form = p.Form,
                timeRequirement = p.TimeRequirement,
                description = p.Description
            })
            .ToListAsync();

        return Ok(forcePowers);
    }
}