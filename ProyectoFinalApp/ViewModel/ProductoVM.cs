using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoFinalApp.Models;

namespace ProyectoFinalApp.ViewModel
{
    public class ProductoVM
    {
        public List<Producto> productos { get; set; }
        public SelectList listaCategorias { get; set; }
        public string busquedaNombre { get; set; }
        public int? busquedaCodigo { get; set; }
        public int? categoriaId { get; set; }
        public Paginador paginador { get; set; }
    }
}
