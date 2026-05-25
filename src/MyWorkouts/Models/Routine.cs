using System.ComponentModel.DataAnnotations;

namespace MyWorkouts.Models;

public class Routine
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(1, 480)]
    public int EstimatedMinutes { get; set; } = 7;

    public List<Exercise> Exercises { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
