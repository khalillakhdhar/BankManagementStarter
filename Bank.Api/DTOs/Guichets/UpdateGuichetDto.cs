using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Guichets;

public class UpdateGuichetDto
{
    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;
    [Required]
    [MaxLength(200)]
    public string Adresse { get; set; } = string.Empty;
    [Required]
    [MaxLength(100)]
    public string Ville { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public bool IsActive { get; set; } = true;
}
