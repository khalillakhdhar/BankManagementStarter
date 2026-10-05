using Bank.Api.Configuration;
using Bank.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Data;

public static class IdentitySeeder
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { "Admin", "Agent" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var admin = await userManager.FindByEmailAsync(LocalDevelopmentSettings.AdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = LocalDevelopmentSettings.AdminEmail,
                Email = LocalDevelopmentSettings.AdminEmail,
                EmailConfirmed = true,
                Nom = LocalDevelopmentSettings.AdminLastName,
                Prenom = LocalDevelopmentSettings.AdminFirstName
            };
            var result = await userManager.CreateAsync(admin, LocalDevelopmentSettings.AdminPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(" ", result.Errors.Select(error => error.Description)));
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}
