using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Controllers
{
    [Authorize(Roles = "Cliente, Administrador")] // Clientes y administradores pueden acceder a las vistas
    public class ProductoController : Controller
    {
        private readonly BDContext _context;

        public ProductoController(BDContext context)
        {
            _context = context;
        }

        // GET: Producto
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.Producto.Include(p => p.IdProveedorNavigation);
            return View(await bDContext.ToListAsync());
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor");
            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripción,IdCategoría,IdProveedor,PrecioCosto,PrecioVenta,CódigoDeBarras,CantidadEnInventario,FechaDeIngreso,FechaDeCaducidad,Estado,Laboratorio")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", producto.IdProveedor);
            return View(producto);
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", producto.IdProveedor);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripción,IdCategoría,IdProveedor,PrecioCosto,PrecioVenta,CódigoDeBarras,CantidadEnInventario,FechaDeIngreso,FechaDeCaducidad,Estado,Laboratorio")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", producto.IdProveedor);
            return View(producto);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.IdProveedorNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                _context.Producto.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.Id == id);
        }



public async Task<IActionResult> ExportarPDF()
    {
        var productos = await _context.Producto.Include(p => p.IdProveedorNavigation).ToListAsync();

        using (MemoryStream ms = new MemoryStream())
        {
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter.GetInstance(pdfDoc, ms);
            pdfDoc.Open();

            pdfDoc.Add(new Paragraph("Lista de Productos"));
            pdfDoc.Add(new Paragraph(" ")); // Espacio entre el título y la tabla

            PdfPTable table = new PdfPTable(5); // Ajusta el número de columnas según los datos que deseas mostrar
            table.AddCell("ID");
            table.AddCell("Nombre");
            table.AddCell("Precio");
            table.AddCell("Cantidad en Inventario");
            table.AddCell("Proveedor");

            foreach (var producto in productos)
            {
                table.AddCell(producto.Id.ToString());
                table.AddCell(producto.Nombre);
                table.AddCell(producto.PrecioVenta.ToString("C")); // Formato de moneda
                table.AddCell(producto.CantidadEnInventario.ToString());
                table.AddCell(producto.IdProveedorNavigation.Nombre); // Asegúrate de que el nombre del proveedor esté incluido
            }

            pdfDoc.Add(table);
            pdfDoc.Close();

            return File(ms.ToArray(), "application/pdf", "Productos.pdf");
        }
    }


}
}
