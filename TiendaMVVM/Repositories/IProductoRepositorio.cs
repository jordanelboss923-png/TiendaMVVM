using TiendaMVVM.Models;

namespace TiendaMVVM.Repositories;

public interface IProductoRepositorio
{
    Task<List<Producto>> ObtenerTodosAsync();
    Task<List<Categoria>> ObtenerCategoriasAsync();
    Task AgregarAsync(Producto p);
    Task ActualizarAsync(Producto p);
    Task EliminarAsync(int id);
}