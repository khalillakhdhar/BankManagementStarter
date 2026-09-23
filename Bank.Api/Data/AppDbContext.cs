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

    // TODO EXERCICE :
    // Décommenter ces DbSet après avoir créé les propriétés Id
    // dans les Models.
    //
    // public DbSet<Guichet> Guichets => Set<Guichet>();
    // public DbSet<Client> Clients => Set<Client>();
    // public DbSet<TypeCompte> TypesComptes => Set<TypeCompte>();
    // public DbSet<Compte> Comptes => Set<Compte>();
    // public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // TODO EXERCICE :
        // Ajouter ici :
        // - index uniques
        // - précision decimal(18,2)
        // - relations
        // - DeleteBehavior.Restrict
    }
}
