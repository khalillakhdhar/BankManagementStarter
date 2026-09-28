using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Transactions;

public class DepotDto
{
    [Range(1, int.MaxValue)] public int CompteId { get; set; }
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")] public decimal Montant { get; set; }
    [MaxLength(250)] public string? Description { get; set; }
}
