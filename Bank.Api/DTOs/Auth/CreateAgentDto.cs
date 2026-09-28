using System.ComponentModel.DataAnnotations;

namespace Bank.Api.DTOs.Auth;

public class CreateAgentDto
{
    [Required, MaxLength(80)] public string Nom { get; set; } = string.Empty;
    [Required, MaxLength(80)] public string Prenom { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)] public string Password { get; set; } = string.Empty;
    public int? GuichetId { get; set; }
}
