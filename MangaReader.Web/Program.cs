using MangaReader.Application.Interfaces;
using MangaReader.Application.Services;
using MangaReader.Application.UseCases;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Persistence;
using MangaReader.Infrastructure.Repositories;
using MangaReader.Infrastructure.Security;
using MangaReader.Infrastructure.Ocr;
using MangaReader.Infrastructure.Translation;
using MangaReader.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie authentication for MVC pages
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "MangaReader.Auth";
    });

// Repositories
builder.Services.AddScoped<IPhraseRepository, PhraseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();


builder.Services.AddHttpClient<IOcrService, HttpOcrService>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:8001");
});

// Infrastructure services
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<MangaService>();
builder.Services.AddScoped<DemoTranslationSeeder>();
builder.Services.AddScoped<ITranslationService, FakeTranslationService>();
builder.Services.AddScoped<IChapterProcessingService, ChapterProcessingService>();

// Application services
builder.Services.AddScoped<IPhraseService, PhraseService>();

// Use cases
builder.Services.AddScoped<IGetPhrasesForPageUseCase, GetPhrasesForPageUseCase>();
builder.Services.AddScoped<IAddPhraseTranslationUseCase, AddPhraseTranslationUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();