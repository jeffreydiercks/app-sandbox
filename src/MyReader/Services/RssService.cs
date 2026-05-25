using System.Xml.Linq;
using MyReader.Models;

namespace MyReader.Services;

public class RssService(HttpClient http)
{
    public async Task<List<FeedArticle>> ParseFeedAsync(string url)
    {
        try
        {
            using var stream = await http.GetStreamAsync(url);
            var doc = XDocument.Load(stream);
            XNamespace atom = "http://www.w3.org/2005/Atom";

            var articles = new List<FeedArticle>();

            // RSS 2.0
            foreach (var item in doc.Descendants("item"))
            {
                articles.Add(new FeedArticle
                {
                    Title = item.Element("title")?.Value ?? "(no title)",
                    Link = item.Element("link")?.Value ?? "",
                    Summary = StripHtml(item.Element("description")?.Value ?? ""),
                    Guid = item.Element("guid")?.Value
                           ?? item.Element("link")?.Value
                           ?? Guid.NewGuid().ToString(),
                    PublishedAt = TryParseDate(item.Element("pubDate")?.Value)
                });
            }

            // Atom feed
            if (articles.Count == 0)
            {
                foreach (var entry in doc.Descendants(atom + "entry"))
                {
                    var link = entry.Element(atom + "link")?.Attribute("href")?.Value ?? "";
                    articles.Add(new FeedArticle
                    {
                        Title = entry.Element(atom + "title")?.Value ?? "(no title)",
                        Link = link,
                        Summary = StripHtml(entry.Element(atom + "summary")?.Value
                                  ?? entry.Element(atom + "content")?.Value ?? ""),
                        Guid = entry.Element(atom + "id")?.Value ?? link,
                        PublishedAt = TryParseDate(
                            entry.Element(atom + "published")?.Value
                            ?? entry.Element(atom + "updated")?.Value)
                    });
                }
            }

            return articles;
        }
        catch
        {
            return [];
        }
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return "";
        var text = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", " ");
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\s{2,}", " ");
        return System.Net.WebUtility.HtmlDecode(text).Trim();
    }

    private static DateTime? TryParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return DateTime.TryParse(value, out var dt) ? dt.ToUniversalTime() : null;
    }
}
