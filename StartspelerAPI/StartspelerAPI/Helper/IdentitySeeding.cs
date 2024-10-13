using Microsoft.AspNetCore.Identity;
using StartspelerAPI.Models;
using System.Data.Common;

namespace StartspelerAPI.Helper
{
    public class IdentitySeeding
    {
        public async Task IdentitySeedingAsync(UserManager<Gebruiker> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Rollen aanmaken 
                // Gebruiker
                bool role = await roleManager.RoleExistsAsync("gebruiker");
                if (!role) await roleManager.CreateAsync(new IdentityRole("gebruiker"));

                // CommunityManager
                role = await roleManager.RoleExistsAsync("communityManager");
                if (!role) await roleManager.CreateAsync(new IdentityRole("communityManager"));

                // Admin
                role = await roleManager.RoleExistsAsync("admin");
                if (!role) await roleManager.CreateAsync(new IdentityRole("admin"));

                if (userManager.FindByNameAsync("Admin").Result == null)
                {
                    var defaultUser = new Gebruiker
                    {
                        UserName = "Admin",
                        Email = "admin@thomasmore.be",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    // Gebruiker aanmaken met rol
                    var admin = await userManager.CreateAsync(defaultUser, "t0LTHxzy.v");
                    if (admin.Succeeded && userManager.FindByNameAsync("Admin").Result != null)
                        await userManager.AddToRoleAsync(defaultUser, "admin");

                }
            }
            catch (DbException ex) { throw new Exception(ex.Message.ToString()); }
        }
    }
}
