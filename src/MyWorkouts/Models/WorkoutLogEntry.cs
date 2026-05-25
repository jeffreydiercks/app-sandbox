using System.ComponentModel.DataAnnotations;

namespace MyWorkouts.Models;

public class WorkoutLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(36)]
    public string ExerciseId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string ExerciseName { get; set; } = string.Empty;

    [Range(0, 100)]
    public int SetsCompleted { get; set; }

    [Range(0, 500)]
    public int? ActualReps { get; set; }

    [Range(0, 2000)]
    public decimal? ActualWeight { get; set; }

    [MaxLength(10)]
    public string? ActualWeightUnit { get; set; }
}
