using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Inventario
{
    [Key]
    public int IdInventario { get; set; }

    public int? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    [StringLength(100)]
    public string? Ubicación { get; set; }

    public DateOnly? FechaDeIngreso { get; set; }

    [StringLength(20)]
    public string? Estado { get; set; }

    [InverseProperty("IdInventarioNavigation")]
    public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();

    [ForeignKey("IdProducto")]
    [InverseProperty("Inventario")]
    public virtual Producto? IdProductoNavigation { get; set; }
}
