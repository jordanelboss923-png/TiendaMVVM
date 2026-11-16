using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using TiendaMVVM.Infrastructure;
using TiendaMVVM.Models;
using TiendaMVVM.Repositories;

namespace TiendaMVVM.ViewModels;

public record ResumenCategoria(string Nombre, int Cantidad, decimal Total, decimal Promedio);

public class ReportesViewModel : ObservableObject
{
    private readonly IProductoRepositorio _repo;
    private List<Producto> _todos = new();

    public ObservableCollection<ResumenCategoria> PorCategoria { get; } = new();
    public ObservableCollection<Producto> Filtrados { get; } = new();

    private string _precioMinimoTexto = "";
    private int _totalProductos;
    private decimal _valorTotal;
    private decimal _precioPromedio;
    private string _productoMasCaro = "-";
    private string _mensaje = "";

    public ReportesViewModel(IProductoRepositorio repo)
    {
        _repo = repo;
        ActualizarCommand = new AsyncRelayCommand(ActualizarAsync);
    }

    public AsyncRelayCommand ActualizarCommand { get; }

    public string PrecioMinimoTexto
    {
        get => _precioMinimoTexto;
        set { if (SetField(ref _precioMinimoTexto, value)) AplicarFiltro(); }
    }

    public int TotalProductos { get => _totalProductos; private set => SetField(ref _totalProductos, value); }
    public decimal ValorTotal { get => _valorTotal; private set => SetField(ref _valorTotal, value); }
    public decimal PrecioPromedio { get => _precioPromedio; private set => SetField(ref _precioPromedio, value); }
    public string ProductoMasCaro { get => _productoMasCaro; private set => SetField(ref _productoMasCaro, value); }
    public string Mensaje { get => _mensaje; private set => SetField(ref _mensaje, value); }

    public async Task ActualizarAsync()
    {
        try
        {
            _todos = await _repo.ObtenerTodosAsync();

            // LINQ: Count, Sum, Average, OrderByDescending
            TotalProductos = _todos.Count;
            ValorTotal = _todos.Sum(p => p.Precio);
            PrecioPromedio = _todos.Select(p => p.Precio).DefaultIfEmpty(0m).Average();

            var caro = _todos.OrderByDescending(p => p.Precio).FirstOrDefault();
            ProductoMasCaro = caro is null ? "-" : $"{caro.Nombre} ({caro.Precio:N2})";

            // LINQ: GroupBy + Count + Sum + Average
            var grupos = _todos
                .GroupBy(p => p.Categoria?.Nombre ?? "Sin categoría")
                .Select(g => new ResumenCategoria(
                    g.Key,
                    g.Count(),
                    g.Sum(p => p.Precio),
                    g.Average(p => p.Precio)))
                .OrderByDescending(r => r.Total);

            PorCategoria.Clear();
            foreach (var r in grupos) PorCategoria.Add(r);

            AplicarFiltro();
            Mensaje = "";
        }
        catch (Exception ex)
        {
            Mensaje = "No se pudo generar el reporte: " + ex.Message;
        }
    }

    // LINQ: Where + OrderBy
    private void AplicarFiltro()
    {
        decimal.TryParse(PrecioMinimoTexto, out var minimo); // si no es número, minimo = 0

        var filtrados = _todos
            .Where(p => p.Precio >= minimo)
            .OrderBy(p => p.Precio);

        Filtrados.Clear();
        foreach (var p in filtrados) Filtrados.Add(p);
    }
}