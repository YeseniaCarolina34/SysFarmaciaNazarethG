using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Paragraph = iTextSharp.text.Paragraph;
using PdfWriter = iTextSharp.text.pdf.PdfWriter;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace SysFarmaciaNazarethG.Controllers
{
    public class FacturaController : Controller
    {
        private readonly BDContext _context;

        public FacturaController(BDContext context)
        {
            _context = context;
        }

        // GET: Factura
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.Factura.Include(f => f.IdClienteNavigation).Include(f => f.IdVentaNavigation);
            return View(await bDContext.ToListAsync());
        }

        // GET: Factura/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Factura
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.IdVentaNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // GET: Factura/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Factura.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }
            ViewData["IdCliente"] = new SelectList(_context.Cliente, "IdCliente", "IdCliente", factura.IdCliente);
            ViewData["IdVenta"] = new SelectList(_context.Venta, "IdVenta", "IdVenta", factura.IdVenta);
            return View(factura);
        }

        // POST: Factura/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFactura,FacturaNo,Fecha,SubTotal,Iva,Total,IdVenta,IdCliente")] Factura factura)
        {
            if (id != factura.IdFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.IdFactura))
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
            ViewData["IdCliente"] = new SelectList(_context.Cliente, "IdCliente", "IdCliente", factura.IdCliente);
            ViewData["IdVenta"] = new SelectList(_context.Venta, "IdVenta", "IdVenta", factura.IdVenta);
            return View(factura);
        }

        // GET: Factura/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var factura = await _context.Factura
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.IdVentaNavigation)
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // POST: Factura/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var factura = await _context.Factura.FindAsync(id);
            if (factura != null)
            {
                _context.Factura.Remove(factura);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
            return _context.Factura.Any(e => e.IdFactura == id);
        }

        [HttpGet]
        public IActionResult CrearFactura()
        {
            // Obtener la lista de productos desde la base de datos
            var productos = _context.Producto.Select(p => new ProductoViewModel
            {
                Id = p.Id, // Verifica que "Id" es la columna correcta
                Nombre = p.Nombre, // Verifica que "Nombre" es la columna correcta
                //PrecioVenta = p.PrecioVenta // Verifica que "PrecioVenta" es la columna correcta
            }).ToList();

            if (productos == null || !productos.Any())
            {
                // Si no hay productos, mostrar un mensaje
                ModelState.AddModelError("", "No hay productos disponibles en la base de datos.");
            }

            // Crear el modelo y asignar la lista de productos
            var model = new FacturaViewModel
            {
                Producto = productos
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CrearFactura(FacturaViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.Detalles == null || !model.Detalles.Any())
                        {
                            ModelState.AddModelError("", "Debe agregar al menos un producto a la factura.");
                            return View(model);
                        }

                        // Guardar cliente
                        var cliente = new Cliente
                        {
                            Nombre = model.NombreCliente,
                            Direccion = model.DireccionCliente,
                        };
                        _context.Cliente.Add(cliente);
                        _context.SaveChanges();

                        // Calcular totales usando el precio ingresado manualmente
                        var factura = new Factura
                        {
                            IdCliente = cliente.IdCliente,
                            SubTotal = model.Detalles.Sum(d => (d.Cantidad * d.PrecioUnitario) / 1.13m), // Precio sin IVA
                            Iva = model.Detalles.Sum(d => (d.Cantidad * d.PrecioUnitario) - (d.Cantidad * d.PrecioUnitario / 1.13m)), // IVA incluido
                            Total = model.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario) // Precio con IVA
                        };
                        _context.Factura.Add(factura);
                        _context.SaveChanges();

                        // Guardar detalles de la factura, ventas y actualizar inventario
                        foreach (var detalle in model.Detalles)
                        {
                            var producto = _context.Producto.FirstOrDefault(p => p.Id == detalle.IdProducto);

                            // Guardar en DetalleFactura
                            var detalleFactura = new DetalleFactura
                            {
                                IdFactura = factura.IdFactura,
                                Descripcion = producto?.Nombre ?? detalle.Descripcion, // Usar el nombre del producto si está disponible
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario
                            };
                            _context.DetalleFactura.Add(detalleFactura);

                            // Guardar en Venta
                            var venta = new Venta
                            {
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario,
                                PrecioTotal = detalle.Cantidad * detalle.PrecioUnitario
                            };
                            _context.Venta.Add(venta);

                            // Actualizar el inventario del producto
                            var inventario = _context.Inventario.FirstOrDefault(i => i.IdProducto == detalle.IdProducto);

                            if (inventario != null)
                            {
                                inventario.Cantidad -= detalle.Cantidad;

                                if (inventario.Cantidad <= 0)
                                {
                                    // Si se queda sin inventario, marca el producto como inactivo
                                    if (producto != null)
                                    {
                                        producto.Estado = "Inactivo";
                                        _context.Entry(producto).State = EntityState.Modified;
                                    }
                                    // Eliminar inventario si la cantidad es 0
                                    _context.Inventario.Remove(inventario);
                                }
                                else
                                {
                                    // Guardar los cambios en el inventario
                                    _context.Entry(inventario).State = EntityState.Modified;
                                }
                            }
                        }
                        _context.SaveChanges();

                        transaction.Commit();
                        return GenerarFacturaPDF(factura.IdFactura);
                    }
                    catch
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Ocurrió un error al guardar la factura.");
                        return View(model);
                    }
                }
            }

            return View(model);
        }




        private IActionResult GenerarFacturaPDF(int idFactura)
        {
            // Buscar la factura completa con cliente y detalles
            var factura = _context.Factura
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.DetalleFactura)
                .FirstOrDefault(f => f.IdFactura == idFactura);

            if (factura == null)
            {
                return NotFound("Factura no encontrada.");
            }
            factura.SubTotal = factura.DetalleFactura.Sum(d => (d.Cantidad * d.PrecioUnitario) / 1.13m); // Precio sin IVA
            factura.Iva = factura.DetalleFactura.Sum(d => (d.Cantidad * d.PrecioUnitario) - (d.Cantidad * d.PrecioUnitario / 1.13m)); // IVA incluido
            factura.Total = factura.DetalleFactura.Sum(d => d.Cantidad * d.PrecioUnitario); // Precio con IVA

            using (var stream = new MemoryStream())
            {
                // Crear el documento PDF
                var document = new iTextSharp.text.Document();
                PdfWriter.GetInstance(document, stream).CloseStream = false;
                document.Open();

                // Encabezado
                var titulo = new Paragraph("Farmacia Nazareth\n", FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                var Nombre = new Paragraph("OSCAR ANTONIO FRANCO ARIAS\n\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL));
                Nombre.Alignment = Element.ALIGN_CENTER;
                document.Add(Nombre);

                var direccion = new Paragraph("1ª Calle Pte. Av. San Juan, Local 2, Bo. La Trinidad, Nahuizalco, Sonsonate\nGiro: Venta de productos Farmacéuticos y Medicina\n\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL));
                direccion.Alignment = Element.ALIGN_CENTER;
                document.Add(direccion);

                var facturaInfo = new Paragraph($"Factura No: {factura.IdFactura:D5}        Fecha: {DateTime.Now:dd/MM/yyyy}\n\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL));
                facturaInfo.Alignment = Element.ALIGN_RIGHT;
                document.Add(facturaInfo);

                // Información del cliente
                document.Add(new Paragraph($"Cliente: {factura.IdClienteNavigation.Nombre}", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph($"Dirección: {factura.IdClienteNavigation.Direccion}", FontFactory.GetFont("Arial", 10)));
                // Agregar un espacio entre la información del cliente y la tabla
                document.Add(new Paragraph("\n")); // Un salto de línea para separar la tabla
                                                   // También puedes ajustar el espacio con: document.Add(new Paragraph(" ", FontFactory.GetFont("Arial", 6)));

                // Tabla de detalles
                var table = new PdfPTable(4); // 4 columnas: Descripción, Cantidad, Precio Unitario, Total
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 4, 1, 2, 2 });

                // Encabezados de la tabla
                var headerFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD);
                table.AddCell(new PdfPCell(new Phrase("Descripción", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Cantidad", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Precio Unitario", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Total", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                // Detalles de la tabla
                var cellFont = FontFactory.GetFont("Arial", 10);
                foreach (var detalle in factura.DetalleFactura)
                {
                    table.AddCell(new PdfPCell(new Phrase(detalle.Descripcion, cellFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(detalle.Cantidad.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(detalle.PrecioUnitario?.ToString("C") ?? "0.00", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase((detalle.Cantidad * detalle.PrecioUnitario)?.ToString("C") ?? "0.00", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(table);

                // Totales
                document.Add(new Paragraph($"\nSubtotal: {factura.SubTotal:C}", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph($"IVA (13%): {factura.Iva:C}", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph($"Total: {factura.Total:C}", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)));

                document.Close();

                // Preparar el archivo para descarga
                stream.Position = 0;
                var fileName = $"Factura_{factura.IdFactura}.pdf";
                var contentType = "application/pdf";

                return File(stream.ToArray(), contentType, fileName);
            }
        }





        [HttpGet]
        public IActionResult CrearFacturaSinIVA()
        {
            // Obtener la lista de productos desde la base de datos
            var productos = _context.Producto.Select(p => new ProductoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                PrecioVenta = p.PrecioVenta
            }).ToList();

            if (productos == null || !productos.Any())
            {
                // Si no hay productos, mostrar un mensaje
                ModelState.AddModelError("", "No hay productos disponibles en la base de datos.");
            }

            // Crear el modelo y asignar la lista de productos
            var model = new FacturaViewModel
            {
                Producto = productos
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CrearFacturaSinIVA(FacturaViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.Detalles == null || !model.Detalles.Any())
                        {
                            ModelState.AddModelError("", "Debe agregar al menos un producto a la factura.");
                            return View(model);
                        }

                        var cliente = new Cliente
                        {
                            Nombre = model.NombreCliente,
                            Direccion = model.DireccionCliente,
                        };
                        _context.Cliente.Add(cliente);
                        _context.SaveChanges();

                        var factura = new Factura
                        {
                            IdCliente = cliente.IdCliente,
                            SubTotal = model.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario), // Precio sin IVA
                            Iva = 0.00m, // IVA fijo en 0.00
                            Total = model.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario) // Precio sin IVA
                        };
                        _context.Factura.Add(factura);
                        _context.SaveChanges();

                        // Guardar detalles de la factura
                        foreach (var detalle in model.Detalles)
                        {
                            var producto = _context.Producto.FirstOrDefault(p => p.Id == detalle.IdProducto);
                            var detalleFactura = new DetalleFactura
                            {
                                IdFactura = factura.IdFactura,
                                Descripcion = producto?.Nombre ?? detalle.Descripcion, // Usar el nombre del producto si está disponible
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario
                            };
                            _context.DetalleFactura.Add(detalleFactura);

                            // Guardar en Venta
                            var venta = new Venta
                            {
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                PrecioUnitario = detalle.PrecioUnitario,
                                PrecioTotal = detalle.Cantidad * detalle.PrecioUnitario
                            };
                            _context.Venta.Add(venta);
                            // Actualizar el inventario del producto
                            var inventario = _context.Inventario.FirstOrDefault(i => i.IdProducto == detalle.IdProducto);

                            if (inventario != null)
                            {
                                inventario.Cantidad -= detalle.Cantidad;

                                if (inventario.Cantidad <= 0)
                                {
                                    // Si se queda sin inventario, marca el producto como inactivo
                                    if (producto != null)
                                    {
                                        producto.Estado = "Inactivo";
                                        _context.Entry(producto).State = EntityState.Modified;
                                    }
                                    // Eliminar inventario si la cantidad es 0
                                    _context.Inventario.Remove(inventario);
                                }
                                else
                                {
                                    // Guardar los cambios en el inventario
                                    _context.Entry(inventario).State = EntityState.Modified;
                                }
                            }
                        }
                        _context.SaveChanges();

                        transaction.Commit();
                        return GenerarFacturaPDF(factura.IdFactura);
                    }
                    catch
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Ocurrió un error al guardar la factura.");
                        return View(model);
                    }
                } 
            }
            return View(model);
        }

        private IActionResult GenerarFacturaPDFSinIVA(int idFactura)
        {
            // Buscar la factura completa con cliente y detalles
            var factura = _context.Factura
                .Include(f => f.IdClienteNavigation)
                .Include(f => f.DetalleFactura)
                .FirstOrDefault(f => f.IdFactura == idFactura);

            if (factura == null)
            {
                return NotFound("Factura no encontrada.");
            }
            factura.SubTotal = factura.DetalleFactura.Sum(d => d.Cantidad * d.PrecioUnitario); // Precio total sin IVA
            factura.Iva = 0.00m; // IVA fijo en 0.00
            factura.Total = factura.SubTotal; // Total igual al subtotal ya que no hay IVA

            using (var stream = new MemoryStream())
            {
                // Crear el documento PDF
                var document = new iTextSharp.text.Document();
                PdfWriter.GetInstance(document, stream).CloseStream = false;
                document.Open();

                // Encabezado
                var titulo = new Paragraph("Farmacia Nazareth\n", FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                var Nombre = new Paragraph("OSCAR ANTONIO FRANCO ARIAS\n\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL));
                Nombre.Alignment = Element.ALIGN_CENTER;
                document.Add(Nombre);

                var direccion = new Paragraph("1ª Calle Pte. Av. San Juan, Local 2, Bo. La Trinidad, Nahuizalco, Sonsonate\nGiro: Venta de productos Farmacéuticos y Medicina\n\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL));
                direccion.Alignment = Element.ALIGN_CENTER;
                document.Add(direccion);

                var facturaInfo = new Paragraph($"Factura No: {factura.IdFactura:D5}        Fecha: {DateTime.Now:dd/MM/yyyy}\n\n", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL));
                facturaInfo.Alignment = Element.ALIGN_RIGHT;
                document.Add(facturaInfo);

                // Información del cliente
                document.Add(new Paragraph($"Cliente: {factura.IdClienteNavigation.Nombre}", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph($"Dirección: {factura.IdClienteNavigation.Direccion}", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph("\n"));

                // Tabla de detalles
                var table = new PdfPTable(4); // 4 columnas: Descripción, Cantidad, Precio Unitario, Total
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 4, 1, 2, 2 });

                var headerFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD);
                table.AddCell(new PdfPCell(new Phrase("Descripción", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Cantidad", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Precio Unitario", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });
                table.AddCell(new PdfPCell(new Phrase("Total", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = BaseColor.LIGHT_GRAY });

                var cellFont = FontFactory.GetFont("Arial", 10);
                foreach (var detalle in factura.DetalleFactura)
                {
                    table.AddCell(new PdfPCell(new Phrase(detalle.Descripcion, cellFont)) { HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(detalle.Cantidad.ToString(), cellFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase(detalle.PrecioUnitario?.ToString("C") ?? "0.00", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase((detalle.Cantidad * detalle.PrecioUnitario)?.ToString("C") ?? "0.00", cellFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                document.Add(table);

                // Totales
                document.Add(new Paragraph($"\nSubtotal: {factura.SubTotal:C}", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph($"IVA: 0.00", FontFactory.GetFont("Arial", 10)));
                document.Add(new Paragraph($"Total: {factura.Total:C}", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)));

                document.Close();

                stream.Position = 0;
                var fileName = $"FacturaSinIVA_{factura.IdFactura}.pdf"; 
                var contentType = "application/pdf";

                return File(stream.ToArray(), contentType, fileName);
            }
        }


    }
}