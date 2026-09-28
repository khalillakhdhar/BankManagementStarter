using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.TypesComptes;

public class CreateTypeCompteDto
{
    [Required, StringLength(30, MinimumLength = 2)] public string Code { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(300)] public string? Description { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal SoldeMinimum { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal FraisMensuels { get; set; }
}
