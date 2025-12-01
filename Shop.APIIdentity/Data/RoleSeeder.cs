using Microsoft.AspNetCore.Identity;

namespace Shop.APIIdentity.Data
{
    public static class RoleSeeder
    {
        public static async Task SeeddRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@shop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
        }
    }
}
