using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Bank.Api.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(80)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(80)]
    public string Prenom { get; set; } = string.Empty;

    public int? GuichetId { get; set; }

    public Guichet? Guichet { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;
}