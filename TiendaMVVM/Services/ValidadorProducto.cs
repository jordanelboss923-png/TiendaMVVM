using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaMVVM.Models;

namespace TiendaMVVM.Services
{
    public static class ValidadorProducto
    {
        public static string? Validar(string nombre, string precioTexto, Categoria? categoria, out decimal precio)
        {
            precio = 0;
            if (string.IsNullOrWhiteSpace(nombre)) return "El nombre es obligatorio ";
            if (nombre.Trim().Length > 100) return "El nombre no puede pasar de 100 caracteres.";
            if (!decimal.TryParse(precioTexto, out precio)) return "El precio no es un número válido.";
            if (precio < 0 || precio > 999999) return "El precio debe estar entre 0 y 999999.";
            if (categoria is null) return "Seleccione una categoría.";
            return null;
        }
    }
}
