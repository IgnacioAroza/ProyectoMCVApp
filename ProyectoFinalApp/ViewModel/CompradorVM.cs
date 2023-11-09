using Microsoft.Identity.Client;
using ProyectoFinalApp.Models;

namespace ProyectoFinalApp.ViewModel
{
    public class CompradorVM
    {
        public List<Comprador> compradores {  get; set; }
        public string busquedaNombre { get; set; }
        public string busquedaApellido { get; set; }
        public Paginador paginador { get; set; }
    }
}
