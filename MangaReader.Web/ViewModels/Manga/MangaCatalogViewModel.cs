namespace MangaReader.Web.ViewModels.Manga;

public class MangaCatalogViewModel
{
    public Guid? SelectedCategoryId { get; set; }
    public List<CategoryOptionViewModel> Categories { get; set; } = new();
    public List<MangaCatalogItemViewModel> Mangas { get; set; } = new();
}