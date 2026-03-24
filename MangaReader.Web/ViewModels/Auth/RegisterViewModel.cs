namespace MangaReader.Web.ViewModels.Auth;

public class RegisterViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid? PreferredLanguageId { get; set; }
}