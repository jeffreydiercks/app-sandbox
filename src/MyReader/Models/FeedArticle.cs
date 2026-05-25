namespace MyReader.Models;

public class FeedArticle
{
    public string Guid { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public bool IsRead { get; set; }
}
