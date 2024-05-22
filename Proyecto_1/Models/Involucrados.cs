using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_1.Models
{
    public class Involucrados
    {
        public int IdInvolucrado { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Twittwer { get; set; }
        public string Otros { get; set; }
        public int OP { get; set; }
    }
}