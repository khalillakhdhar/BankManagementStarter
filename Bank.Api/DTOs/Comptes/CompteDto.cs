using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Comptes;

public class CompteDto
{
    public int Id { get; set; }
    public string NumeroCompte { get; set; } = string.Empty;
    public decimal Solde { get; set; }
    public DateTime DateOuverture { get; set; }
    public bool IsActive { get; set; }
    public int ClientId { get; set; }
    public string ClientNomComplet { get; set; } = string.Empty;
    public int TypeCompteId { get; set; }
    public string TypeCompte { get; set; } = string.Empty;
    public int GuichetId { get; set; }
    public string Guichet { get; set; } = string.Empty;
}
