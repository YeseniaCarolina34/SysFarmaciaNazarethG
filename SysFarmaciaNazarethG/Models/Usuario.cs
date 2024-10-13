using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Usuario
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [StringLength(50)]
    public string Apellido { get; set; } = null!;

    [StringLength(50)]
    public string Login { get; set; } = null!;

    [StringLength(100)]
    public string Password { get; set; } = null!;

    [StringLength(20)]
    public string? Estatus { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public int? IdRol { get; set; }

    [ForeignKey("IdRol")]
    [InverseProperty("Usuario")]
    public virtual Rol? IdRolNavigation { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Proveedor> Proveedor { get; set; } = new List<Proveedor>();
}
