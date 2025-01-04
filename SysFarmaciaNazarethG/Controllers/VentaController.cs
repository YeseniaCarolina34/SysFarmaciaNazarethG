using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;

namespace SysFarmaciaNazarethG.Controllers
{
    public class VentaController : Controller
    {
        private readonly BDContext _context;

        public VentaController(BDContext context)
        {
            _context = context;
        }

        // GET: Venta
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.Venta.Include(v => v.IdProductoNavigation);
            return View(await bDContext.ToListAsync());
        }

        // GET: Venta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(v => v.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Venta/Create
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVenta,Cantidad,PrecioUnitario,PrecioTotal,EnviosDeSucursal,IdProducto")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                // Agregar la venta al contexto
                _context.Add(venta);

                // Obtener el inventario del producto
                var inventario = await _context.Inventario
                    .FirstOrDefaultAsync(i => i.IdProducto == venta.IdProducto);

                // Obtener el producto
                var producto = await _context.Producto
                    .FirstOrDefaultAsync(p => p.Id == venta.IdProducto);

                // Obtener la descripción del producto
                string descripcionProducto = producto?.Descripción ?? "Sin descripción";

                // Puedes usar la descripción en algún lugar, como para un log o devolverlo a la vista
                Console.WriteLine("Descripción del Producto: " + descripcionProducto);

                if (inventario != null)
                {
                    inventario.Cantidad -= venta.Cantidad;

                    if (inventario.Cantidad <= 0)
                    {
                        // Actualizar el estado del producto si el inventario llega a 0
                        if (producto != null)
                        {
                            producto.Estado = "Inactivo"; // Actualizar el estado
                        }

                        // Eliminar el inventario si la cantidad es 0
                        _context.Inventario.Remove(inventario);
                    }
                }

                // Si el producto se encontró pero no tiene inventario, actualizar su estado
                if (producto != null && (inventario == null || inventario.Cantidad <= 0))
                {
                    producto.Estado = "Inactivo";
                    _context.Entry(producto).State = EntityState.Modified;
                }

                // Guardar todos los cambios en la base de datos
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, recargar la vista
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Nombre", venta.IdProducto);
            return View(venta);
        }
        // GET: Venta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Nombre", venta.IdProducto);
            return View(venta);
        }
        [HttpGet]
        public async Task<IActionResult> GetDescripcionProducto(int idProducto)
        {
            var producto = await _context.Producto.FirstOrDefaultAsync(p => p.Id == idProducto);

            if (producto != null)
            {
                return Json(new { descripcion = producto.Descripción });
            }

            return Json(new { descripcion = "Descripción no disponible" });
        }


        // POST: Venta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVenta,Cantidad,PrecioUnitario,PrecioTotal,EnviosDeSucursal,IdProducto")] Venta venta)
        {
            if (id != venta.IdVenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.IdVenta))
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
            ViewData["IdProducto"] = new SelectList(_context.Producto, "Id", "Id", venta.IdProducto);
            return View(venta);
        }

        // GET: Venta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Venta
                .Include(v => v.IdProductoNavigation)
                .FirstOrDefaultAsync(m => m.IdVenta == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Venta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta != null)
            {
                _context.Venta.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Venta.Any(e => e.IdVenta == id);
        }
    }
}
