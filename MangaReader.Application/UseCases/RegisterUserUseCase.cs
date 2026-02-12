using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using MangaReader.Domain.Entities;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace MangaReader.Application.UseCases;

public class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Execute(string userName, string password)
    {
        var existingUser = await _userRepository.GetByUserNameAsync(userName);
        if (existingUser != null)
            throw new InvalidOperationException("Username already taken");

        var passwordHash = _passwordHasher.Hash(password);
        var user = new User(userName, passwordHash);
        await _userRepository.AddAsync(user);

        var roles = user.Roles.Select(r => r.ToString()).ToList();
        var token = _jwtTokenGenerator.GenerateToken(user.Id, roles);
        return token;
    }
}
