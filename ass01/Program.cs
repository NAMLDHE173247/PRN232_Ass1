using ass01.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Register Services
builder.Services.AddScoped<ass01.BusinessLogic.Services.IAuthService, ass01.BusinessLogic.Services.AuthService>();
builder.Services.AddScoped<ass01.BusinessLogic.Services.IAccountService, ass01.BusinessLogic.Services.AccountService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
