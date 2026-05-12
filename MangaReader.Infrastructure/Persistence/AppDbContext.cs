using MangaReader.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaReader.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Manga> Mangas { get; set; } = null!;
        public DbSet<Chapter> Chapters { get; set; } = null!;
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<Phrase> Phrases { get; set; } = null!;
        public DbSet<PhraseTranslation> PhraseTranslations { get; set; } = null!;
        public DbSet<Bubble> Bubbles { get; set; } = null!;
        public DbSet<BubbleTranslation> BubbleTranslations { get; set; } = null!;
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<MangaCover> MangaCovers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // =================== Manga ===================
            modelBuilder.Entity<Manga>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Description)
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                // СВЯЗЬ С АВТОРОМ (ВАЖНО)
                entity.HasOne(m => m.Author)
                      .WithMany()
                      .HasForeignKey(m => m.AuthorId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.OriginalLanguageId)
                      .IsRequired();

                entity.HasOne(e => e.OriginalLanguage)
                      .WithMany()
                      .HasForeignKey(e => e.OriginalLanguageId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Chapters
                entity.HasMany(m => m.Chapters)
                      .WithOne(c => c.Manga)
                      .HasForeignKey(c => c.MangaId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Covers
                entity.HasMany(m => m.Covers)
                      .WithOne()
                      .HasForeignKey(c => c.MangaId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(m => m.Categories)
                        .WithMany(c => c.Mangas)
                        .UsingEntity(j => j.ToTable("MangaCategories"));
            });

            // =================== Chapter ===================
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Title).IsRequired();
                entity.Property(c => c.Number).IsRequired();
                entity.Property(c => c.Views).IsRequired();
                
                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.HasMany(c => c.Pages)
                      .WithOne(p => p.Chapter)
                      .HasForeignKey(p => p.ChapterId)
                      .OnDelete(DeleteBehavior.Cascade);

                
            });

            // =================== Page ===================
            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Number).IsRequired();
                entity.Property(p => p.ImagePath).IsRequired();

                entity.Property(p => p.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.Property(p => p.ProcessingStatus)
                      .IsRequired();

                entity.Property(p => p.OcrProcessedAt);

                entity.Property(p => p.TranslationProcessedAt);

                entity.Property(p => p.ProcessingError);

                entity.HasMany(p => p.Phrases)
                      .WithOne(ph => ph.Page)
                      .HasForeignKey(ph => ph.PageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

           

            // =================== User ===================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.UserName).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PreferredLanguageId).IsRequired();
                entity.Property(u => u.AvatarPath);
                entity.Property(u => u.About);
                entity.Property(u => u.TelegramUrl);
                entity.Property(u => u.InstagramUrl);
                entity.Property(u => u.TikTokUrl);

                entity.Property(u => u.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.HasOne<Language>()
                      .WithMany()
                      .HasForeignKey(u => u.PreferredLanguageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =================== MangaCover ===================
            modelBuilder.Entity<MangaCover>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Path).IsRequired();
                entity.Property(c => c.IsPinned).IsRequired();

                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Category>(entity =>
            {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            });
            // =================== Bubble ===================
            modelBuilder.Entity<Bubble>(entity =>
            {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.OriginalText)
                  .IsRequired();

            entity.Property(e => e.X).IsRequired();
            entity.Property(e => e.Y).IsRequired();
            entity.Property(e => e.Width).IsRequired();
            entity.Property(e => e.Height).IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Page)
                  .WithMany(p => p.Bubbles)
                  .HasForeignKey(e => e.PageId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Phrases)
                  .WithOne(p => p.Bubble)
                  .HasForeignKey(p => p.BubbleId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Translations)
                  .WithOne(t => t.Bubble)
                  .HasForeignKey(t => t.BubbleId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
            // =================== BubbleTranslation ===================    
            modelBuilder.Entity<BubbleTranslation>(entity =>
            {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Text)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW()");

            entity.HasOne(e => e.Language)
                  .WithMany(l => l.BubbleTranslations)
                  .HasForeignKey(e => e.LanguageId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.BubbleId, e.LanguageId })
                  .IsUnique();
            });  
        }
        
    }
}