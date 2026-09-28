using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Comptes;

public class CreateCompteDto
{
    [Required, StringLength(34, MinimumLength = 4)]
    public string NumeroCompte { get; set; } = string.Empty;
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal SoldeInitial { get; set; }
    [Range(1, int.MaxValue)] public int ClientId { get; set; }
    [Range(1, int.MaxValue)] public int TypeCompteId { get; set; }
    [Range(1, int.MaxValue)] public int GuichetId { get; set; }
}
