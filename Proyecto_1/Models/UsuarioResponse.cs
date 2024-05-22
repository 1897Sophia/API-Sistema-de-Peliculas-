using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto_1.Models
{
    public class UsuarioResponse
    {
        public int Operacion { get; set; }
        public int IdUsuario { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public int Estado { get; set; }
    }
}