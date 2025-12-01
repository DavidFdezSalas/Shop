using Microsoft.AspNetCore.Identity;

namespace Shop.APIIdentity.Data
{
    public static class RoleSeeder
    {
        public static async Task SeeddRolesAndAdminAsync(IServiceProvider serviceProvider, ILogger logger)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            logger.LogInformation("Iniciando seed de roles y usuario admin...");
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Rol '{Role}' creado exitosamente.", role);
                }
            }

            var adminEmail = "admin@shop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    EmailConfirmed = true
                };


                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Usuario admin creado y rol asignado exitosamente.");
                }
            }
            else
            {
                logger.LogInformation("Usuario admin ya existe en la base de datos.");
            }
        }
    }
}
