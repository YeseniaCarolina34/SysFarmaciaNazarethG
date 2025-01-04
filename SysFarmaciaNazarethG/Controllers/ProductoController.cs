using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Controllers
{
   
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
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre");
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
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.IdProveedor);
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
            // Obtener la lista de productos con su proveedor
            var productos = await _context.Producto.Include(p => p.IdProveedorNavigation).ToListAsync();

            // Validar si hay productos
            if (productos == null || !productos.Any())
            {
                return BadRequest("No hay productos disponibles para generar el PDF.");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
                PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();

                // Título del documento
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                pdfDoc.Add(new Paragraph("Lista de Productos", titleFont));
                pdfDoc.Add(new Paragraph(" ")); // Espacio entre el título y la tabla

                // Crear una tabla
                PdfPTable table = new PdfPTable(5); // Ajusta el número de columnas según los datos
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1f, 2f, 1f, 1.5f, 2f }); // Ajusta proporciones según los datos

                // Encabezados de la tabla con fondo celeste
                BaseColor headerBackgroundColor = new BaseColor(173, 216, 230); // Color celeste

                PdfPCell headerCell = new PdfPCell(new Phrase("ID", headerFont))
                {
                    BackgroundColor = headerBackgroundColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Nombre", headerFont))
                {
                    BackgroundColor = headerBackgroundColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Precio", headerFont))
                {
                    BackgroundColor = headerBackgroundColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Cantidad en Inventario", headerFont))
                {
                    BackgroundColor = headerBackgroundColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Proveedor", headerFont))
                {
                    BackgroundColor = headerBackgroundColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(headerCell);

                // Filas de datos con fondo blanco
                foreach (var producto in productos)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(producto.Id.ToString(), normalFont))
                    {
                        BackgroundColor = BaseColor.WHITE,
                        Padding = 5
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(producto.Nombre, normalFont))
                    {
                        BackgroundColor = BaseColor.WHITE,
                        Padding = 5
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(producto.PrecioVenta.ToString("C"), normalFont))
                    {
                        BackgroundColor = BaseColor.WHITE,
                        Padding = 5
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(producto.CantidadEnInventario.ToString(), normalFont))
                    {
                        BackgroundColor = BaseColor.WHITE,
                        Padding = 5
                    };
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(producto.IdProveedorNavigation?.Nombre ?? "Sin proveedor", normalFont))
                    {
                        BackgroundColor = BaseColor.WHITE,
                        Padding = 5
                    };
                    table.AddCell(cell);
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                return File(ms.ToArray(), "application/pdf", "Productos.pdf");
            }
        }




    }

}
