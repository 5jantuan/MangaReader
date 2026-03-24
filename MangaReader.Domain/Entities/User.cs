using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using MangaReader.Domain.Enums;

namespace MangaReader.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Guid PreferredLanguageId { get; private set; }
    public string? AvatarPath { get; private set; }
    public string? About { get; private set; }
    public string? TelegramUrl { get; private set; }
    public string? InstagramUrl { get; private set; }
    public string? TikTokUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly HashSet<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles;

    protected User() { } // EF

    public User(string userName, string passwordHash, Guid preferredLanguageId)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty", nameof(userName));

        if (preferredLanguageId == Guid.Empty)
            throw new ArgumentException("Language cannot be empty", nameof(preferredLanguageId));


        Id = Guid.NewGuid();
        UserName = userName;
        PasswordHash = passwordHash;
        PreferredLanguageId = preferredLanguageId;
        CreatedAt = DateTime.UtcNow;

        // каждый пользователь — читатель по умолчанию
        _roles.Add(UserRole.Reader);
    }

    public bool HasRole(UserRole role) => _roles.Contains(role);

    public bool IsAuthor() => HasRole(UserRole.Author);
    public bool IsReader() => HasRole(UserRole.Reader);

    public void BecomeAuthor()
    {
        if (HasRole(UserRole.Author))
            return;

        _roles.Add(UserRole.Author);
    }

    public void ChangeUserName(string newUserName)
    {
        if (string.IsNullOrWhiteSpace(newUserName))
            throw new ArgumentException("Username cannot be empty", nameof(newUserName));

        UserName = newUserName;
    }

    public void ChangePassword(string newPasswordHash) 
    { 
        PasswordHash = newPasswordHash; 
    }

    public void ChangePreferredLangugeId (Guid languageId)
    {
        if ( languageId == Guid.Empty)
            throw new ArgumentException("Language can not be empty", nameof(languageId));

        PreferredLanguageId = languageId;
    }

    public void UpdateProfile(
        string? about,
        string? telegramUrl,
        string? instagramUrl,
        string? tikTokUrl,
        string? avatarPath)
    {
        About = about;
        TelegramUrl = telegramUrl;
        InstagramUrl = instagramUrl;
        TikTokUrl = tikTokUrl;

        if (!string.IsNullOrWhiteSpace(avatarPath))
            AvatarPath = avatarPath;
    }
}
