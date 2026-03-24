using System.Text;
using MangaReader.Application.Interfaces;
using MangaReader.Application.Services;
using MangaReader.Application.UseCases;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Persistence;
using MangaReader.Infrastructure.Repositories;
using MangaReader.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MangaReader.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ==================== MVC ====================
builder.Services.AddControllersWithViews();

// ==================== Database ====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ==================== Authentication / JWT ====================
var secret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT Secret is not configured");

var key = Encoding.UTF8.GetBytes(secret);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ClockSkew = TimeSpan.Zero
        };
    });

// ==================== Repositories ====================
builder.Services.AddScoped<IPhraseRepository, PhraseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();

// ==================== Infrastructure services ====================
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<MangaService>();

// ==================== Application services ====================
builder.Services.AddScoped<IPhraseService, PhraseService>();

// ==================== Use cases ====================
builder.Services.AddScoped<IGetPhrasesForPageUseCase, GetPhrasesForPageUseCase>();
builder.Services.AddScoped<IAddPhraseTranslationUseCase, AddPhraseTranslationUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RegisterUserUseCase>();

var app = builder.Build();

// ==================== Middleware ====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ==================== Static files ====================
app.MapStaticAssets();

// ==================== Routes ====================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();