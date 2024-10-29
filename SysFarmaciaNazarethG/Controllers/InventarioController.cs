using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace SysFarmaciaNazarethG.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo los administradores pueden acceder a estas acciones
    public class InventarioController : Controller
    {
        private readonly BDContext _context;

        public InventarioController(BDContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.Inventario.Include(i => i.IdProductoNavigation);
            return View(await bDContext.ToListAsync());
        }

        // GET: Inventario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // GET: Inventario/Create
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Id");
            return View();
        }

        // POST: Inventario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdInventario,IdProducto,Cantidad,Ubicación,FechaDeIngreso,Estado")] Inventario inventario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Id", inventario.IdProducto);
            return View(inventario);
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Id", inventario.IdProducto);
            return View(inventario);
        }

        // POST: Inventario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdInventario,IdProducto,Cantidad,Ubicación,FechaDeIngreso,Estado")] Inventario inventario)
        {
            if (id != inventario.IdInventario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventarioExists(inventario.IdInventario))
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
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Id", inventario.IdProducto);
            return View(inventario);
        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventario = await _context.Inventario
                .Include(i => i.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdInventario == id);
            if (inventario == null)
            {
                return NotFound();
            }

            return View(inventario);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventario = await _context.Inventario.FindAsync(id);
            if (inventario != null)
            {
                _context.Inventario.Remove(inventario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       public async Task<IActionResult> ExportarPDF()
       {
        var inventarios = await _context.Inventario.Include(i => i.IdProductoNavigation).ToListAsync();

        using (MemoryStream ms = new MemoryStream())
        {
            Document pdfDoc = new Document(PageSize.A4, 10, 10, 10, 10);
            PdfWriter.GetInstance(pdfDoc, ms);
            pdfDoc.Open();

            pdfDoc.Add(new Paragraph("Lista de Inventario"));
            pdfDoc.Add(new Paragraph(" ")); // Espacio entre el título y la tabla

            PdfPTable table = new PdfPTable(5); // Ajusta el número de columnas según los datos que deseas mostrar
            table.AddCell("ID Inventario");
            table.AddCell("ID Producto");
            table.AddCell("Cantidad");
            table.AddCell("Ubicación");
            table.AddCell("Fecha de Ingreso");

            foreach (var inventario in inventarios)
            {
                table.AddCell(inventario.IdInventario.ToString());
                table.AddCell(inventario.IdProducto.ToString());
                table.AddCell(inventario.Cantidad.ToString());
                table.AddCell(inventario.Ubicación);
                table.AddCell(inventario.FechaDeIngreso?.ToString("MM/dd/yyyy") ?? "");

            }

                pdfDoc.Add(table);
            pdfDoc.Close();

            return File(ms.ToArray(), "application/pdf", "Inventario.pdf");
        }
    }
    private bool InventarioExists(int id)
        {
            return _context.Inventario.Any(e => e.IdInventario == id);
        }

    }
}
