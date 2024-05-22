using Proyecto_1.Models;
using Proyecto_1_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Proyecto_1.Controllers
{
    public class CalificacionesController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();


        public CalificacionesController()
        {
            entities.Configuration.LazyLoadingEnabled = true;
            entities.Configuration.ProxyCreationEnabled = true;
        }

        [HttpPost]
        [Route("api/Criticos/InsertarCalificacion")]
        public IHttpActionResult InsertarCalificacion(Models.Calificaciones nuevaCalificacion)
        {
            if (nuevaCalificacion == null)
            {
                return BadRequest("La calificación proporcionada es nula.");
            }

            try
            {
                int resultado = entities.SP_MantenimientosCalificacion(
                    idCriticoCalificacion: nuevaCalificacion.IdCriticoCalificacion,
                    idCritico: nuevaCalificacion.IdCritico,  
                    idPelicula: nuevaCalificacion.IdPelicula,
                    calificacion: nuevaCalificacion.Calificacion,
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/Criticos/ModificarCalificacion/{idCritico}")]
        public IHttpActionResult ModificarCalificacion(int idCriticoCal, int idCritico, Models.Calificaciones calificacionModificada)
        {
            if (calificacionModificada == null)
            {
                return BadRequest("Los datos de la calificación son nulos.");
            }

            var calificacionExistente = entities.Criticos_Calificacion.FirstOrDefault(c => c.IdCriticoCalificacion == idCriticoCal);
            if (calificacionExistente == null)
            {
                return NotFound();
            }

            try
            {
                calificacionExistente.IdPelicula = calificacionModificada.IdPelicula;
                calificacionExistente.Calificacion = calificacionModificada.Calificacion;

                int resultado = entities.SP_MantenimientosCalificacion(
                    idCriticoCalificacion: idCriticoCal,
                    idCritico: idCritico,
                    idPelicula: calificacionModificada.IdPelicula,
                    calificacion: calificacionModificada.Calificacion,
                    oP: 2 
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

        [HttpDelete]
        [Route("api/Criticos/EliminarCalificacion/{idCritico}")]
        public IHttpActionResult EliminarCalificacion(int idCriticoCal)
        {
            var calificacionExistente = entities.Criticos_Calificacion.FirstOrDefault(c => c.IdCriticoCalificacion == idCriticoCal);
            if (calificacionExistente == null)
            {
                return NotFound();
            }

            try
            {
                int resultado = entities.SP_MantenimientosCalificacion(
                    idCriticoCalificacion:idCriticoCal,
                    idCritico: null,
                    idPelicula: null,
                    calificacion: null,
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
