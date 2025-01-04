using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Models;

public partial class Factura
{
    [Key]
    public int IdFactura { get; set; }

    public int? FacturaNo { get; set; }

    public DateOnly? Fecha { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? SubTotal { get; set; }

    [Column("IVA", TypeName = "decimal(10, 2)")]
    public decimal? Iva { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Total { get; set; }

    public int? IdVenta { get; set; }

    public int? IdCliente { get; set; }

    public int? IdProducto { get; set; }

    [InverseProperty("IdFacturaNavigation")]
    public virtual ICollection<DetalleFactura> DetalleFactura { get; set; } = new List<DetalleFactura>();

    [ForeignKey("IdCliente")]
    [InverseProperty("Factura")]
    public virtual Cliente? IdClienteNavigation { get; set; }


    [ForeignKey("IdProducto")]
    [InverseProperty("Factura")]
    public virtual Producto? IdProductoNavigation { get; set; }

    [ForeignKey("IdVenta")]
    [InverseProperty("Factura")]
    public virtual Venta? IdVentaNavigation { get; set; }
}