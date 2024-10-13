using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Venta
{
    [Key]
    public int IdVenta { get; set; }

    public int? Cantidad { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioTotal { get; set; }

    [StringLength(100)]
    public string? EnviosDeSucursal { get; set; }

    public int? IdProducto { get; set; }

    [InverseProperty("IdVentaNavigation")]
    public virtual ICollection<Factura> Factura { get; set; } = new List<Factura>();

    [ForeignKey("IdProducto")]
    [InverseProperty("Venta")]
    public virtual Producto? IdProductoNavigation { get; set; }
}
