using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_1.Models
{
    public class Involucrados_info
    {
        [Key]
        public int IdInvolucradoInfo { get; set; }
        public int IdInvolucrado { get; set; }

        public int IdRol { get; set; }

        public int IdPelicula { get; set; }

        [Required]
        public int OrdenAparicion { get; set; }
    }
}