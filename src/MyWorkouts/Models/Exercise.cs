using System.ComponentModel.DataAnnotations;

namespace MyWorkouts.Models;

public class Exercise
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsRepBased { get; set; } = false;

    [Range(5, 3600)]
    public int DurationSeconds { get; set; } = 30;

    [Range(1, 500)]
    public int Reps { get; set; } = 10;

    [Range(1, 100)]
    public int Sets { get; set; } = 1;

    [Range(0, 300)]
    public int IntraSetRestSeconds { get; set; } = 30;

    [Range(0, 600)]
    public int RestSeconds { get; set; } = 10;

    public decimal? PrescribedWeight { get; set; }

    public string WeightUnit { get; set; } = "lbs";

    public EquipmentType Equipment { get; set; } = EquipmentType.Bodyweight;

    public bool IsOneSided { get; set; } = false;

    public int Order { get; set; }
    public string? Notes { get; set; }
}
