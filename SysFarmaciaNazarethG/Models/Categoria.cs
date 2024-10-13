using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Categoria
{
    [Key]
    public int IdCategoria { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(255)]
    public string? Descripción { get; set; }

    public int? IdInventario { get; set; }

    [ForeignKey("IdInventario")]
    [InverseProperty("Categoria")]
    public virtual Inventario? IdInventarioNavigation { get; set; }
}
