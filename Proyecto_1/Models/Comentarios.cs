using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Proyecto_1.Models
{
    public class Comentarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdComentario { get; set; }

        public int IdPelicula { get; set; }

        public int IdUsuario { get; set; }

        [Required]
        [StringLength(500)]
        public string ComentarioTexto { get; set; }

        public int? IdComentarioPadre { get; set; }

        public DateTime? Fecha { get; set; }

    }
}