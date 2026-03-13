using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Entities;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MangaReader.Application.UseCases;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILanguageRepository _languageRepository;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILanguageRepository languageRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _languageRepository = languageRepository;
    }

    public async Task<string> Execute(string userName, string password, Guid preferredLanguageId)
    {
        var existingUser = await _userRepository.GetByUserNameAsync(userName);
        if (existingUser != null)
            throw new InvalidOperationException("Username already taken");

        // если язык не указан, берём дефолтный
        Guid languageId;
        if (preferredLanguageId != Guid.Empty)
        {
            languageId = preferredLanguageId;
        }
        else
        {
            var defaultLanguage = await _languageRepository.GetDefaultLanguageAsync();
            languageId = defaultLanguage.Id;
        }

        var passwordHash = _passwordHasher.Hash(password);
        var user = new User(userName, passwordHash, preferredLanguageId);
        await _userRepository.AddAsync(user);

        var roles = user.Roles.Select(r => r.ToString()).ToList();
        var token = _jwtTokenGenerator.GenerateToken(user.Id, roles);
        return token;
    }
}
