using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TiendaMVVM.Data;
using TiendaMVVM.Models;

namespace TiendaMVVM.Repositories;

// Misma interfaz que ProductoRepositorio, pero con ADO.NET puro
public class ProductoAdoRepositorio : IProductoRepositorio
{
    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        var lista = new List<Producto>();

        using var con = new SqliteConnection(Conexion.Cadena);
        await con.OpenAsync();

        const string sql = @"
            SELECT p.Id, p.Nombre, p.Precio, p.CategoriaId, c.Nombre
            FROM Productos p
            INNER JOIN Categorias c ON c.Id = p.CategoriaId
            ORDER BY p.Id";

        using var cmd = new SqliteCommand(sql, con);
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            lista.Add(new Producto
            {
                Id = rd.GetInt32(0),
                Nombre = rd.GetString(1),
                Precio = rd.GetDecimal(2),
                CategoriaId = rd.GetInt32(3),
                Categoria = new Categoria { Id = rd.GetInt32(3), Nombre = rd.GetString(4) }
            });
        }
        return lista;
    }

    public async Task<List<Categoria>> ObtenerCategoriasAsync()
    {
        var lista = new List<Categoria>();

        using var con = new SqliteConnection(Conexion.Cadena);
        await con.OpenAsync();

        using var cmd = new SqliteCommand("SELECT Id, Nombre FROM Categorias ORDER BY Nombre", con);
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
            lista.Add(new Categoria { Id = rd.GetInt32(0), Nombre = rd.GetString(1) });

        return lista;
    }

    public Task AgregarAsync(Producto p) => EjecutarAsync(
        "INSERT INTO Productos (Nombre, Precio, CategoriaId) VALUES ($nombre, $precio, $categoriaId)",
        ("$nombre", p.Nombre), ("$precio", p.Precio), ("$categoriaId", p.CategoriaId));

    public Task ActualizarAsync(Producto p) => EjecutarAsync(
        "UPDATE Productos SET Nombre = $nombre, Precio = $precio, CategoriaId = $categoriaId WHERE Id = $id",
        ("$nombre", p.Nombre), ("$precio", p.Precio), ("$categoriaId", p.CategoriaId), ("$id", p.Id));

    public Task EliminarAsync(int id) => EjecutarAsync(
        "DELETE FROM Productos WHERE Id = $id",
        ("$id", id));

    // Connection + Command + parámetros (nunca concatenar texto en el SQL)
    private static async Task EjecutarAsync(string sql, params (string nombre, object valor)[] parametros)
    {
        using var con = new SqliteConnection(Conexion.Cadena);
        await con.OpenAsync();

        using var cmd = new SqliteCommand(sql, con);
        foreach (var (nombre, valor) in parametros)
            cmd.Parameters.AddWithValue(nombre, valor);

        await cmd.ExecuteNonQueryAsync();
    }
}