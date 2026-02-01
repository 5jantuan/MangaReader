using MangaReader.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MangaReader.Domain.Interfaces;
using MangaReader.Infrastructure.Repositories;
using MangaReader.Application.Interfaces;
using MangaReader.Application.Services;
using MangaReader.Application.UseCases;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Подключаем PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IPhraseRepository, PhraseRepository>();
builder.Services.AddScoped<IPhraseService, PhraseService>();
builder.Services.AddScoped<IGetPhrasesForPageUseCase, GetPhrasesForPageUseCase>();
builder.Services.AddScoped<
    MangaReader.Application.Interfaces.IAddPhraseTranslationUseCase,
    MangaReader.Application.UseCases.AddPhraseTranslationUseCase>();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
