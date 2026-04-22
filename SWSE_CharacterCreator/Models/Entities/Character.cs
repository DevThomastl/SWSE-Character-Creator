using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;

namespace SWSE_CharacterCreator.Models.Entities;

// main character model
public class Character
{
    public int Id { get; set; }

    // character name
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    // male/female
    public Sex Sex { get; set; } = Sex.Unspecified;

    // level 1–20
    [Range(1, 20)]
    public int Level { get; set; } = 1;

    public int? Age { get; set; }

    // optional physical stats
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }

    // species link
    public int? SpeciesId { get; set; }
    public Species? Species { get; set; }

    // notes for gear
    public string? EquipmentNotes { get; set; }

    // class info
    [MaxLength(50)]
    public string HeroicClassName { get; set; } = "Soldier";
    public int? HeroicClassId { get; set; }

    public int DarkSideScore { get; set; } = 0;
    public long Credits { get; set; } = 0;

    // ability scores + derived stats
    [Required]
    public AbilityScores AbilityScores { get; set; } = new();

    [Required]
    public DerivedStats DerivedStats { get; set; } = new();

    // csv in db
    [MaxLength(2000)]
    public string FeatIdsCsv { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string TalentIdsCsv { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string ForcePowerIdsCsv { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string TrainedSkillIdsCsv { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string SkillFeatSelectionsJson { get; set; } = "[]";

    [MaxLength(4000)]
    public string SpeciesChosenFeatsJson { get; set; } = "[]";

    // convert csv to list (not stored in the db)
    [NotMapped]
    public List<int> FeatIds
    {
        get => CsvToIntList(FeatIdsCsv);
        set => FeatIdsCsv = IntListToCsv(value);
    }

    [NotMapped]
    public List<int> TalentIds
    {
        get => CsvToIntList(TalentIdsCsv);
        set => TalentIdsCsv = IntListToCsv(value);
    }

    [NotMapped]
    public List<int> ForcePowerIds
    {
        get => CsvToIntList(ForcePowerIdsCsv);
        set => ForcePowerIdsCsv = IntListToCsv(value);
    }

    [NotMapped]
    public List<int> TrainedSkillIds
    {
        get => CsvToIntList(TrainedSkillIdsCsv);
        set => TrainedSkillIdsCsv = IntListToCsv(value);
    }

    [NotMapped]
    public List<SkillFeatSelection> SkillFeatSelections
    {
        get => JsonToSkillFeatSelectionList(SkillFeatSelectionsJson);
        set => SkillFeatSelectionsJson = SkillFeatSelectionListToJson(value);
    }

    [NotMapped]
    public List<string> SpeciesChosenFeats
    {
        get => JsonToStringList(SpeciesChosenFeatsJson);
        set => SpeciesChosenFeatsJson = StringListToJson(value);
    }

    // extra languages
    public List<string> BonusLanguages { get; set; } = new();

    // inventory list
    public List<InventoryEntry> Inventory { get; set; } = new();

    // csv to list helper
    private static List<int> CsvToIntList(string? csv)
    {
        if (string.IsNullOrWhiteSpace(csv)) return new List<int>();

        return csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                  .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
                  .Where(n => n.HasValue)
                  .Select(n => n!.Value)
                  .ToList();
    }

    // list to csv helper
    private static string IntListToCsv(IEnumerable<int>? list)
    {
        if (list == null) return string.Empty;
        return string.Join(",", list);
    }

    private static List<SkillFeatSelection> JsonToSkillFeatSelectionList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<SkillFeatSelection>();

        try
        {
            return JsonSerializer.Deserialize<List<SkillFeatSelection>>(json) ?? new List<SkillFeatSelection>();
        }
        catch
        {
            return new List<SkillFeatSelection>();
        }
    }

    private static string SkillFeatSelectionListToJson(IEnumerable<SkillFeatSelection>? list)
    {
        if (list == null) return "[]";
        return JsonSerializer.Serialize(list);
    }

    private static List<string> JsonToStringList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static string StringListToJson(IEnumerable<string>? list)
    {
        if (list == null) return "[]";
        return JsonSerializer.Serialize(list);
    }
}

// inventory item
public class InventoryEntry
{
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(1, 999)]
    public int Quantity { get; set; } = 1;
}

public class SkillFeatSelection
{
    public int FeatId { get; set; }
    public int SkillId { get; set; }
}