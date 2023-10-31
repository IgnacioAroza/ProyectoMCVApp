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
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Agregar cantidades de Stock
        public IActionResult AgregarStock(int stockId, int cantAgregada)
        {
            var stock = _context.stocks.Find(stockId);
            if(stock != null)
            {
                stock.cantidad += cantAgregada;
                _context.SaveChanges();
                return RedirectToAction("DetallesStock", new { stockId = stockId });
            }
            return NotFound();
        }

        //Vender productos
        public IActionResult Vender(int stockId, int cantVendida)
        {
            var stock = _context.stocks.Find(stockId);
            if(stock != null && stock.cantidad >= cantVendida)
            {
                stock.cantidad -= cantVendida;
                _context.SaveChanges();
                return RedirectToAction("DetallesStock", new { stockId = stockId });
            }
            return NotFound();
        }

        // GET: Stocks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.stocks.Include(s => s.producto);
            return View(await applicationDbContext.ToListAsync());
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
