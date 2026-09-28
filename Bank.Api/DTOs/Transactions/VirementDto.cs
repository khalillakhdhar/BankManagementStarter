using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Transactions;

public class VirementDto
{
    [Range(1, int.MaxValue)] public int CompteSourceId { get; set; }
    [Range(1, int.MaxValue)] public int CompteDestinationId { get; set; }
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")] public decimal Montant { get; set; }
    [MaxLength(250)] public string? Description { get; set; }
}
