using Microsoft.IdentityModel.Tokens;
using Proyecto_1.Models;
using Proyecto_1_DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Proyecto_1.Controllers
{
    public class UsuariosController : ApiController
    {
        private BDPeliculasEntities entities = new BDPeliculasEntities();

        public UsuariosController()
        {
            entities.Configuration.LazyLoadingEnabled = true;
            entities.Configuration.ProxyCreationEnabled = true;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetUsuariosEstado(int estado)
        {
            var query = from c in entities.Usuarios
                        where c.Estado == estado
                        select c;

            List<Usuarios> clientes = await query.ToListAsync();
            List<UsuarioResponse> respuesta = new List<UsuarioResponse>();
            clientes.ForEach(c => respuesta.Add(new UsuarioResponse()
            {
                IdUsuario = c.IdUsuario,
                Usuario = c.Usuario,
                Clave = c.Clave,
                Nombre = c.Nombre,
                Apellidos = c.Apellidos,
                Correo = c.Correo,
                Estado = c.Estado
            }));
            return Ok(respuesta);
        }
        [HttpPost]
        public IHttpActionResult InsertarUsuario(UsuarioResponse nuevoUsuario)
        {
            if (nuevoUsuario == null)
            {
                return BadRequest("El usuario proporcionado es nulo.");
            }

            var usuarioExistente = entities.Usuarios.FirstOrDefault(u => u.Usuario == nuevoUsuario.Usuario && u.Clave == nuevoUsuario.Clave);
            if (usuarioExistente != null)
            {
                var mensajeError = "El usuario ya existe.";
                return Content(HttpStatusCode.Conflict, mensajeError);
            }

            int resultado = entities.SP_MantenimientoUsuarios(1, null, nuevoUsuario.Usuario, nuevoUsuario.Clave, nuevoUsuario.Nombre, nuevoUsuario.Apellidos, nuevoUsuario.Correo, nuevoUsuario.Estado);

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
        public IHttpActionResult ModificarUsuario(int idUsuario, UsuarioResponse usuarioModificado)
        {
            if (usuarioModificado == null)
            {
                return BadRequest("Los datos del usuario son nulos.");
            }

            var usuarioExistente = entities.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.Usuario = usuarioModificado.Usuario;
            usuarioExistente.Clave = usuarioModificado.Clave;
            usuarioExistente.Nombre = usuarioModificado.Nombre;
            usuarioExistente.Apellidos = usuarioModificado.Apellidos;
            usuarioExistente.Correo = usuarioModificado.Correo;
            usuarioExistente.Estado = usuarioModificado.Estado;

            int resultado = entities.SP_MantenimientoUsuarios(2, idUsuario, usuarioModificado.Usuario, usuarioModificado.Clave, usuarioModificado.Nombre, usuarioModificado.Apellidos, usuarioModificado.Correo, 1);

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
        public IHttpActionResult EliminarUsuario(int idUsuario)
        {
            var usuarioExistente = entities.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            int resultado = entities.SP_MantenimientoUsuarios(3, idUsuario, null, null, null, null, null, null);

            if (resultado == 1)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        [HttpPut]
        [Route("api/Usuarios/activar_inactivar")]
        public IHttpActionResult ActivarInactivarUsuario([FromBody] ActivarInactivarResponse activarInactivar)
        {
            if (activarInactivar == null || activarInactivar.IdUsuario <= 0)
            {
                return BadRequest("Datos de activación/inactivación incorrectos.");
            }

            var resultado = entities.SP_ActivarInactivarUsuario(activarInactivar.IdUsuario, activarInactivar.Activar);

            if (resultado.FirstOrDefault() == true)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPut]
        [Route("api/Usuarios/activar_inactivar_nombre")]
        public IHttpActionResult ActivarInactivarUsuarioNombre([FromBody] ActivarInactivarNombre activarInactivar)
        {
            if (activarInactivar == null || string.IsNullOrEmpty(activarInactivar.Usuario))
            {
                return BadRequest("Datos de activación/inactivación incorrectos.");
            }

            var user = entities.Usuarios.FirstOrDefault(u => u.Usuario == activarInactivar.Usuario);
            if (user == null)
            {
                return NotFound();
            }

            var resultado = entities.SP_ActivarInactivarUsuario(user.IdUsuario, activarInactivar.Activar);

            if (resultado.FirstOrDefault() == true)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("api/Usuarios/estado_usuario/{username}")]
        public IHttpActionResult ObtenerEstadoUsuario(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Nombre de usuario inválido.");
            }

            var user = entities.Usuarios.FirstOrDefault(u => u.Usuario == username);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { Activo = user.Estado });
        }

        [HttpPost]
        [Route("api/Usuarios/login")]
        public IHttpActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Usuario) || string.IsNullOrEmpty(loginRequest.Clave))
            {
                return BadRequest("Usuario y contraseña son obligatorios.");
            }

            var resultadoVerificacion = entities.SP_VerificarUsuario(loginRequest.Usuario, loginRequest.Clave);

            if (resultadoVerificacion.FirstOrDefault() == true)
            {
                var token = GenerarTokenJwt(loginRequest.Usuario);

                return Ok(new { Token = token });
            }
            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Credenciales incorrectas"),
                ReasonPhrase = "Credenciales incorrectas"
            });
        }

        private string GenerarTokenJwt(string nombreUsuario)
        {
            var claveSecreta = GenerarClaveSecretaSegura(256); ;
            var issuer = "Proyecto_1";
            var audience = "Proyecto_1";
            var tiempoExpiracionMinutos = 15;

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, nombreUsuario)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var identity = new ClaimsIdentity(new GenericIdentity(nombreUsuario, "Token"), claims);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Subject = identity,
                Expires = DateTime.UtcNow.AddMinutes(tiempoExpiracionMinutos),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public static string GenerarClaveSecretaSegura(int longitudBits)
        {
            byte[] claveBytes = new byte[longitudBits / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(claveBytes);
            }
            return Convert.ToBase64String(claveBytes);
        }
 
        [HttpGet]
        [Route("api/Usuarios/ObtenerIdPorNombre/{nombreUsuario}")]
        public IHttpActionResult ObtenerIdUsuarioPorNombre(string nombreUsuario)
        {
            if (string.IsNullOrEmpty(nombreUsuario))
            {
                return BadRequest("Nombre de usuario inválido.");
            }

            var usuario = entities.Usuarios.FirstOrDefault(u => u.Usuario == nombreUsuario);

            if (usuario == null)
            {
                return Ok(new { IdUsuario = -1 });  
            }

            return Ok(new { IdUsuario = usuario.IdUsuario });
        }


    }
}

