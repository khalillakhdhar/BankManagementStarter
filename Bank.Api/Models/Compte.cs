using System.ComponentModel.DataAnnotations;

namespace Bank.Api.Models;

public class Compte
{
    public int Id { get; set; }

    [Required]
    [MaxLength(34)]
    public string NumeroCompte { get; set; } = string.Empty;

    public decimal Solde { get; set; }

    public DateTime DateOuverture { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    public int TypeCompteId { get; set; }

    public TypeCompte TypeCompte { get; set; } = null!;

    public int GuichetId { get; set; }

    public Guichet Guichet { get; set; } = null!;
    public ICollection<Transaction> TransactionsSources { get; set; }
    public ICollection<Transaction> TransactionsDestinations { get; set; }
}