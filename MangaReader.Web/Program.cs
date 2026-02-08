using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Repositories;
using MangaReader.Infrastructure.Security;
using MangaReader.Application.Interfaces;
using MangaReader.Application.Services;
using MangaReader.Application.UseCases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Подключаем PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT Secret is not configured");
        var key = Encoding.UTF8.GetBytes(secret);

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

builder.Services.AddScoped<IPhraseRepository, PhraseRepository>();
builder.Services.AddScoped<IPhraseService, PhraseService>();
builder.Services.AddScoped<IGetPhrasesForPageUseCase, GetPhrasesForPageUseCase>();
builder.Services.AddScoped<
    MangaReader.Application.Interfaces.IAddPhraseTranslationUseCase,
    MangaReader.Application.UseCases.AddPhraseTranslationUseCase>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<IUserRepository, UserRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
