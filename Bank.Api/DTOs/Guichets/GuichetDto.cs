using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Guichets;

public class GuichetDto
{
    public int Id { get; set; }

   
    public string Code { get; set; } = string.Empty;
    
    public string Nom { get; set; } = string.Empty;
  
    public string Adresse { get; set; } = string.Empty;
 
    public string Ville { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime DateCreation { get; set; }
    public int NombreAgents { get; set; }
}
