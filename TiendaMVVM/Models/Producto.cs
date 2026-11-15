using System.ComponentModel.DataAnnotations;

namespace TiendaMVVM.Models;

public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = "";

    [Range(0, 999999)]
    public decimal Precio { get; set; }

    public int CategoriaId { get; set; }          // clave foránea
    public Categoria? Categoria { get; set; }     // propiedad de navegación
}