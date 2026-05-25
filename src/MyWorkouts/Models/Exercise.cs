using System.ComponentModel.DataAnnotations;

namespace MyWorkouts.Models;

public class Exercise
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(5, 3600)]
    public int DurationSeconds { get; set; } = 30;

    [Range(0, 600)]
    public int RestSeconds { get; set; } = 10;

    public int Order { get; set; }
    public string? Notes { get; set; }
}
