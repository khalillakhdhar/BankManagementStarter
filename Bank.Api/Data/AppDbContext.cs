using Bank.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Guichet> Guichets => Set<Guichet>();

    public DbSet<Client> Clients => Set<Client>();

    public DbSet<TypeCompte> TypesComptes => Set<TypeCompte>();

    public DbSet<Compte> Comptes => Set<Compte>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ConfigureGuichet(builder);
        ConfigureClient(builder);
        ConfigureTypeCompte(builder);
        ConfigureCompte(builder);
        ConfigureApplicationUser(builder);
        ConfigureTransaction(builder);
    }

    private static void ConfigureGuichet(ModelBuilder builder)
    {
        builder.Entity<Guichet>()
            .HasIndex(g => g.Code)
            .IsUnique();

        builder.Entity<Guichet>()
            .Property(g => g.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Entity<Guichet>()
            .Property(g => g.Nom)
            .IsRequired()
            .HasMaxLength(120);
    }

    private static void ConfigureClient(ModelBuilder builder)
    {
        builder.Entity<Client>()
            .HasIndex(c => c.Cin)
            .IsUnique();

        builder.Entity<Client>()
            .Property(c => c.Cin)
            .IsRequired()
            .HasMaxLength(20);

        builder.Entity<Client>()
            .Property(c => c.Nom)
            .IsRequired()
            .HasMaxLength(80);

        builder.Entity<Client>()
            .Property(c => c.Prenom)
            .IsRequired()
            .HasMaxLength(80);
    }

    private static void ConfigureTypeCompte(ModelBuilder builder)
    {
        builder.Entity<TypeCompte>()
            .HasIndex(t => t.Code)
            .IsUnique();

        builder.Entity<TypeCompte>()
            .Property(t => t.SoldeMinimum)
            .HasPrecision(18, 2);

        builder.Entity<TypeCompte>()
            .Property(t => t.FraisMensuels)
            .HasPrecision(18, 2);
    }

    private static void ConfigureCompte(ModelBuilder builder)
    {
        builder.Entity<Compte>()
            .HasIndex(c => c.NumeroCompte)
            .IsUnique();

        builder.Entity<Compte>()
            .Property(c => c.Solde)
            .HasPrecision(18, 2);

        builder.Entity<Compte>()
            .HasOne(c => c.Client)
            .WithMany(c => c.Comptes)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Compte>()
            .HasOne(c => c.TypeCompte)
            .WithMany(t => t.Comptes)
            .HasForeignKey(c => c.TypeCompteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Compte>()
            .HasOne(c => c.Guichet)
            .WithMany(g => g.Comptes)
            .HasForeignKey(c => c.GuichetId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureApplicationUser(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Guichet)
            .WithMany(g => g.Agents)
            .HasForeignKey(u => u.GuichetId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureTransaction(ModelBuilder builder)
    {
        builder.Entity<Transaction>()
            .HasIndex(t => t.Reference)
            .IsUnique();

        builder.Entity<Transaction>()
            .Property(t => t.Montant)
            .HasPrecision(18, 2);

        builder.Entity<Transaction>()
            .HasOne(t => t.CompteSource)
            .WithMany()
            .HasForeignKey(t => t.CompteSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Transaction>()
            .HasOne(t => t.CompteDestination)
            .WithMany()
            .HasForeignKey(t => t.CompteDestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Transaction>()
            .HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey(t => t.AgentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}