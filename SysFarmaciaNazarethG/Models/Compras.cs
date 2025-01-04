using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace SysFarmaciaNazarethG.Models
{
    public class Compras
    {
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string Nombre { get; set; }  // Nuevo campo 'Nombre'

        [Required]
        [StringLength(50)]
        [Unicode(false)]
        public string? Codigo { get; set; } 

        [StringLength(255)]
        [Unicode(false)]
        public string? Descripcion { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Bonificacion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecioUnitario { get; set; }

        [StringLength(100)]
        [Unicode(false)]
        public string? Laboratorio { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Descuento { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? VentasNoSujetas { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? VentasExtensas { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? VentasAfectas { get; set; }
         
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PrecioCosto { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        [ForeignKey("IdProveedor")]
        [InverseProperty("Compras")]
        public virtual Proveedor? IdProveedorNavigation { get; set; } = null!;

    }
}
