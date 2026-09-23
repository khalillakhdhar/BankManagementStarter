using Bank.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Bank.Api.Models;

public class Transaction
{
    public long Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string Reference { get; set; } = string.Empty;

    public TypeTransaction Type { get; set; }

    public decimal Montant { get; set; }

    public DateTime DateOperation { get; set; } = DateTime.UtcNow;

    [MaxLength(250)]
    public string? Description { get; set; }

    public int CompteSourceId { get; set; }

    public Compte CompteSource { get; set; } = null!;

    public int? CompteDestinationId { get; set; }

    public Compte? CompteDestination { get; set; }

    [Required]
    public string AgentId { get; set; } = string.Empty;

    public ApplicationUser Agent { get; set; } = null!;
}