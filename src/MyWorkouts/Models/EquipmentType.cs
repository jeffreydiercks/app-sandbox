using System.ComponentModel.DataAnnotations;

namespace MyWorkouts.Models;

public enum EquipmentType
{
    Bodyweight,
    Dumbbell,
    Barbell,
    Kettlebell,
    [Display(Name = "Resistance Band")]
    ResistanceBand,
    [Display(Name = "Pull-Up Bar")]
    PullUpBar,
    [Display(Name = "Chair / Bench")]
    ChairBench,
    Wall,
    Machine,
    Other
}

public static class EquipmentTypeExtensions
{
    public static string ToDisplayName(this EquipmentType equipment) => equipment switch
    {
        EquipmentType.ResistanceBand => "Resistance Band",
        EquipmentType.PullUpBar => "Pull-Up Bar",
        EquipmentType.ChairBench => "Chair / Bench",
        _ => equipment.ToString()
    };
}
