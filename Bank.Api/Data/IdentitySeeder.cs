using Bank.Api.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bank.Api.Models;
namespace Bank.Api.Data
{
    public static class IdentitySeeder
    {

        public static async Task initializeAsync(IServiceProvider services)
        { 
            var context= services.GetRequiredService<AppDbContext>();

            await context.Database.MigrateAsync();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in new[] { "Admin", "Agent" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                var userManager = services.GetRequiredService<UserManager<Models.ApplicationUser>>();
                var admin=await userManager.FindByEmailAsync(LocalDevelopmentSettings.AdminEmail);
                if (admin != null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = LocalDevelopmentSettings.AdminEmail,
                        Email = LocalDevelopmentSettings.AdminEmail,
                        Nom = LocalDevelopmentSettings.AdminLastName,
                        Prenom = LocalDevelopmentSettings.AdminFirstName,
                        IsActive = true,
                        DateCreation = DateTime.UtcNow
                    };
                    var result = await userManager.CreateAsync(admin, LocalDevelopmentSettings.AdminPassword);
                    if(!result.Succeeded)
                    {
                        throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    if(!await userManager.IsInRoleAsync(admin, "Admin"))
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }


        }
    }
}
