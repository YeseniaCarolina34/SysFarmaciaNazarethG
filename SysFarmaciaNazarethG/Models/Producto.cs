using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Producto
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripción { get; set; }

    public int? IdCategoría { get; set; }

    public int? IdProveedor { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioCosto { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioVenta { get; set; }

    [StringLength(50)]
    public string? CódigoDeBarras { get; set; }

    public int? CantidadEnInventario { get; set; }

    public DateOnly? FechaDeIngreso { get; set; }

    public DateOnly? FechaDeCaducidad { get; set; }

    [StringLength(20)]
    public string? Estado { get; set; }

    [StringLength(100)]
    public string? Laboratorio { get; set; }

    [ForeignKey("IdProveedor")]
    [InverseProperty("Producto")]
    public virtual Proveedor? IdProveedorNavigation { get; set; }

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<Inventario> Inventario { get; set; } = new List<Inventario>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
