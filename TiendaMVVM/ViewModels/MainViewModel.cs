using TiendaMVVM.Infrastructure;
using TiendaMVVM.Repositories;

namespace TiendaMVVM.ViewModels;

public class MainViewModel : ObservableObject
{
    public ProductosViewModel Productos { get; }
    // PARTE 2: tu compañero agrega aquí ReportesViewModel

    public MainViewModel(IProductoRepositorio repo)
    {
        Productos = new ProductosViewModel(repo);
    }

    public async Task InicializarAsync()
    {
        await Productos.CargarAsync();
    }
}