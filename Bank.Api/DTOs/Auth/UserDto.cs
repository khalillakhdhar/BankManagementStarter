using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Auth;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? GuichetId { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}
