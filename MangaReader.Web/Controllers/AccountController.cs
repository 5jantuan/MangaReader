using System.Security.Claims;
using MangaReader.Application.UseCases;
using MangaReader.Domain.Interfaces;
using MangaReader.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MangaReader.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly RegisterUserUseCase _registerUserUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly IUserRepository _userRepository;

        public AccountController(
            RegisterUserUseCase registerUserUseCase,
            LoginUseCase loginUseCase,
            IUserRepository userRepository)
        {
            _registerUserUseCase = registerUserUseCase;
            _loginUseCase = loginUseCase;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var languageId = model.PreferredLanguageId ?? Guid.Empty;

                await _registerUserUseCase.Execute(
                    model.UserName,
                    model.Password,
                    languageId);

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _loginUseCase.Execute(model.UserName, model.Password);

                var user = await _userRepository.GetByUserNameAsync(model.UserName);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                return RedirectToAction("Dashboard", "Author");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}