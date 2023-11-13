using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalApp.Data;
using ProyectoFinalApp.Models;
using ProyectoFinalApp.ViewModel;

namespace ProyectoFinalApp.Controllers
{
    [Authorize]
    public class CategoriasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Categorias
        public IActionResult Index(int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.categorias.Select(c => c);

            paginas.TotalRegistros = applicationDbContext.Count();

            var mostrarRegistros = applicationDbContext
                .Skip((pagina - 1) * paginas.RegistrosPorPagina)
                .Take(paginas.RegistrosPorPagina);

            CategoriaVM datos = new CategoriaVM()
            {
                categorias = mostrarRegistros.ToList(),
                paginador = paginas
            };
            return View(datos);
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.categorias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,descripcion")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,descripcion")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.categorias == null)
            {
                return NotFound();
            }

            var categoria = await _context.categorias
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.categorias == null)
            {
                return Problem("Entity set 'ApplicationDbContext.categorias'  is null.");
            }
            var categoria = await _context.categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.categorias.Remove(categoria);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
          return (_context.categorias?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
