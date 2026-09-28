using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Comptes;

public class CompteSummaryDto
{
    public int Id { get; set; }
    public string NumeroCompte { get; set; } = string.Empty;
    public decimal Solde { get; set; }
    public string TypeCompte { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
