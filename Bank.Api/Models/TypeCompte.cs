namespace Bank.Api.Models;

using System.ComponentModel.DataAnnotations;


public class TypeCompte
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Libelle { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    public decimal SoldeMinimum { get; set; }

    public decimal FraisMensuels { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Compte> Comptes { get; set; }
        = new List<Compte>();
}