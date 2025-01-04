using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Models;

public partial class Producto
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Descripción { get; set; }

    public int? IdCategoría { get; set; }

    public int? IdProveedor { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioCosto { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal PrecioVenta { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? CódigoDeBarras { get; set; }

    public int? CantidadEnInventario { get; set; }

    public DateOnly? FechaDeIngreso { get; set; }

    public DateOnly? FechaDeCaducidad { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Estado { get; set; }

    [StringLength(100)]
    public string? Laboratorio { get; set; }

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<DetalleFactura> DetalleFactura { get; set; } = new List<DetalleFactura>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<Factura> Factura { get; set; } = new List<Factura>();


    [ForeignKey("IdProveedor")]
    [InverseProperty("Producto")]
    public virtual Proveedor? IdProveedorNavigation { get; set; }

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<Inventario> Inventario { get; set; } = new List<Inventario>();

    [InverseProperty("IdProductoNavigation")]
    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}