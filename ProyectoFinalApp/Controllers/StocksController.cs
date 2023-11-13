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
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Stocks
        public IActionResult Index(int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 5;

            var applicationDbContext = _context.stocks.Select(s => s);

            paginas.TotalRegistros = applicationDbContext.Count();

            var mostarRegistros = applicationDbContext
                .Skip((pagina - 1) * paginas.RegistrosPorPagina)
                .Take(paginas.RegistrosPorPagina);

            StockVM datos = new StockVM()
            {
                stocks = mostarRegistros.ToList(),
                paginador = paginas
            };
            return View(datos);
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.stocks == null)
            {
                return NotFound();
            }

            var stock = await _context.stocks
                .Include(s => s.producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewData["productoId"] = new SelectList(_context.productos, "Id", "nombre");
            return View();
        }

        // POST: Stocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,cantidad,fecha,productoId")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["productoId"] = new SelectList(_context.productos, "Id", "Id", stock.productoId);
            return View(stock);
        }


        public IActionResult Venta()
        {
            ViewData["productoId"] = new SelectList(_context.productos, "Id", "nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Venta([Bind("Id,cantidad,fecha,productoId")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                stock.cantidad *= -1;
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["productoId"] = new SelectList(_context.productos, "Id", "Id", stock.productoId);
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.stocks == null)
            {
                return NotFound();
            }

            var stock = await _context.stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            ViewData["productoId"] = new SelectList(_context.productos, "Id", "Id", stock.productoId);
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,cantidad,fecha,productoId")] Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.Id))
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
            ViewData["productoId"] = new SelectList(_context.productos, "Id", "Id", stock.productoId);
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.stocks == null)
            {
                return NotFound();
            }

            var stock = await _context.stocks
                .Include(s => s.producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stock == null)
            {
                return NotFound();
            }

            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.stocks == null)
            {
                return Problem("Entity set 'ApplicationDbContext.stocks'  is null.");
            }
            var stock = await _context.stocks.FindAsync(id);
            if (stock != null)
            {
                _context.stocks.Remove(stock);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
          return (_context.stocks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
