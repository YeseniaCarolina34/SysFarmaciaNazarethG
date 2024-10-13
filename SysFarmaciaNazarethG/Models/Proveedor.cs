using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Proveedor
{
    [Key]
    public int IdProveedor { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("DUI")]
    [StringLength(20)]
    public string? Dui { get; set; }

    [StringLength(100)]
    public string? PersonaDeContacto { get; set; }

    [StringLength(20)]
    public string? Teléfono { get; set; }

    [StringLength(100)]
    public string? CorreoElectrónico { get; set; }

    [StringLength(255)]
    public string? Dirección { get; set; }

    [StringLength(50)]
    public string? País { get; set; }

    [StringLength(50)]
    public string? MétodoDePago { get; set; }

    [Column("BancoYNumeroDeCuenta")]
    [StringLength(100)]
    public string? BancoYnumeroDeCuenta { get; set; }

    [StringLength(50)]
    public string? MonedaDeTransacción { get; set; }

    public int? IdUsuario { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Proveedor")]
    public virtual Usuario? IdUsuarioNavigation { get; set; }

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<Producto> Producto { get; set; } = new List<Producto>();
}
