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
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<MangaCover> MangaCovers { get; set; } = null!;

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

                entity.HasMany(p => p.Phrases)
                      .WithOne(ph => ph.Page)
                      .HasForeignKey(ph => ph.PageId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =================== Phrase ===================
            modelBuilder.Entity<Phrase>(entity =>
            {
                entity.HasKey(ph => ph.Id);

                entity.Property(ph => ph.Text).IsRequired();

                entity.Property(ph => ph.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.HasMany(ph => ph.PhraseTranslations)
                      .WithOne(pt => pt.Phrase)
                      .HasForeignKey(pt => pt.PhraseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // =================== PhraseTranslation ===================
            modelBuilder.Entity<PhraseTranslation>(entity =>
            {
                entity.HasKey(pt => pt.Id);

                entity.Property(pt => pt.Text).IsRequired();

                entity.Property(pt => pt.CreatedAt)
                      .HasDefaultValueSql("NOW()");

                entity.HasOne(pt => pt.Language)
                      .WithMany(l => l.PhraseTranslations)
                      .HasForeignKey(pt => pt.LanguageId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // =================== Language ===================
            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(l => l.Id);

                entity.Property(l => l.Code)
                      .IsRequired()
                      .HasMaxLength(10);

                entity.Property(l => l.Name)
                      .IsRequired()
                      .HasMaxLength(100);
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
        }
    }
}