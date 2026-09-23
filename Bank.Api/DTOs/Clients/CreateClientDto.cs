using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Clients;

public class CreateClientDto
{
    [Required]
    [StringLength(20, MinimumLength = 4)]
    public string Cin { get; set; } = string.Empty;
    [Required]
    [MaxLength(30)]
    public string Nom { get; set; } = string.Empty;
    [Required]
    [MaxLength(30)]
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



}
