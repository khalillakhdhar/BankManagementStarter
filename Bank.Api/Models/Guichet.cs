using System.ComponentModel.DataAnnotations;

namespace Bank.Api.Models;

public class Guichet
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Adresse { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Ville { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Telephone { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public ICollection<ApplicationUser> Agents { get; set; }
        = new List<ApplicationUser>();

    public ICollection<Compte> Comptes { get; set; }
        = new List<Compte>();
}