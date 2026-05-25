using System.ComponentModel.DataAnnotations;

namespace MyGuitar.Models;

public class Lesson
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Stage { get; set; }

    public LessonStatus Status { get; set; } = LessonStatus.NotStarted;

    public string? Notes { get; set; }

    public DateTime? LastPracticedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
