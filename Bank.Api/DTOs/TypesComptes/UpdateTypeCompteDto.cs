using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.TypesComptes;

public class UpdateTypeCompteDto
{
    [Required, MaxLength(100)] public string Libelle { get; set; } = string.Empty;
    [MaxLength(300)] public string? Description { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal SoldeMinimum { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal FraisMensuels { get; set; }
    public bool IsActive { get; set; }
}
