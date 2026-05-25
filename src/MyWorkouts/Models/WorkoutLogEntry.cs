namespace MyWorkouts.Models;

public class WorkoutLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ExerciseId { get; set; } = string.Empty;
    public string ExerciseName { get; set; } = string.Empty;
    public int SetsCompleted { get; set; }
    public int? ActualReps { get; set; }
    public decimal? ActualWeight { get; set; }
    public string? ActualWeightUnit { get; set; }
}
