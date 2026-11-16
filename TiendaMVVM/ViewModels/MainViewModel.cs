using TiendaMVVM.Infrastructure;
using TiendaMVVM.Repositories;

namespace TiendaMVVM.ViewModels;

public class MainViewModel : ObservableObject
{
    public ProductosViewModel Productos { get; }
    public ReportesViewModel Reportes { get; }

    public MainViewModel(IProductoRepositorio repo)
    {
        Productos = new ProductosViewModel(repo);
        Reportes = new ReportesViewModel(repo);

        // Cuando cambian los productos, el reporte se refresca solo
        Productos.DatosCambiaron += Reportes.ActualizarAsync;
    }

    public async Task InicializarAsync()
    {
        await Productos.CargarAsync();
        await Reportes.ActualizarAsync();
    }
}