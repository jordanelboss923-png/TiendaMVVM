using System.ComponentModel.DataAnnotations;

namespace TiendaMVVM.Models;

public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Nombre { get; set; } = "";

    public List<Producto> Productos { get; set; } = new();

    public override string ToString() => Nombre;
}