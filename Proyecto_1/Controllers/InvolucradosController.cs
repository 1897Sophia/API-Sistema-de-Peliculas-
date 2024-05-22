using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Proyecto_1.Models;
using Proyecto_1_DataAccess;

namespace Proyecto_1.Controllers
{
    public class InvolucradosController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();
        [HttpPost]
        public IHttpActionResult InsertarInvolucrado(Models.Involucrados nuevoInvolucrado)
        {
            if (nuevoInvolucrado == null)
            {
                return BadRequest("El usuario proporcionado es nulo.");
            }

            var InvolucradosExistente = entities.Involucrados.FirstOrDefault(u => u.IdInvolucrado == nuevoInvolucrado.IdInvolucrado
                                                                            || u.Facebook == nuevoInvolucrado.Facebook
                                                                            || u.Instagram == nuevoInvolucrado.Instagram
                                                                            || u.Twitter == nuevoInvolucrado.Twittwer);
            if (InvolucradosExistente != null)
            {
                var mensajeError = "El usuario ya existe.";
                return Content(HttpStatusCode.Conflict, mensajeError);
            }

            int resultado = entities.SP_MantenimientoInvolucrados(nuevoInvolucrado.IdInvolucrado,nuevoInvolucrado.Nombre, nuevoInvolucrado.Apellidos,
                                                                nuevoInvolucrado.Facebook, nuevoInvolucrado.Instagram,
                                                                nuevoInvolucrado.Twittwer, nuevoInvolucrado.Otros,1);

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
        public IHttpActionResult ModificarInvolucrado(int idInvolucrado, string Nombre, Models.Involucrados involucradoModificado)
        {
            if (involucradoModificado == null)
            {
                return BadRequest("Los datos del usuario son nulos.");
            }

            var involucradoExistente = entities.Involucrados.FirstOrDefault(u => u.IdInvolucrado == idInvolucrado && u.Nombre == Nombre);
            if (involucradoExistente == null)
            {
                return NotFound();
            }

            /*if (string.IsNullOrWhiteSpace(involucradoModificado.Nombre))
            {
                return BadRequest("El nombre es obligatorio.");
            }*/

            involucradoExistente.Nombre = involucradoModificado.Nombre;
            involucradoExistente.Apellidos = involucradoModificado.Apellidos;
            involucradoExistente.Facebook = involucradoModificado.Facebook;
            involucradoExistente.Instagram = involucradoModificado.Instagram;
            involucradoExistente.Twitter = involucradoModificado.Twittwer; 
            involucradoExistente.Otros = involucradoModificado.Otros;

            int resultado = entities.SP_MantenimientoInvolucrados(idInvolucrado, involucradoModificado.Nombre,
                                                                   involucradoModificado.Apellidos, involucradoModificado.Facebook,
                                                                   involucradoModificado.Instagram, involucradoModificado.Twittwer,
                                                                   involucradoModificado.Otros, 2);

            if (resultado == 1)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        [HttpDelete]
        public IHttpActionResult EliminarInvolucrado(int idInvolucrado)
        {
            var InvolucradoExistente = entities.Involucrados.FirstOrDefault(u => u.IdInvolucrado == idInvolucrado);
            if (InvolucradoExistente == null)
            {
                return NotFound();
            }

            int resultado = entities.SP_MantenimientoInvolucrados(idInvolucrado, null, null, null, null, null, null, 3);

            if (resultado == 1)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        [Route("api/Controlador/ObtenerInvolucrados")]
        public IHttpActionResult ObtenerInvolucrados(string nombre = null)
        {
            try
            {
                var resultados = entities.ObtenerInvolucrados(nombre).ToList();

                if (resultados == null || resultados.Count == 0)
                {
                    return NotFound();
                }

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("api/Controlador/ObtenerInvolucradosOrden")]
        public IHttpActionResult ObtenerInvolucradosOrden(int idPelicula)
        {
            try
            {
                var resultados = entities.Involucrados_Info
                                       .Where(info => info.IdPelicula == idPelicula)
                                       .OrderBy(info => info.OrdenAparicion)
                                       .ToList();

                if (resultados == null || !resultados.Any())
                {
                    return NotFound();
                }

                return Ok(resultados);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return InternalServerError(ex);
            }
        }



    }
}
