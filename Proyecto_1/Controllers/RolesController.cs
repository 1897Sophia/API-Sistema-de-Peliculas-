using Proyecto_1_DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Proyecto_1.Controllers
{
    public class RolesController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();


        public RolesController()
        {
            entities.Configuration.LazyLoadingEnabled = true;
            entities.Configuration.ProxyCreationEnabled = true;
        }

        [HttpPost]
        [Route("api/Roles/CrearRol")]
        public IHttpActionResult CrearRol(Models.Roles nuevoRol)
        {
            if (nuevoRol == null)
            {
                return BadRequest("El rol proporcionado es nulo.");
            }

            try
            {
                int resultado = entities.SP_MantenimientosRoles(
                    idRol: 0, 
                    nombre: nuevoRol.Nombre,
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
        [Route("api/Roles/ModificarRol/{idRol}")]
        public IHttpActionResult ModificarRol(int idRol, Models.Roles rolModificado)
        {
            if (rolModificado == null)
            {
                return BadRequest("Los datos del rol son nulos.");
            }

            var rolExistente = entities.Roles.FirstOrDefault(r => r.IdRol == idRol);
            if (rolExistente == null)
            {
                return NotFound();
            }

            try
            {
                rolExistente.Nombre = rolModificado.Nombre;

                int resultado = entities.SP_MantenimientosRoles(
                    idRol: idRol,
                    nombre: rolModificado.Nombre,
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
        [Route("api/Roles/EliminarRol/{idRol}")]
        public IHttpActionResult EliminarRol(int idRol)
        {
            var rolExistente = entities.Roles.FirstOrDefault(r => r.IdRol == idRol);
            if (rolExistente == null)
            {
                return NotFound();
            }

            try
            {
                int resultado = entities.SP_MantenimientosRoles(
                    idRol: idRol,
                    nombre: null, 
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
