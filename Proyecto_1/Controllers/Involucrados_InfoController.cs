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
    public class Involucrados_InfoController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();


        public Involucrados_InfoController()
        {
            entities.Configuration.LazyLoadingEnabled = true;
            entities.Configuration.ProxyCreationEnabled = true;
        }

        [HttpPost]
        [Route("api/Involucrados/AgregarInvolucrado")]
        public IHttpActionResult AgregarInvolucrado(Models.Involucrados_info nuevoInvolucrado)
        {
            if (nuevoInvolucrado == null)
            {
                return BadRequest("El involucrado proporcionado es nulo.");
            }

            try
            {
                int resultado = entities.SP_Mantenimiento_InfoInv(
                    idInvolucradoInfo: nuevoInvolucrado.IdInvolucradoInfo,
                    idInvolucrado: nuevoInvolucrado.IdInvolucrado,
                    idRol: nuevoInvolucrado.IdRol,
                    idPelicula: nuevoInvolucrado.IdPelicula,
                    ordenAparicion: nuevoInvolucrado.OrdenAparicion,
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
        [Route("api/Involucrados/ModificarInvolucrado/{idInvolucrado}")]
        public IHttpActionResult ModificarInvolucrado(int idInvolucradoInf,int idInvolucrado, Models.Involucrados_info involucradoModificado)
        {
            if (involucradoModificado == null)
            {
                return BadRequest("Los datos del involucrado son nulos.");
            }

            var involucradoExistente = entities.Involucrados_Info.FirstOrDefault(i => i.IdInvolucrado == idInvolucrado);
            if (involucradoExistente == null)
            {
                return NotFound();
            }

            try
            {
                involucradoExistente.IdRol = involucradoModificado.IdRol;
                involucradoExistente.IdPelicula = involucradoModificado.IdPelicula;
                involucradoExistente.OrdenAparicion = involucradoModificado.OrdenAparicion;

                int resultado = entities.SP_Mantenimiento_InfoInv(
                     idInvolucradoInfo: idInvolucradoInf,
                    idInvolucrado: idInvolucrado,
                    idRol: involucradoModificado.IdRol,
                    idPelicula: involucradoModificado.IdPelicula,
                    ordenAparicion: involucradoModificado.OrdenAparicion,
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
        [Route("api/Involucrados/EliminarInvolucrado/{idInvolucrado}")]
        public IHttpActionResult EliminarInvolucrado(int idInvolucradoInf)
        {
            var involucradoExistente = entities.Involucrados_Info.FirstOrDefault(i => i.IdInvolucradoInfo == idInvolucradoInf);
            if (involucradoExistente == null)
            {
                return NotFound();
            }

            try
            {
                int resultado = entities.SP_Mantenimiento_InfoInv(
                    idInvolucradoInfo : idInvolucradoInf,
                    idInvolucrado: null,
                    idRol: null, 
                    idPelicula: null,
                    ordenAparicion: null,
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
