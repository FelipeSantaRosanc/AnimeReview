using System.Text;
using AnimeReview.Data;
using AnimeReview.Entities;
using AnimeReview.Repositories;
using AnimeReview.Repositories.Interfaces;
using AnimeReview.Services;
using AnimeReview.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

/*
Estrutura do programa

Builder

Services{
    Controllers
    Swagger
    Database
    Identity
    Authentication (JWT)
    Repositories
    Services
    CORS
}

Pipeline{
    Swagger
    HTTPS
    CORS
    Authentication
    Authorization
    MapControllers
}

Run
*/

var builder = WebApplication.CreateBuilder(args);

// =============================
// Controllers
// =============================
builder.Services.AddControllers();


// =============================
// Swagger
// =============================
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AnimeReview API",
        Version = "v1",
        Description = "API para gerenciamento de animes, mangás e reviews"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// =============================
// Database - MySQL
// =============================
builder.Services.AddDbContext<AnimeReviewDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    ));


// =============================
// Identity
// =============================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AnimeReviewDbContext>()
    .AddDefaultTokenProviders();


// =============================
// Authentication - JWT
// =============================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_123456";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});


// =============================
// Repositories
// =============================
builder.Services.AddScoped<IAnimeRepository, AnimeRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();


// =============================
// Services
// =============================
builder.Services.AddScoped<IAnimeService, AnimeService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IAuthService, AuthService>();


// =============================
// CORS
// =============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});


// =============================
// Build
// =============================
var app = builder.Build();


// =============================
// Pipeline
// =============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();