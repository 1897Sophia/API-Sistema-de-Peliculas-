using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Proyecto_1.Models;
using Proyecto_1_DataAccess;

namespace Proyecto_1.Controllers
{
    public class CriticosController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();

        [HttpPost]
        [Route("api/Controlador/MantenimientoCritico")]
        public IHttpActionResult MantenimientoCritico([FromBody] Models.Criticos input)
        {
            if (input == null)
            {
                return BadRequest("Entrada inválida.");
            }

            try
            {
                switch (input.OP)
                {
                    case 1:
                        entities.SP_MantenimientosCriticos(input.IdCritico, input.Nombre, 1);
                        return StatusCode(HttpStatusCode.Created);

                    case 2:
                        entities.SP_MantenimientosCriticos(input.IdCritico, input.Nombre, 2);
                        return Ok("Registro actualizado correctamente.");

                    case 3:
                        entities.SP_MantenimientosCriticos(input.IdCritico, null, 3);
                        return Ok("Registro eliminado correctamente.");

                    default:
                        return BadRequest("Operación no válida.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPut]
        public IHttpActionResult ModificarUsuario(int idCritico, Models.Criticos usuarioModificado)
        {
            if (usuarioModificado == null)
            {
                return BadRequest("Los datos del usuario son nulos.");
            }

            var usuarioExistente = entities.Criticos.FirstOrDefault(u => u.IdCritico == idCritico);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.Nombre = usuarioModificado.Nombre;

            int resultado = entities.SP_MantenimientosCriticos(idCritico, usuarioModificado.Nombre,2);

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
        [Route("api/Controlador/BorrarUsuario/{idCritico}")]
        public IHttpActionResult BorrarUsuario(int idCritico)
        {
            try
            {
                var usuarioExistente = entities.Criticos.FirstOrDefault(u => u.IdCritico == idCritico);
                if (usuarioExistente == null)
                {
                    return NotFound();
                }

                int resultado = entities.SP_MantenimientosCriticos(idCritico, null, 3);

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

