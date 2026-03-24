namespace MangaReader.Web.ViewModels.Manga;

public class CreateMangaViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public List<Guid> SelectedCategoryIds { get; set; } = new();
    public List<CategoryOptionViewModel> AvailableCategories { get; set; } = new();
}

public class CategoryOptionViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}