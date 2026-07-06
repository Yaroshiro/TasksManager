using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TasksManager.Models;

namespace TasksManager.Areas.Identity
{
    public class CustomRoles
    {
        public static async Task AddRoles(IServiceScope scope)
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (string role in Enum.GetNames(typeof(ApplicationUser.RoleType)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
