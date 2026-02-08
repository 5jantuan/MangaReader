using MangaReader.Application.Interfaces;
using MangaReader.Domain.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace MangaReader.Application.UseCases;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Execute(string userName, string password)
    {
        var user = await _userRepository.GetByUserNameAsync(userName);
        if ( user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        var isValid = _passwordHasher.Verify(password, user.PasswordHash);
        if ( !isValid)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        var roles = user.Roles
            .Select(r=>r.ToString())
            .ToList();

        var token = _jwtTokenGenerator.GenerateToken(user.Id, roles);
        return token; 
    }
}