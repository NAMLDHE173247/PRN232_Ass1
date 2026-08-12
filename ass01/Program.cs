using ass01.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<FunewsManagementContext>(
options => options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn"))
);

// Register Singleton Provider
builder.Services.AddSingleton<ass01.BusinessLogic.Services.AdminAccountConfigurationProvider>();

// Register DAOs
builder.Services.AddScoped<ass01.DataAccess.DAOs.IAccountDAO, ass01.DataAccess.DAOs.AccountDAO>();
builder.Services.AddScoped<ass01.DataAccess.DAOs.ICategoryDAO, ass01.DataAccess.DAOs.CategoryDAO>();
builder.Services.AddScoped<ass01.DataAccess.DAOs.INewsArticleDAO, ass01.DataAccess.DAOs.NewsArticleDAO>();
builder.Services.AddScoped<ass01.DataAccess.DAOs.ITagDAO, ass01.DataAccess.DAOs.TagDAO>();

// Register Repositories
builder.Services.AddScoped<ass01.DataAccess.Repositories.IAccountRepository, ass01.DataAccess.Repositories.AccountRepository>();
builder.Services.AddScoped<ass01.DataAccess.Repositories.ICategoryRepository, ass01.DataAccess.Repositories.CategoryRepository>();
builder.Services.AddScoped<ass01.DataAccess.Repositories.INewsArticleRepository, ass01.DataAccess.Repositories.NewsArticleRepository>();
builder.Services.AddScoped<ass01.DataAccess.Repositories.ITagRepository, ass01.DataAccess.Repositories.TagRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
