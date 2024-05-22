using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Proyecto_1.Models;
using Proyecto_1_DataAccess;

namespace Proyecto_1.Controllers
{
    public class ComentariosController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();

        public ComentariosController()
        {
            entities.Configuration.LazyLoadingEnabled = true;
            entities.Configuration.ProxyCreationEnabled = true;
        }

        [HttpPost]
        [Route("api/Comentario/CrearComentario")]
        public IHttpActionResult CrearComentario(Models.Comentarios nuevoComentario)
        {
            if (nuevoComentario == null)
            {
                return BadRequest("El comentario proporcionado es nulo.");
            }

            int? idComentarioPadre = nuevoComentario.IdComentarioPadre;

            int resultado = entities.SP_Mantenimiento_Comentarios(
                idComentario: 0,
                idUsuario: nuevoComentario.IdUsuario,
                idPelicula: nuevoComentario.IdPelicula,
                comentarioTexto: nuevoComentario.ComentarioTexto,
                idComentarioPadre: idComentarioPadre,
                oP: 1
            );

            if (resultado == 1)
            {
                return StatusCode(HttpStatusCode.Created);
            }
            else
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("api/Comentario/ModificarComentario")]
        public IHttpActionResult ModificarComentario(int idComentario, [FromBody] ComentarioModificado comentarioModificado)
        {
            try
            {
                if (comentarioModificado == null || comentarioModificado.ComentarioTexto == null)
                {
                    return BadRequest("Los datos del comentario son inválidos");
                }

                int resultado = entities.SP_Mantenimiento_Comentarios(
                    idComentario,
                    null,
                    null,
                    comentarioModificado.ComentarioTexto,
                    null,
                    2
                );

                if (resultado == 0)
                {
                    return NotFound();
                }

                return Ok("Comentario modificado con éxito");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        public class ComentarioModificado
        {
            public string ComentarioTexto { get; set; }
        }

        [HttpDelete]
        [Route("api/Comentario/EliminarComentario/{idComentario}")]
        public IHttpActionResult EliminarComentario(int idComentario)
        {
            var comentarioExistente = entities.Comentarios.Find(idComentario);
            if (comentarioExistente == null)
            {
                return NotFound();
            }

            try
            {
                int resultado = entities.SP_Mantenimiento_Comentarios(
                    idComentario: idComentario,
                    idUsuario: 0,
                    idPelicula: 0,
                    comentarioTexto: null,
                    idComentarioPadre: 0,
                       oP: 3
                );

                if (resultado == 1)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("api/Comentario/ObtenerComentariosConRespuestas")]
        public IHttpActionResult ObtenerComentariosConRespuestas(int idPelicula)
        {
            try
            {
                var comentarios = entities.Comentarios
                    .Where(c => c.IdPelicula == idPelicula && c.IdComentarioPadre == null)
                    .ToList();

                var comentariosJerarquicos = new List<dynamic>();

                Action<List<Proyecto_1_DataAccess.Comentarios>, dynamic> obtenerRespuestas = null;
                obtenerRespuestas = (lista, comentarioPadre) =>
                {
                    var respuestas = lista
                        .Where(c => c.IdComentarioPadre == comentarioPadre.IdComentario)
                        .Select(c => new
                        {
                            IdComentario = c.IdComentario,
                            NombreUsuario = ObtenerNombreUsuario(c.IdUsuario ?? 0), // Usar 0 u otro valor predeterminado en caso de nulo
                            ComentarioTexto = c.ComentarioTexto,
                            Fecha = c.Fecha,
                            Respuestas = new List<dynamic>()
                        })
                        .ToList();

                    foreach (var respuesta in respuestas)
                    {
                        obtenerRespuestas(lista, respuesta);
                    }

                    comentarioPadre.Respuestas.AddRange(respuestas);
                };

                foreach (var comentarioOriginal in comentarios)
                {
                    var comentarioConRespuestas = new
                    {
                        IdComentario = comentarioOriginal.IdComentario,
                        NombreUsuario = ObtenerNombreUsuario(comentarioOriginal.IdUsuario ?? 0), // Usar 0 u otro valor predeterminado en caso de nulo
                        ComentarioTexto = comentarioOriginal.ComentarioTexto,
                        Fecha = comentarioOriginal.Fecha,
                        Respuestas = new List<dynamic>()
                    };

                    obtenerRespuestas(entities.Comentarios.ToList(), comentarioConRespuestas);

                    comentariosJerarquicos.Add(comentarioConRespuestas);
                }

                return Ok(comentariosJerarquicos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private string ObtenerNombreUsuario(int idUsuario)
        {
            var usuario = entities.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            return usuario != null ? usuario.Nombre : "Usuario Desconocido";
        }

        [HttpDelete]
        [Route("api/Comentario/EliminarComentario/{idUsuario}/{idComentario}")]
        public IHttpActionResult EliminarComentario(int idUsuario, int idComentario)
        {
            var comentarioExistente = entities.Comentarios
                .FirstOrDefault(c => c.IdComentario == idComentario && c.IdUsuario == idUsuario);

            if (comentarioExistente == null)
            {
                return NotFound();
            }

            try
            {
                int resultado = entities.SP_Mantenimiento_Comentarios(
                    idComentario: idComentario,
                    idUsuario: idUsuario,
                    idPelicula: 0,
                    comentarioTexto: null,
                    idComentarioPadre: 0,
                    oP: 3
                );

                if (resultado == 1)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else
                {
                    return StatusCode(HttpStatusCode.InternalServerError);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}