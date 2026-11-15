using TiendaMVVM.Models;

namespace TiendaMVVM.Data;

public static class DbInicializador
{
    public static void Inicializar()
    {
        using var db = new AppDbContext();
        db.Database.EnsureCreated();

        if (db.Categorias.Any()) return;

        var bebidas = new Categoria { Nombre = "Bebidas" };
        var tecno = new Categoria { Nombre = "Tecnología" };
        var hogar = new Categoria { Nombre = "Hogar" };
        db.Categorias.AddRange(bebidas, tecno, hogar);

        db.Productos.AddRange(
            new Producto { Nombre = "Café", Precio = 4.50m, Categoria = bebidas },
            new Producto { Nombre = "Laptop", Precio = 850m, Categoria = tecno },
            new Producto { Nombre = "Mouse", Precio = 25m, Categoria = tecno },
            new Producto { Nombre = "Monitor", Precio = 300m, Categoria = tecno },
            new Producto { Nombre = "Taza", Precio = 8m, Categoria = hogar });
        db.SaveChanges();
    }
}