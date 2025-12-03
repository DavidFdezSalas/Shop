using Microsoft.EntityFrameworkCore;
using Shop.APIProducts.Models;

namespace Shop.APIProducts.Data
{
    public static class ProductsSeeder
    {
        public static async Task SeedCategoriesAndProductsAsync(ProductDbContext context, ILogger logger)
        {
            logger.LogInformation("Iniciando seed de categorías y productos...");

            // Si ya hay datos, no hacer nada
            if (await context.Categories.AnyAsync())
            {
                logger.LogInformation("La base de datos ya contiene categorías. No se ejecuta el seed.");
                return;
            }

            // ===== CREAR CATEGORÍAS =====
            var categories = new List<Categories>
            {
                new() { Name = "Electrónica", Description = "Dispositivos electrónicos y gadgets" },
                new() { Name = "Ropa", Description = "Prendas de vestir para hombre y mujer" },
                new() { Name = "Deportes", Description = "Equipamiento deportivo y fitness" },
                new() { Name = "Hogar", Description = "Artículos para el hogar y decoración" },
                new() { Name = "Juguetes", Description = "Juguetes y juegos para niños" },
                new() { Name = "Libros", Description = "Libros físicos y digitales" },
                new() { Name = "Belleza", Description = "Productos de cuidado personal y cosmética" },
                new() { Name = "Alimentos", Description = "Alimentos y bebidas" },
                new() { Name = "Automóviles", Description = "Accesorios y repuestos para vehículos" },
                new() { Name = "Mascotas", Description = "Productos para el cuidado de mascotas" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            logger.LogInformation("Categorías creadas exitosamente.");

            // ===== CREAR PRODUCTOS =====
            var products = new List<Products>
            {
                // Electrónica (ID: 1)
                new() { Name = "Laptop Gaming HP Omen", Description = "Laptop de alto rendimiento con RTX 4060", Price = 1299.99m, Stock = 15, CategoriesId = 1, ImageUrl = "/images/productos/laptop-hp.jpg" },
                new() { Name = "Mouse Logitech G502", Description = "Mouse gaming con sensor HERO 25K", Price = 79.99m, Stock = 50, CategoriesId = 1, ImageUrl = "/images/productos/mouse-logitech.jpg" },

                // Ropa (ID: 2)
                new() { Name = "Camiseta Nike Dri-FIT", Description = "Camiseta deportiva de secado rápido", Price = 29.99m, Stock = 100, CategoriesId = 2, ImageUrl = "/images/productos/camiseta-nike.jpg" },
                new() { Name = "Jeans Levi's 501", Description = "Jeans clásicos de corte recto", Price = 89.99m, Stock = 75, CategoriesId = 2, ImageUrl = "/images/productos/jeans-levis.jpg" },

                // Deportes (ID: 3)
                new() { Name = "Balón de Fútbol Adidas", Description = "Balón profesional talla 5", Price = 49.99m, Stock = 40, CategoriesId = 3, ImageUrl = "/images/productos/balon-adidas.jpg" },
                new() { Name = "Mancuernas Ajustables 20kg", Description = "Par de mancuernas con peso ajustable", Price = 129.99m, Stock = 20, CategoriesId = 3, ImageUrl = "/images/productos/mancuernas.jpg" },

                // Hogar (ID: 4)
                new() { Name = "Lámpara de Mesa LED", Description = "Lámpara moderna con regulador de intensidad", Price = 39.99m, Stock = 60, CategoriesId = 4, ImageUrl = "/images/productos/lampara-led.jpg" },
                new() { Name = "Cojín Decorativo 45x45cm", Description = "Cojín de algodón con funda lavable", Price = 19.99m, Stock = 150, CategoriesId = 4, ImageUrl = "/images/productos/cojin.jpg" },

                // Juguetes (ID: 5)
                new() { Name = "LEGO Star Wars Millennium Falcon", Description = "Set de construcción de 1351 piezas", Price = 159.99m, Stock = 25, CategoriesId = 5, ImageUrl = "/images/productos/lego-star-wars.jpg" },
                new() { Name = "Barbie Fashionista", Description = "Muñeca Barbie con accesorios", Price = 24.99m, Stock = 80, CategoriesId = 5, ImageUrl = "/images/productos/barbie.jpg" },

                // Libros (ID: 6)
                new() { Name = "Cien Años de Soledad - García Márquez", Description = "Novela clásica de literatura latinoamericana", Price = 14.99m, Stock = 120, CategoriesId = 6, ImageUrl = "/images/productos/cien-anos-soledad.jpg" },
                new() { Name = "El Quijote - Cervantes", Description = "Edición conmemorativa ilustrada", Price = 29.99m, Stock = 50, CategoriesId = 6, ImageUrl = "/images/productos/quijote.jpg" },

                // Belleza (ID: 7)
                new() { Name = "Crema Facial L'Oréal Paris", Description = "Crema hidratante anti-edad 50ml", Price = 19.99m, Stock = 90, CategoriesId = 7, ImageUrl = "/images/productos/crema-loreal.jpg" },
                new() { Name = "Set de Brochas Maquillaje", Description = "12 brochas profesionales con estuche", Price = 34.99m, Stock = 45, CategoriesId = 7, ImageUrl = "/images/productos/brochas.jpg" },

                // Alimentos (ID: 8)
                new() { Name = "Café Grano Arábica Premium 1kg", Description = "Café de origen colombiano", Price = 24.99m, Stock = 200, CategoriesId = 8, ImageUrl = "/images/productos/cafe-arabica.jpg" },
                new() { Name = "Aceite de Oliva Virgen Extra 500ml", Description = "Aceite de primera presión en frío", Price = 12.99m, Stock = 150, CategoriesId = 8, ImageUrl = "/images/productos/aceite-oliva.jpg" },

                // Automóviles (ID: 9)
                new() { Name = "Neumático Michelin 205/55R16", Description = "Neumático de alto rendimiento", Price = 89.99m, Stock = 30, CategoriesId = 9, ImageUrl = "/images/productos/neumatico-michelin.jpg" },
                new() { Name = "Kit de Herramientas para Coche", Description = "45 piezas con maletín", Price = 49.99m, Stock = 40, CategoriesId = 9, ImageUrl = "/images/productos/kit-herramientas.jpg" },

                // Mascotas (ID: 10)
                new() { Name = "Cama para Perro Grande", Description = "Cama ortopédica con funda lavable", Price = 59.99m, Stock = 35, CategoriesId = 10, ImageUrl = "/images/productos/cama-perro.jpg" },
                new() { Name = "Rascador para Gato", Description = "Torre rascador de 120cm con plataformas", Price = 79.99m, Stock = 25, CategoriesId = 10, ImageUrl = "/images/productos/rascador-gato.jpg" }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            logger.LogInformation("Productos creados exitosamente.");
            logger.LogInformation("Seed completado: {CategoriesCount} categorías y {ProductsCount} productos.", categories.Count, products.Count);
        }
    }
}