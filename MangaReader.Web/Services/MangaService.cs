using MangaReader.Web.Models.Manga;
using MangaReader.Domain.Entities;
using MangaReader.Web.ViewModels.Manga;
using MangaReader.Web.ViewModels.Chapter;

public class MangaService
{
    private static List<MangaDto> _mangas = new();

    public List<MangaDto> GetMyMangas()
    {
        return _mangas;
    }
    
    public async Task CreateManga(CreateMangaViewModel model, string coverUrl)
    {
        var manga = new MangaDto
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Chapters = new List<ChapterDto>()
        };

        _mangas.Add(manga);
    }

    public async Task CreateChapter(CreateChapterViewModel model, List<string> pages)
    {
        var manga = _mangas.FirstOrDefault(x => x.Id == model.MangaId);

        if (manga == null)
            return;

        var chapter = new ChapterDto
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Number = model.Number
        };

        manga.Chapters.Add(chapter);
    }

}