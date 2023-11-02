using Microsoft.AspNetCore.Mvc;
using ProyectoFinalApp.Data;
using ProyectoFinalApp.Models;
using System.Diagnostics;

namespace ProyectoFinalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.productos.ToList();
            foreach(var item in productos)
            {
                item.stocks = _context.stocks.Where(x => x.productoId == item.Id).ToList();
            }
            return View(productos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}