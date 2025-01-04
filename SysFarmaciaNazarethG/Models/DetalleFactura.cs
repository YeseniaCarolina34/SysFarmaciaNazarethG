using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Models;

public partial class DetalleFactura
{
    [Key]
    public int IdDetalleFactura { get; set; }

    public int? IdFactura { get; set; }

    public int? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public string? Descripcion { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Total { get; set; }

    [ForeignKey("IdFactura")]
    [InverseProperty("DetalleFactura")]
    public virtual Factura? IdFacturaNavigation { get; set; }

    [ForeignKey("IdProducto")]
    [InverseProperty("DetalleFactura")]
    public virtual Producto? IdProductoNavigation { get; set; }
}