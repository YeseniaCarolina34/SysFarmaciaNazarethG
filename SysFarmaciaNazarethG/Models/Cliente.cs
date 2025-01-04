using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Models;

public partial class Cliente
{
    [Key]
    public int IdCliente { get; set; }

    public string? Nombre { get; set; }

    public string Direccion { get; set; } = null!;

    [Column("Venta_a_Cuenta_de")]
    public string? VentaACuentaDe { get; set; }

    [InverseProperty("IdClienteNavigation")]
    public virtual ICollection<Factura> Factura { get; set; } = new List<Factura>();

}