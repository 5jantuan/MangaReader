using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Entities;


namespace MangaReader.Application.UseCases;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Execute(string userName, string password)
    {
        var passwordHash = _passwordHasher.Hash(password);

        var user = new User(userName, passwordHash);

        await _userRepository.AddAsync(user);

        return user.Id;
    }
}
