using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Clients;

public class ClientDto
{
    public int Id { get; set; }
  
    public string Cin { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public DateOnly? DateNaissance { get; set; }
  
    public string? Email { get; set; }
  
    public string Telephone { get; set; } = string.Empty;
  
    public string? Adresse { get; set; }
    public DateTime DateCreation { get; set; }
    public bool IsActive { get; set; }
}
