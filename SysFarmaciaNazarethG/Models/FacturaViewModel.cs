
namespace SysFarmaciaNazarethG.Models;
public class FacturaViewModel
{
    public string? NombreCliente { get; set; }
    public string? DireccionCliente { get; set; }
    // Lista de detalles de factura
    public List<DetalleFacturaViewModel> Detalles { get; set; } = new List<DetalleFacturaViewModel>();

    // Nueva propiedad para los productos
    public List<ProductoViewModel> Producto { get; set; } = new List<ProductoViewModel>();
    public decimal Subtotal { get; set; }
    public decimal IVA { get; set; }
    public decimal Total { get; set; }
    // Nueva propiedad para asociar con la venta
    public int IdVenta { get; set; }
}

public class DetalleFacturaViewModel
{
    public int IdProducto { get; set; }
    public string? Descripcion { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}



public class ProductoViewModel
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public decimal PrecioVenta { get; set; }
}
