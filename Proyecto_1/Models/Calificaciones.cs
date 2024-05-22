using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_1.Models
{
    public class Calificaciones
    {
        public int IdCriticoCalificacion { get; set; }
        public int IdCritico { get; set; }
        public int IdPelicula { get; set; }
        public int Calificacion { get; set; }
    }
}