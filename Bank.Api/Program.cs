using Bank.Api.Data;
using Bank.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQL Server + Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

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

// Swagger disponible pendant la formation
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AngularClient");

// JWT sera configuré dans la séance sécurité.
// Ces middlewares sont déjà positionnés au bon endroit.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
