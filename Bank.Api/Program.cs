using Bank.Api.Data;
using Bank.Api.Models;
using Bank.Api.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(LocalDevelopmentSettings.Url);

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Saisir uniquement le jeton JWT retourné par POST /api/auth/login."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }] = Array.Empty<string>()
    });
});

// SQL Server + Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(LocalDevelopmentSettings.ConnectionString));

// ASP.NET Core Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = LocalDevelopmentSettings.JwtIssuer,
        ValidAudience = LocalDevelopmentSettings.JwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(LocalDevelopmentSettings.JwtSecret))
    };
});

// Services métier
builder.Services.AddScoped<IGuichetService, GuichetService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ITypeCompteService, TypeCompteService>();
builder.Services.AddScoped<ICompteService, CompteService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Préparation CORS pour le futur frontend Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    await IdentitySeeder.InitializeAsync(scope.ServiceProvider);

// Swagger disponible pendant la formation
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AngularClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
