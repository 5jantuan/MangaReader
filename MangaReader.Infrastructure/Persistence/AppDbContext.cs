using System.Reflection.Metadata;
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

        public DbSet<Manga> Mangas { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<PhraseTranslation> PhraseTranslations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Manga
            modelBuilder.Entity<Manga>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("NOW()");

                entity.HasMany(e => e.Chapters)
                    .WithOne(c => c.Manga)
                    .HasForeignKey(c => c.MangaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            // Chapter
            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.ImagePath).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(p => p.Chapter)
                    .WithMany(c => c.Pages)
                    .HasForeignKey(p => p.ChapterId)
                    .OnDelete(DeleteBehavior.Cascade);    
            });

            modelBuilder.Entity<Phrase>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.X).IsRequired();
                entity.Property(e => e.Y).IsRequired();
                entity.Property(e => e.Width).IsRequired();
                entity.Property(e => e.Height).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(p => p.Page)
                    .WithMany(p => p.Phrases)
                    .HasForeignKey(p => p.PageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<PhraseTranslation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Text).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

                entity.HasOne(pt => pt.Phrase)
                    .WithMany(p => p.PhraseTranslations)
                    .HasForeignKey(pt => pt.PhraseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.Language)
                    .WithMany(l => l.PhraseTranslations)
                    .HasForeignKey(pt => pt.LanguageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
