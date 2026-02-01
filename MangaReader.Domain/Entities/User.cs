using System;
using System.Security.Cryptography.X509Certificates;

namespace MangaReader.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly HashSet<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles;

    protected User() { } // EF

    public User(string userName, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("Username cannot be empty", nameof(userName));

        Id = Guid.NewGuid();
        UserName = userName;
        PasswordHash = passwordHash;
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

    public void ChangePassword(string newPasswordHash) 
    { 
        PasswordHash = newPasswordHash; 
    }
}

public enum UserRole
{
    Reader = 1,
    Author = 2,
    Admin = 3
}
