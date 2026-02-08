namespace MangaReader.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, IReadOnlyCollection<string> roles);
}