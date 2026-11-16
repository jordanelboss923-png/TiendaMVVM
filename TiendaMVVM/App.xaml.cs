using System.Windows;
using TiendaMVVM.Data;
using TiendaMVVM.Repositories;
using TiendaMVVM.ViewModels;

namespace TiendaMVVM;

// Raíz de composición: el único lugar que conoce las clases concretas
public partial class App : Application
{
    // false = EF Core | true = ADO.NET
    private static readonly bool UsarAdoNet = false;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DbInicializador.Inicializar();

        IProductoRepositorio repo = UsarAdoNet
            ? new ProductoAdoRepositorio()
            : new ProductoRepositorio();

        var main = new MainViewModel(repo);
        var ventana = new MainWindow { DataContext = main };
        ventana.Show();

        await main.InicializarAsync();
    }
}