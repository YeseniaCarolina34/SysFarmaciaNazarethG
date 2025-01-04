using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Models;

public partial class BDContext : DbContext
{
    public BDContext()
    {
    }

    public BDContext(DbContextOptions<BDContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Cliente> Cliente { get; set; }

    public virtual DbSet<Compras> Compras { get; set; }

    public virtual DbSet<DetalleFactura> DetalleFactura { get; set; }

    public virtual DbSet<Factura> Factura { get; set; }

    public virtual DbSet<Inventario> Inventario { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<Proveedor> Proveedor { get; set; }

    public virtual DbSet<Rol> Rol { get; set; }

    public virtual DbSet<Usuario> Usuario { get; set; }

    public virtual DbSet<Venta> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK_Categori_A3C02A107418E2B6");

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.Categoria).HasConstraintName("FK_CategoriaIdInv_31EC6D26");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK_Cliente_D59466421CD45162");
        });

        modelBuilder.Entity<Compras>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Compras_3214EC0738E8DD2F");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Compras)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ComprasIdProve_412EB0B6");
        });

        modelBuilder.Entity<DetalleFactura>(entity =>
        {
            entity.HasKey(e => e.IdDetalleFactura).HasName("PK_DetalleF_DB5F4631733400EB");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.DetalleFactura).HasConstraintName("FK_DetalleFaIdFac_3E52440B");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleFactura).HasConstraintName("FK_DetalleFaIdPro_3D5E1FD2");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PK_Factura_50E7BAF171A7691B");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Factura).HasConstraintName("FK_FacturaIdClien_3A81B327");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.Factura).HasConstraintName("FK_FacturaIdVenta_398D8EEE");
            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Factura).HasConstraintName("FK_FacturaIdPProductoo_398D8AEE");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("PK_Inventar_1927B20CF8FA27C6");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Inventario).HasConstraintName("FK_InventariIdPro_2F10007B");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Producto_3214EC078FF34AF0");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Producto).HasConstraintName("FK_ProductoIdProv_2C3393D0");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK_Proveedo_E8B631AFD44131CC");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Proveedor).HasConstraintName("FK_ProveedorIdUsu_29572725");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Rol_3214EC07E30F0897");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Usuario_3214EC070BB6D92F");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario).HasConstraintName("FK_UsuarioIdRol_267ABA7A");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK_Venta_BC1240BDFC805110");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Venta).HasConstraintName("FK_VentaIdProduct_34C8D9D1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}