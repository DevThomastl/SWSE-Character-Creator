using System.Text.RegularExpressions;
using SWSE_CharacterCreator.Models.Entities;

namespace SWSE_CharacterCreator.Services;

public sealed class SpeciesRulesService
{
    public List<string> GetBonusClassSkills(Species species)
    {
        var result = new List<string>();

        foreach (var title in GetTraitTitles(species))
        {
            if (!title.StartsWith("Bonus Class Skill", StringComparison.OrdinalIgnoreCase))
                continue;

            var skillName = ExtractParenValue(title);
            if (!string.IsNullOrWhiteSpace(skillName))
                result.Add(skillName);
        }

        return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public int GetBonusFeatCount(Species species)
    {
        return GetTraitTitles(species)
            .Count(title => title.Equals("Bonus Feat", StringComparison.OrdinalIgnoreCase));
    }

    public int GetBonusTrainedSkillCount(Species species)
    {
        return GetTraitTitles(species)
            .Count(title => title.Equals("Bonus Trained Skill", StringComparison.OrdinalIgnoreCase));
    }

    public List<string> GetAutomaticBonusFeatNames(Species species)
    {
        var result = new List<string>();

        foreach (var link in species.TraitLinks ?? Enumerable.Empty<SpeciesTraitSpecies>())
        {
            var title = link.SpeciesTrait?.Title?.Trim() ?? string.Empty;
            var description = link.SpeciesTrait?.Description?.Trim() ?? string.Empty;

            if (IsChoiceBonusFeat(species, title))
                continue;

            if (title.StartsWith("Bonus Feat (", StringComparison.OrdinalIgnoreCase))
            {
                var featName = ExtractParenValue(title);
                if (!string.IsNullOrWhiteSpace(featName))
                    result.Add(featName);
            }

            if (description.Contains("gain the Force Training feat as a bonus Feat", StringComparison.OrdinalIgnoreCase))
            {
                result.Add("Force Training");
            }
        }

        return result.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
    }

    public List<SpeciesChoiceBonusFeatRuleDto> GetChoiceBonusFeatRules(Species species)
    {
        var rules = new List<SpeciesChoiceBonusFeatRuleDto>();

        if (species.Name.Equals("Tof", StringComparison.OrdinalIgnoreCase))
        {
            rules.Add(new SpeciesChoiceBonusFeatRuleDto
            {
                ChoiceKey = "TofBonusFeatChoice",
                Label = "Choose 1 Bonus Feat",
                Options = new List<string>
                {
                    "Rapid Shot",
                    "Rapid Strike"
                }
            });
        }

        return rules;
    }

    public bool IsSpeciesChoiceFeatOption(Species species, string featName)
    {
        return GetChoiceBonusFeatRules(species)
            .SelectMany(rule => rule.Options)
            .Any(option => option.Equals(featName, StringComparison.OrdinalIgnoreCase));
    }

    public List<ConditionalBonusFeatRuleDto> GetConditionalBonusFeatRules(Species species)
    {
        var rules = new List<ConditionalBonusFeatRuleDto>();

        foreach (var link in species.TraitLinks ?? Enumerable.Empty<SpeciesTraitSpecies>())
        {
            var title = link.SpeciesTrait?.Title?.Trim() ?? string.Empty;
            var description = link.SpeciesTrait?.Description?.Trim() ?? string.Empty;

            if (!title.StartsWith("Conditional Bonus Feat", StringComparison.OrdinalIgnoreCase))
                continue;

            var grantedName = ExtractParenValue(title);
            if (string.IsNullOrWhiteSpace(grantedName))
                continue;

            var rule = new ConditionalBonusFeatRuleDto
            {
                FeatName = NormalizeGrantedFeatName(grantedName),
                Label = grantedName
            };

            if (grantedName.StartsWith("Skill Focus (", StringComparison.OrdinalIgnoreCase))
            {
                rule.SkillName = ExtractNestedSkillFocusSkillName(grantedName);
                rule.Label = $"Skill Focus ({rule.SkillName})";
            }

            var trainedSkillMatch = Regex.Match(
                description,
                @"with\s+(.+?)\s+as\s+a\s+Trained\s+Skill|who\s+has\s+(.+?)\s+as\s+a\s+Trained\s+Skill|has\s+(.+?)\s+as\s+a\s+Trained\s+Skill",
                RegexOptions.IgnoreCase);

            if (trainedSkillMatch.Success)
            {
                rule.RequiredTrainedSkill = trainedSkillMatch.Groups.Cast<Group>()
                    .Skip(1)
                    .Select(g => g.Value?.Trim())
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            }

            var featMatch = Regex.Match(description, @"with the\s+(.+?)\s+feat", RegexOptions.IgnoreCase);
            if (featMatch.Success)
            {
                rule.RequiredFeatName = featMatch.Groups[1].Value.Trim();
            }

            var dexMatch = Regex.Match(description, @"dexterity\s+of\s+(\d+)", RegexOptions.IgnoreCase);
            if (dexMatch.Success && int.TryParse(dexMatch.Groups[1].Value, out var dexReq))
            {
                rule.MinimumDexterity = dexReq;
            }

            var strMatch = Regex.Match(description, @"strength\s+of\s+(\d+)", RegexOptions.IgnoreCase);
            if (strMatch.Success && int.TryParse(strMatch.Groups[1].Value, out var strReq))
            {
                rule.MinimumStrength = strReq;
            }

            var babMatch = Regex.Match(description, @"Base Attack Bonus of \+(\d+)", RegexOptions.IgnoreCase);
            if (babMatch.Success && int.TryParse(babMatch.Groups[1].Value, out var babReq))
            {
                rule.MinimumBaseAttackBonus = babReq;
            }

            rules.Add(rule);
        }

        return rules;
    }

    private static IEnumerable<string> GetTraitTitles(Species species)
    {
        return species.TraitLinks?
            .Select(link => link.SpeciesTrait?.Title?.Trim() ?? string.Empty)
            .Where(title => !string.IsNullOrWhiteSpace(title))
            ?? Enumerable.Empty<string>();
    }

    private static bool IsChoiceBonusFeat(Species species, string title)
    {
        if (species.Name.Equals("Tof", StringComparison.OrdinalIgnoreCase))
        {
            if (title.Equals("Bonus Feat (Rapid Shot)", StringComparison.OrdinalIgnoreCase) ||
                title.Equals("Bonus Feat (Rapid Strike)", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string ExtractParenValue(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var start = text.IndexOf('(');
        if (start < 0)
            return string.Empty;

        var depth = 0;
        for (var i = start; i < text.Length; i++)
        {
            if (text[i] == '(')
                depth++;
            else if (text[i] == ')')
            {
                depth--;
                if (depth == 0)
                    return text.Substring(start + 1, i - start - 1).Trim();
            }
        }

        return string.Empty;
    }

    private static string ExtractNestedSkillFocusSkillName(string grantedName)
    {
        const string prefix = "Skill Focus (";
        if (!grantedName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        var inner = grantedName.Substring(prefix.Length);
        if (inner.EndsWith(")"))
            inner = inner.Substring(0, inner.Length - 1);

        return inner.Trim();
    }

    private static string NormalizeGrantedFeatName(string grantedName)
    {
        if (grantedName.StartsWith("Skill Focus (", StringComparison.OrdinalIgnoreCase))
            return "Skill Focus";

        return grantedName.Trim();
    }
}

public sealed class ConditionalBonusFeatRuleDto
{
    public string FeatName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? SkillName { get; set; }
    public string? RequiredTrainedSkill { get; set; }
    public string? RequiredFeatName { get; set; }
    public int? MinimumDexterity { get; set; }
    public int? MinimumStrength { get; set; }
    public int? MinimumBaseAttackBonus { get; set; }
}

public sealed class SpeciesChoiceBonusFeatRuleDto
{
    public string ChoiceKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
}