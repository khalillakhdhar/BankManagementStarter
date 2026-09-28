using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.TypesComptes;

public class TypeCompteDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal SoldeMinimum { get; set; }
    public decimal FraisMensuels { get; set; }
    public bool IsActive { get; set; }
}
