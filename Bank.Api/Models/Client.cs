namespace Bank.Api.Models;

using System.ComponentModel.DataAnnotations;


public class Client
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Cin { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Prenom { get; set; } = string.Empty;

    public DateOnly? DateNaissance { get; set; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(30)]
    public string Telephone { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Adresse { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public ICollection<Compte> Comptes { get; set; }
        = new List<Compte>();
}
