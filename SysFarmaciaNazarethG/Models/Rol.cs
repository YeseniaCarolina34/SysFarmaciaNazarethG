using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SysFarmaciaNazarethG.Models;

public partial class Rol
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [InverseProperty("IdRolNavigation")]
    public virtual ICollection<Usuario> Usuario { get; set; } = new List<Usuario>();
}
