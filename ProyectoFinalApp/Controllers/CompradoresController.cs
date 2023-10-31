using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalApp.Data;
using ProyectoFinalApp.Models;

namespace ProyectoFinalApp.Controllers
{
    public class CompradoresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CompradoresController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Compradores
        public async Task<IActionResult> Index()
        {
              return _context.compradores != null ? 
                          View(await _context.compradores.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.compradores'  is null.");
        }

        // GET: Compradores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.compradores == null)
            {
                return NotFound();
            }

            var comprador = await _context.compradores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comprador == null)
            {
                return NotFound();
            }

            return View(comprador);
        }

        // GET: Compradores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Compradores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,apellido,dni,foto")] Comprador comprador)
        {
            if (ModelState.IsValid)
            {
                comprador.foto = cargarFoto("");

                _context.Add(comprador);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comprador);
        }

        // GET: Compradores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.compradores == null)
            {
                return NotFound();
            }

            var comprador = await _context.compradores.FindAsync(id);
            if (comprador == null)
            {
                return NotFound();
            }
            return View(comprador);
        }

        // POST: Compradores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre,apellido,dni,foto")] Comprador comprador)
        {
            if (id != comprador.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string nuevaFoto = cargarFoto(string.IsNullOrEmpty(comprador.foto) ? "" : comprador.foto);

                    if(!string.IsNullOrEmpty(nuevaFoto))
                        comprador.foto = nuevaFoto;

                    _context.Update(comprador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompradorExists(comprador.Id))
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
            return View(comprador);
        }

        // GET: Compradores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.compradores == null)
            {
                return NotFound();
            }

            var comprador = await _context.compradores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comprador == null)
            {
                return NotFound();
            }

            return View(comprador);
        }

        // POST: Compradores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.compradores == null)
            {
                return Problem("Entity set 'ApplicationDbContext.compradores'  is null.");
            }
            var comprador = await _context.compradores.FindAsync(id);
            if (comprador != null)
            {
                _context.compradores.Remove(comprador);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompradorExists(int id)
        {
          return (_context.compradores?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string cargarFoto(string fotoAnterior)
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivoFoto = archivos[0];
                if (archivoFoto.Length > 0)
                {
                    var pathDestino = Path.Combine(_env.WebRootPath, "fotos");

                    fotoAnterior = Path.Combine(pathDestino, fotoAnterior);
                    if (System.IO.File.Exists(fotoAnterior))
                        System.IO.File.Delete(fotoAnterior);

                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                    archivoDestino += Path.GetExtension(archivoFoto.FileName);

                    using (var filestream = new FileStream(Path.Combine(pathDestino, archivoDestino), FileMode.Create))
                    {
                        archivoFoto.CopyTo(filestream);
                        return archivoDestino;
                    };
                }
            }
            return "";
        }
    }
}
