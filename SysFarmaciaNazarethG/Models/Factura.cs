using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Factura
{
    [Key]
    public int IdFactura { get; set; }

    public int? Cantidad { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? PrecioTotal { get; set; }

    public DateOnly? FechaDeEmisión { get; set; }

    [StringLength(50)]
    public string? MétodoDePago { get; set; }

    [StringLength(20)]
    public string? Estado { get; set; }

    public int? IdVenta { get; set; }

    [ForeignKey("IdVenta")]
    [InverseProperty("Factura")]
    public virtual Venta? IdVentaNavigation { get; set; }
}
