using MangaReader.Domain.Entities;
using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Web.Services;

public class DemoTranslationSeeder
{
    private readonly AppDbContext _context;

    public DemoTranslationSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedFrierenDemoAsync(Guid chapterId, Guid russianLanguageId)
    {
        var chapter = await _context.Chapters
            .Include(c => c.Pages)
                .ThenInclude(p => p.Phrases)
                    .ThenInclude(ph => ph.PhraseTranslations)
            .FirstOrDefaultAsync(c => c.Id == chapterId);

        if (chapter == null)
            throw new InvalidOperationException("Chapter not found.");

        var russian = await _context.Languages
            .FirstOrDefaultAsync(l => l.Id == russianLanguageId);

        if (russian == null)
            throw new InvalidOperationException("Russian language not found.");

        var phrasesByPage = new Dictionary<int, List<(string Original, string Translation)>>
        {
            [1] = new()
            {
                ("THE STORY BEGINS AFTER THE END OF ADVENTURE.", "История начинается после конца приключения."),
                ("BRAVE HIMMEL", "Храбрый Химмель"),
                ("MAGE FRIEREN", "Маг Фрирен"),
                ("PRIEST HEITER", "Жрец Хайтер"),
                ("WARRIOR EISEN", "Воин Айзен"),
                ("AND BRING PEACE TO THE WORLD.", "И принести миру покой."),
                ("YOU DEFEATED DEMON KING.", "Вы победили короля демонов.")
            },
            [2] = new()
            {
                ("THE KING WILL BUILD THE STATUE OF US AT THE CITY CENTRAL SQUARE.", "Король установит нашу статую на центральной площади города."),
                ("DON'T TAKE IT SERIOUSLY, FRIEREN.", "Не принимай это всерьёз, Фрирен."),
                ("AT LEAST WE GOT A FREE BEER.", "Зато мы получили бесплатное пиво."),
                ("MONEY IS SUCH A PROBLEM. THIS THING COSTS LIKE 10 COPPER COINS.", "Деньги — это проблема. Эта штука стоит около 10 медных монет."),
                ("MINE IS GOING TO BE AS HANDSOME AS I AM.", "Моя статуя будет такой же красивой, как и я."),
                ("YOU DRUNKARD PRIEST.", "Ты пьяный жрец."),
                ("HAHAHA", "ХАХАХА")
            },
            [3] = new()
            {
                ("I'M REALLY GLAD I'VE TRAVELED WITH YOU.", "Я очень рад, что путешествовал с вами."),
                ("BUT IT WAS SO MUCH FUN.", "Но это было так весело."),
                ("YEAH. EVEN THOUGH THERE WERE BAD TIMES.", "Да. Хотя были и трудные времена."),
                ("I'VE JUST THOUGHT THE SAME THING.", "Я только что подумал о том же.")
            },
            [4] = new()
            {
                ("SHORT?", "Короткий?"),
                ("WHAT?", "Что?"),
                ("10 YEARS?", "10 лет?"),
                ("IT WAS A REALLY GOOD SHORT PERIOD OF TIME.", "Это был действительно хороший короткий период времени."),
                ("IT'S ABOUT TIMES.", "Самое время."),
                ("OH REALLY?", "Правда?"),
                ("LOOK THIS GUY HAS BECOME AN OLD MAN.", "Смотри, этот парень стал стариком."),
                ("WHAT A PITY", "Какая жалость."),
                ("PARDON?", "Прости?"),
                ("ISN'T HE ALREADY OLD SINCE THE BEGINNING?", "Разве он не был старым с самого начала?")
            }
        };

        foreach (var page in chapter.Pages.OrderBy(p => p.Number))
        {
            if (!phrasesByPage.TryGetValue(page.Number, out var demoPhrases))
                continue;

            var index = 0;

            foreach (var (original, translation) in demoPhrases)
            {
                var existingPhrase = page.Phrases.FirstOrDefault(p => p.Text == original);

                if (existingPhrase == null)
                {
                    // Фейковые координаты для демо
                    decimal x = 40;
                    decimal y = 40 + index * 90;
                    decimal width = 220;
                    decimal height = 70;

                    existingPhrase = new Phrase(
                        page.Id,
                        original,
                        x,
                        y,
                        width,
                        height);

                    _context.Phrases.Add(existingPhrase);
                    await _context.SaveChangesAsync();
                }

                var phraseWithTranslations = await _context.Phrases
                    .Include(p => p.PhraseTranslations)
                    .FirstOrDefaultAsync(p => p.Id == existingPhrase.Id);

                if (phraseWithTranslations == null)
                    continue;

                var translationExists = phraseWithTranslations.PhraseTranslations
                    .Any(t => t.LanguageId == russianLanguageId);

                if (!translationExists)
                {
                    var phraseTranslation = new PhraseTranslation(
                        phraseWithTranslations,
                        russian,
                        translation);

                    _context.PhraseTranslations.Add(phraseTranslation);
                }

                index++;
            }
        }
        await _context.SaveChangesAsync();
    }
}