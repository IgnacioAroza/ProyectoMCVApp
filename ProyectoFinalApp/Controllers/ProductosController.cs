using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProyectoFinalApp.Data;
using ProyectoFinalApp.Models;
using ProyectoFinalApp.ViewModel;

namespace ProyectoFinalApp.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> ImportarProductos(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Mensaje = "Error: No se ha podido proporcionado un archivo de Excel";
                return View();
            }
            try
            {
                using (var package = new ExcelPackage(archivo.OpenReadStream()))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    var productos = new List<Producto>();

                    for (int row = worksheet.Dimension.Start.Row; row <= worksheet.Dimension.End.Row; row++)
                    {
                        if (int.TryParse(worksheet.Cells[row, 2].Value.ToString(), out int codigo))
                        {
                            var producto = new Producto
                            {
                                nombre = worksheet.Cells[row, 1].Value.ToString(),
                                codigo = codigo,
                                descripcion = worksheet.Cells[row, 3].Value.ToString(),
                                imagen = worksheet.Cells[row, 4].Value.ToString(),
                                categoriaId = Convert.ToInt32(worksheet.Cells[row, 5].Value)
                            };
                            productos.Add(producto);
                        }
                    }
                    _context.productos.AddRange(productos);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: La importación ha fallado. Verifica el formato del archivo o consulta los registros del servidor para obtener más detalles.";
                Console.WriteLine("Error en la importación: " + ex.Message);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Excepción interna: " + ex.InnerException.Message);
                }
            }
            var applicationDbContext = _context.productos.Include(a => a.categoria);
            return RedirectToAction("Index", await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        // GET: Productos
        public IActionResult Index(string? busqNombre, int? busqCodigo, int? categoriaId, int pagina = 1)
        {
            Paginador paginas = new Paginador();
            paginas.PaginaActual = pagina;
            paginas.RegistrosPorPagina = 3;

            var applicationDbContext = _context.productos.Include(p => p.categoria).Select(e => e);
            if (!string.IsNullOrEmpty(busqNombre))
            {
                applicationDbContext = applicationDbContext.Where(e => e.nombre.Contains(busqNombre));
                paginas.ValoresQueryString.Add("busquedaNombre", busqNombre);
            }
            if(busqCodigo != null && busqCodigo > 0)
            {
                applicationDbContext = applicationDbContext.Where(e => e.codigo.Equals(busqCodigo));
                paginas.ValoresQueryString.Add("busquedaCodigo", busqCodigo.ToString());
            }
            if(categoriaId != null && categoriaId > 0)
            {
                applicationDbContext = applicationDbContext.Where(e => e.categoriaId.Equals(categoriaId));
                paginas.ValoresQueryString.Add("categoriaId", categoriaId.ToString());
            }
            paginas.TotalRegistros = applicationDbContext.Count();

            var mostrarRegistros = applicationDbContext
                .Skip((pagina - 1) * paginas.RegistrosPorPagina)
                .Take(paginas.RegistrosPorPagina);

            ProductoVM datos = new ProductoVM()
            {
                productos = mostrarRegistros.ToList(),
                busquedaNombre = busqNombre,
                busquedaCodigo = busqCodigo,
                listaCategorias = new SelectList(_context.categorias, "Id", "descripcion"),
                categoriaId = categoriaId,
                paginador = paginas
            };

            return View(datos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.productos == null)
            {
                return NotFound();
            }

            var producto = await _context.productos
                .Include(p => p.categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["categoriaId"] = new SelectList(_context.categorias, "Id", "descripcion");
            ViewData["stockId"] = new SelectList(_context.stocks, "Id", "cantidad");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,codigo,descripcion,imagen,categoriaId,stockId")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.imagen = cargarFoto("");

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["categoriaId"] = new SelectList(_context.categorias, "Id", "Id", producto.categoriaId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.productos == null)
            {
                return NotFound();
            }

            var producto = await _context.productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["categoriaId"] = new SelectList(_context.categorias, "Id", "descripcion", producto.categoriaId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre,codigo,descripcion,imagen,categoriaId, stockId")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string nuevaFoto = cargarFoto(string.IsNullOrEmpty(producto.imagen) ? "" : producto.imagen);

                    if(!string.IsNullOrEmpty(nuevaFoto))
                        producto.imagen = nuevaFoto;

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
            ViewData["categoriaId"] = new SelectList(_context.categorias, "Id", "Id", producto.categoriaId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.productos == null)
            {
                return NotFound();
            }

            var producto = await _context.productos
                .Include(p => p.categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.productos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.productos'  is null.");
            }
            var producto = await _context.productos.FindAsync(id);
            if (producto != null)
            {
                _context.productos.Remove(producto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
          return (_context.productos?.Any(e => e.Id == id)).GetValueOrDefault();
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
