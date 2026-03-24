namespace MangaReader.Web.ViewModels.Author;

public class AuthorProfileViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }
    public string? About { get; set; }
    public string? TelegramUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TikTokUrl { get; set; }
    public int ProjectsCount { get; set; }
}