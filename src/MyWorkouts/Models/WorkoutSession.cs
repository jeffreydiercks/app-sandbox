using System.ComponentModel.DataAnnotations;

namespace MyWorkouts.Models;

public class WorkoutSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string RoutineId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string RoutineName { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public List<WorkoutLogEntry> Entries { get; set; } = [];
}
