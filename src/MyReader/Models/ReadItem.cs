namespace MyReader.Models;

public class ReadItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string FeedId { get; set; } = string.Empty;
    public string ItemGuid { get; set; } = string.Empty;
    public DateTime ReadAt { get; set; }
}
