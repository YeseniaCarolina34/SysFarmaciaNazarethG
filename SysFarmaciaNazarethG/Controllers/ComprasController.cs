using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Controllers
{
    public class ComprasController : Controller
    {
        private readonly BDContext _context;

        public ComprasController(BDContext context)
        {
            _context = context;
        }

        // GET: Compras
        public async Task<IActionResult> Index()
        {
            return View(await _context.Compras.Include(p => p.IdProveedorNavigation).ToListAsync());
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compras == null)
            {
                return NotFound();
            }

            return View(compras);
        }

        // GET: Compras/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}
        public IActionResult Create()
        {
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre");
            return View();
        }

        // POST: Compras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create([Bind("Id,Codigo,Descripcion,Cantidad,Bonificacion,PrecioUnitario,Laboratorio,Descuento,VentasNoSujetas,VentasExtensas,VentasAfectas,IdProveedor")] Compras compras)
        //{
        // if (ModelState.IsValid)
        //{
        // _context.Add(compras);
        // await _context.SaveChangesAsync();
        // return RedirectToAction(nameof(Index));
        //}
        // ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", compras.IdProveedor);

        // return View(compras);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Codigo,Descripcion,Cantidad,Bonificacion,PrecioUnitario,PrecioCosto,Laboratorio,Descuento,VentasNoSujetas,VentasExtensas,VentasAfectas,IdProveedor")] Compras compras)
        {
            if (ModelState.IsValid)
            {
                // Agregar la compra al contexto
                _context.Add(compras);
                // Recuperar el proveedor asociado a la compra
                var proveedor = await _context.Proveedor.FindAsync(compras.IdProveedor);
                if (proveedor == null)
                {
                    ModelState.AddModelError("IdProveedor", "El proveedor seleccionado no existe.");
                    ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", compras.IdProveedor);
                    return View(compras);
                }


                // Crear un nuevo producto asociado a la compra
                var nuevoProducto = new Producto
                {
                    Nombre = compras.Nombre,// Asignar el nombre de la compra al producto

                    CódigoDeBarras = compras.Codigo,
                    Descripción = compras.Descripcion,
                    CantidadEnInventario = compras.Cantidad,
                    Laboratorio = compras.Laboratorio,
                    PrecioCosto = compras.PrecioCosto,
                    PrecioVenta = compras.PrecioUnitario,
                    FechaDeIngreso = DateOnly.FromDateTime(DateTime.Now),
                    IdProveedor = proveedor.IdProveedor, // Asignar el ID del proveedor
                    IdProveedorNavigation = proveedor

                };

                // Agregar el nuevo producto a la base de datos
                _context.Producto.Add(nuevoProducto);
                await _context.SaveChangesAsync(); // Guardar para obtener el ID del nuevo producto

                // Registrar en Inventario
                var nuevoInventario = new Inventario
                {
                    IdProducto = nuevoProducto.Id,
                    Cantidad = compras.Cantidad,
                    Ubicación = "Almacén Principal", // Cambiar según la lógica de ubicación
                    FechaDeIngreso = DateOnly.FromDateTime(DateTime.Now),
                    Estado = "Activo" // Cambiar según la lógica
                };
                _context.Inventario.Add(nuevoInventario);

                // Guardar todos los cambios en la base de datos
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si algo falla, recargar los datos necesarios
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", compras.IdProveedor);
            return View(compras);
        }


        // GET: Compras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras.FindAsync(id);
            if (compras == null)
            {
                return NotFound();
            }
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", compras.IdProveedor);
            return View(compras);
        }

        // POST: Compras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Codigo,Descripcion,Cantidad,Bonificacion,PrecioUnitario,Laboratorio,Descuento,VentasNoSujetas,VentasExtensas,VentasAfectas,IdProveedor")] Compras compras)
        {
            if (id != compras.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compras);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComprasExists(compras.Id))
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
            ViewData["IdProveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", compras.IdProveedor);

            return View(compras);
        }

        // GET: Compras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compras = await _context.Compras
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compras == null)
            {
                return NotFound();
            }

            return View(compras);
        }

        // POST: Compras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compras = await _context.Compras.FindAsync(id);
            if (compras != null)
            {
                _context.Compras.Remove(compras);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComprasExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id);
        }
        public IActionResult GenerarPDF()
        {
            var compras = _context.Compras.Include(p => p.IdProveedorNavigation).ToList();

            // Configuración inicial del documento
            MemoryStream workStream = new MemoryStream();
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter.GetInstance(document, workStream).CloseStream = false;

            document.Open();

            // Título del documento
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
            document.Add(new Paragraph("Reporte de Compras", titleFont));
            document.Add(new Paragraph(" ")); // Espacio

            // Crear una tabla
            PdfPTable table = new PdfPTable(5); // Cambia según tus columnas
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.5f, 2f, 1.5f, 1f, 2f }); // Ajusta proporciones

            // Encabezados de la tabla con fondo celeste
            BaseColor headerBackgroundColor = new BaseColor(173, 216, 230); // Color celeste

            PdfPCell headerCell = new PdfPCell(new Phrase("Código", headerFont))
            {
                BackgroundColor = headerBackgroundColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(headerCell);

            headerCell = new PdfPCell(new Phrase("Descripción", headerFont))
            {
                BackgroundColor = headerBackgroundColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(headerCell);

            headerCell = new PdfPCell(new Phrase("Cantidad", headerFont))
            {
                BackgroundColor = headerBackgroundColor,
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 5
            };
            table.AddCell(headerCell);

            headerCell = new PdfPCell(new Phrase("Precio Unitario", headerFont))
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
            foreach (var compra in compras)
            {
                PdfPCell cell = new PdfPCell(new Phrase(compra.Codigo, normalFont))
                {
                    BackgroundColor = BaseColor.WHITE,
                    Padding = 5
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(compra.Descripcion, normalFont))
                {
                    BackgroundColor = BaseColor.WHITE,
                    Padding = 5
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(compra.Cantidad.ToString(), normalFont))
                {
                    BackgroundColor = BaseColor.WHITE,
                    Padding = 5
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(compra.PrecioUnitario.ToString("C"), normalFont))
                {
                    BackgroundColor = BaseColor.WHITE,
                    Padding = 5
                };
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(compra.IdProveedorNavigation?.Nombre ?? "N/A", normalFont))
                {
                    BackgroundColor = BaseColor.WHITE,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            document.Add(table);
            document.Close();

            // Devolver el PDF
            workStream.Position = 0;
            return File(workStream, "application/pdf", "Reporte_Compras.pdf");
        }
    }
}
