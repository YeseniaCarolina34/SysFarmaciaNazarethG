﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SysFarmaciaNazarethG.Models;
using Microsoft.AspNetCore.Authorization;

namespace SysFarmaciaNazarethG.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo los administradores pueden acceder a estas acciones

    public class UsuarioController : Controller
    {
        private readonly BDContext _context;

        public UsuarioController(BDContext context)
        {
            _context = context;
        }


        // GET: Usuario/Login

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        // POST: Usuario/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (ModelState.IsValid)
            {
                // Encriptar la contraseña ingresada con MD5
                string contraseñaEncriptada = EncriptarMD5(password);

                // Verificar si el usuario existe (compara el Login y Password)
                var usuario = _context.Usuario
                                      .Include(u => u.IdRolNavigation) // Incluimos el rol del usuario en la consulta
                                      .FirstOrDefault(u => u.Login == login && u.Password == contraseñaEncriptada);

                if (usuario != null)
                {
                    // Autenticación exitosa, crear claims de autenticación
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Login),
                // Agregar el rol del usuario en los claims
                new Claim(ClaimTypes.Role, usuario.IdRolNavigation.Nombre) // Asignamos el rol
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Para mantener la sesión iniciada si se cierra el navegador
                    };

                    // Iniciar sesión (autenticación)
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirigir según el rol (opcional)
                    if (usuario.IdRolNavigation.Nombre == "Administrador")
                    {
                        return RedirectToAction("Index", "Home"); // Redirigir al dashboard de admin
                    }
                    else if (usuario.IdRolNavigation.Nombre == "Cliente")
                    {
                        return RedirectToAction("Index", "Home"); // Redirigir a la página principal para clientes
                    }
                }
                else
                {
                    // Si las credenciales son incorrectas, mostrar error
                    ViewBag.Error = "Credenciales incorrectas";
                }
            }

            // Si el modelo no es válido o el usuario no existe, mostrar de nuevo la vista de login
            return View();
        }

        // Método para encriptar contraseñas con MD5
        private string EncriptarMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convertir el hash en una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        // GET: Usuario/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuario");
        }

        // Cerrar sesión
        public async Task<IActionResult> Logoutt()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login"); // Redirigir al usuario a la vista de inicio de sesión
        }

        [AllowAnonymous]

        // GET: Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }
        // POST: Registro
        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            ModelState.Remove("IdRolNavigation");
            if (ModelState.IsValid)
            {
                // Asignar el rol automáticamente (IdRol 2 para "Cliente")
                usuario.IdRol = 2; // Asegúrate de que 2 sea el Id correspondiente a "Cliente"
                usuario.Estatus = "Activo"; // Asignamos un estatus activo
                usuario.FechaRegistro = DateTime.Now; // Registrar la fecha de creación

                // Encriptar contraseña con MD5
                usuario.Password = EncriptarMD5(usuario.Password);

                // Agregar el usuario a la base de datos
                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();

                // Redirigir después de registro exitoso
                return RedirectToAction("Index", "Home");
            }

            // Si el modelo no es válido, vuelve a mostrar el formulario con errores
            return View(usuario);
        }




        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var bDContext = _context.Usuario.Include(u => u.IdRolNavigation);
            return View(await bDContext.ToListAsync());
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            ViewData["IdRol"] = new SelectList(_context.Rol, "Id", "Id");
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Login,Password,Estatus,FechaRegistro,IdRol")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Password = EncriptarMD5(usuario.Password);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRol"] = new SelectList(_context.Rol, "Id", "Id", usuario.IdRol);
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["IdRol"] = new SelectList(_context.Rol, "Id", "Id", usuario.IdRol);
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Login,Password,Estatus,FechaRegistro,IdRol")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            ViewData["IdRol"] = new SelectList(_context.Rol, "Id", "Id", usuario.IdRol);
            return View(usuario);
        }

        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }

    }
}
