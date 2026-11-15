using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using TiendaMVVM.Infrastructure;
using TiendaMVVM.Models;
using TiendaMVVM.Repositories;

namespace TiendaMVVM.ViewModels;

public class ProductosViewModel : ObservableObject
{
    private readonly IProductoRepositorio _repo;

    public ObservableCollection<Producto> Productos { get; } = new();
    public ObservableCollection<Categoria> Categorias { get; } = new();

    private Producto? _seleccionado;
    private string _nombre = "";
    private string _precioTexto = "";
    private Categoria? _categoria;
    private string _mensaje = "";

    public ProductosViewModel(IProductoRepositorio repo)
    {
        _repo = repo;
        CargarCommand = new AsyncRelayCommand(CargarAsync);
        AgregarCommand = new AsyncRelayCommand(AgregarAsync);
        GuardarCommand = new AsyncRelayCommand(GuardarAsync, () => Seleccionado is not null);
        EliminarCommand = new AsyncRelayCommand(EliminarAsync, () => Seleccionado is not null);
        LimpiarCommand = new RelayCommand(Limpiar);
    }

    // La pestaña de Reportes (Parte 2) se engancha aquí
    public event Func<Task>? DatosCambiaron;

    public Producto? Seleccionado
    {
        get => _seleccionado;
        set
        {
            if (!SetField(ref _seleccionado, value)) return;
            if (value is null) return;
            Nombre = value.Nombre;
            PrecioTexto = value.Precio.ToString();
            Categoria = Categorias.FirstOrDefault(c => c.Id == value.CategoriaId);
        }
    }

    public string Nombre { get => _nombre; set => SetField(ref _nombre, value); }
    public string PrecioTexto { get => _precioTexto; set => SetField(ref _precioTexto, value); }
    public Categoria? Categoria { get => _categoria; set => SetField(ref _categoria, value); }
    public string Mensaje { get => _mensaje; set => SetField(ref _mensaje, value); }

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand AgregarCommand { get; }
    public AsyncRelayCommand GuardarCommand { get; }
    public AsyncRelayCommand EliminarCommand { get; }
    public RelayCommand LimpiarCommand { get; }

    public async Task CargarAsync()
    {
        try
        {
            var categorias = await _repo.ObtenerCategoriasAsync();
            Categorias.Clear();
            foreach (var c in categorias) Categorias.Add(c);

            var productos = await _repo.ObtenerTodosAsync();
            Productos.Clear();
            foreach (var p in productos) Productos.Add(p);

            Mensaje = $"{Productos.Count} producto(s) cargado(s).";
        }
        catch (Exception ex)
        {
            Mensaje = "No se pudo cargar: " + ex.Message;
        }
    }

    private async Task AgregarAsync()
    {
        var error = Validar(out var precio);
        if (error is not null) { Mensaje = error; return; }

        // Solo CategoriaId (no el objeto) para que EF no intente insertar la categoría otra vez
        var nuevo = new Producto { Nombre = Nombre.Trim(), Precio = precio, CategoriaId = Categoria!.Id };
        await EjecutarAsync(() => _repo.AgregarAsync(nuevo), "Producto agregado.");
    }

    private async Task GuardarAsync()
    {
        var error = Validar(out var precio);
        if (error is not null) { Mensaje = error; return; }

        var editado = new Producto
        {
            Id = Seleccionado!.Id,
            Nombre = Nombre.Trim(),
            Precio = precio,
            CategoriaId = Categoria!.Id
        };
        await EjecutarAsync(() => _repo.ActualizarAsync(editado), "Producto actualizado.");
    }

    private async Task EliminarAsync()
    {
        var id = Seleccionado!.Id;
        await EjecutarAsync(() => _repo.EliminarAsync(id), "Producto eliminado.");
    }

    private async Task EjecutarAsync(Func<Task> operacion, string exito)
    {
        try
        {
            await operacion();
            await CargarAsync();
            Limpiar();
            Mensaje = exito;
            if (DatosCambiaron is not null) await DatosCambiaron.Invoke();
        }
        catch (DbUpdateException ex)
        {
            Mensaje = "Error al guardar: " + (ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            Mensaje = "Error inesperado: " + ex.Message;
        }
    }

    private void Limpiar()
    {
        Seleccionado = null;
        Nombre = "";
        PrecioTexto = "";
        Categoria = null;
    }

    // TODO PARTE 2 (refactor): extraer esta validación a Services/ValidadorProducto.cs
    private string? Validar(out decimal precio)
    {
        precio = 0;
        if (string.IsNullOrWhiteSpace(Nombre)) return "El nombre es obligatorio.";
        if (Nombre.Trim().Length > 100) return "El nombre no puede pasar de 100 caracteres.";
        if (!decimal.TryParse(PrecioTexto, out precio)) return "El precio no es un número válido.";
        if (precio < 0 || precio > 999999) return "El precio debe estar entre 0 y 999999.";
        if (Categoria is null) return "Seleccione una categoría.";
        return null;
    }
}