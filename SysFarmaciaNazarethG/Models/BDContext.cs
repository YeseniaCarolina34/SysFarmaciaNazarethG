using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__A3C02A1076A2A545");

            entity.HasOne(d => d.IdInventarioNavigation).WithMany(p => p.Categoria).HasConstraintName("FK__Categoria__IdInv__31EC6D26");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PK__Factura__50E7BAF1503E48EF");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.Factura).HasConstraintName("FK__Factura__IdVenta__37A5467C");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario).HasName("PK__Inventar__1927B20C0F16D0A7");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Inventario).HasConstraintName("FK__Inventari__IdPro__2F10007B");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Producto__3214EC07418DED5E");

            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Producto).HasConstraintName("FK__Producto__IdProv__2C3393D0");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.IdProveedor).HasName("PK__Proveedo__E8B631AF6860B538");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Proveedor).HasConstraintName("FK__Proveedor__IdUsu__29572725");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rol__3214EC07175CF79F");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC07040BAF9E");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuario).HasConstraintName("FK__Usuario__IdRol__267ABA7A");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Venta__BC1240BDCE6C4F64");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Venta).HasConstraintName("FK__Venta__IdProduct__34C8D9D1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
