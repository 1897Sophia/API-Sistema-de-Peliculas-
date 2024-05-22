using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_1.Models
{
    public class Peliculas
    {
        public int IdPelicula { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Poster { get; set; } 
        public DateTime Fecha { get; set; }

  
    }
}