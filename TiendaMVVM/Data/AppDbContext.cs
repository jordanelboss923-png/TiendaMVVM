using Microsoft.EntityFrameworkCore;
using TiendaMVVM.Models;

namespace TiendaMVVM.Data;

public class AppDbContext : DbContext
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Categoria> Categorias => Set<Categoria>();

    protected override void OnConfiguring(DbContextOptionsBuilder o)
        => o.UseSqlite(Conexion.Cadena);
}

// Cadena compartida: la usará también ADO.NET (Parte 2)
public static class Conexion
{
    public const string Cadena = "Data Source=tienda.db";
}