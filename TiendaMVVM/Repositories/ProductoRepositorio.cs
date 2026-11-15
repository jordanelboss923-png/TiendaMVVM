using Microsoft.EntityFrameworkCore;
using TiendaMVVM.Data;
using TiendaMVVM.Models;

namespace TiendaMVVM.Repositories;

public class ProductoRepositorio : IProductoRepositorio
{
    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        using var db = new AppDbContext();
        return await db.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .ToListAsync();
    }

    public async Task<List<Categoria>> ObtenerCategoriasAsync()
    {
        using var db = new AppDbContext();
        return await db.Categorias.AsNoTracking().ToListAsync();
    }

    public async Task AgregarAsync(Producto p)
    {
        using var db = new AppDbContext();
        db.Productos.Add(p);                 // Added → INSERT
        await db.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Producto p)
    {
        using var db = new AppDbContext();
        db.Productos.Update(p);              // Modified → UPDATE
        await db.SaveChangesAsync();
    }

    public async Task EliminarAsync(int id)
    {
        using var db = new AppDbContext();
        var p = await db.Productos.FindAsync(id);
        if (p is null) return;
        db.Productos.Remove(p);              // Deleted → DELETE
        await db.SaveChangesAsync();
    }
}