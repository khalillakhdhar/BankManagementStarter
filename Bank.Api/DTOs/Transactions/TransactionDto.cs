using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Transactions;

public class TransactionDto
{
    public long Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public Enums.TypeTransaction Type { get; set; }
    public decimal Montant { get; set; }
    public DateTime DateOperation { get; set; }
    public string? Description { get; set; }
    public int CompteSourceId { get; set; }
    public int? CompteDestinationId { get; set; }
    public string AgentId { get; set; } = string.Empty;
}
