using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using Proyecto_1.Controllers;
using Proyecto_1.Models;
using Proyecto_1_DataAccess;

public class PeliculasController : ApiController
{
    private BDPeliculasEntities entities = new BDPeliculasEntities();

    public PeliculasController()
    {
        entities.Configuration.LazyLoadingEnabled = true;
        entities.Configuration.ProxyCreationEnabled = true;
    }

    [HttpPost]
    public IHttpActionResult CrearPelicula(Proyecto_1.Models.Peliculas nuevaPelicula)
    {
        if (nuevaPelicula == null)
        {
            return BadRequest("La película proporcionada es nula.");
        }

        try
        {
            int resultado = entities.SP_Mantenimiento_Peliculas(
                idPelicula: 0,
                nombre: nuevaPelicula.Nombre,
                descripcion: nuevaPelicula.Descripcion,
                poster: nuevaPelicula.Poster,
                fecha: nuevaPelicula.Fecha,
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
    public IHttpActionResult ModificarPelicula(int idPeli, Proyecto_1.Models.Peliculas peliculaModificada)
    {
        if (peliculaModificada == null)
        {
            return BadRequest("Los datos de la película son nulos.");
        }

        var peliculaExistente = entities.Peliculas.FirstOrDefault(p => p.IdPelicula == idPeli);
        if (peliculaExistente == null)
        {
            return NotFound();
        }

        try
        {
            peliculaExistente.Nombre = peliculaModificada.Nombre;
            peliculaExistente.Descripcion = peliculaModificada.Descripcion;
            peliculaExistente.Poster = peliculaModificada.Poster;
            peliculaExistente.Fecha = peliculaModificada.Fecha;

            int resultado = entities.SP_Mantenimiento_Peliculas(
                idPelicula: idPeli,
                nombre: peliculaModificada.Nombre,
                descripcion: peliculaModificada.Descripcion,
                poster: peliculaModificada.Poster,
                fecha: peliculaModificada.Fecha,
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
    public IHttpActionResult EliminarPelicula(int idPeli)
    {
        var peliculaExistente = entities.Peliculas.FirstOrDefault(p => p.IdPelicula == idPeli);
        if (peliculaExistente == null)
        {
            return NotFound();
        }

        try
        {
            int resultado = entities.SP_Mantenimiento_Peliculas(
                idPelicula: idPeli,
                nombre: null,
                descripcion: null,
                poster: null,
                fecha: null,
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
    [Route("api/Peliculas/ObtenerPeliculasRecientes")]
    public IHttpActionResult ObtenerPeliculasRecientes()
    {
        try
        {
            var resultado = entities.SP_ObtenerInformacionPeliculaRecientes().ToList();

            if (resultado.Any())
            {

                return Ok(resultado);
            }
            else
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
    [HttpGet]
    [Route("api/Peliculas/ObtenerPeliculaPorId/{id}")]
    public IHttpActionResult ObtenerPeliculaPorId(int id)
    {
        try
        {
            var pelicula = entities.Peliculas.FirstOrDefault(p => p.IdPelicula == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            var comentariosDePelicula = OrganizarComentariosAnidados(pelicula.IdPelicula);
            var peliculaConComentarios = new
            {
                IdPelicula = pelicula.IdPelicula,
                Titulo = pelicula.Nombre,
                Poster = pelicula.Poster,
                Descripcion = pelicula.Descripcion,
                Fecha = pelicula.Fecha,
                Criticos = entities.Criticos
                    .Where(c => c.Criticos_Calificacion.Any(p => p.IdPelicula == pelicula.IdPelicula))
                    .Select(c => new
                    {
                        IdCritico = c.IdCritico,
                        NombreCritico = c.Nombre,
                        Calificacion = entities.Criticos_Calificacion
                            .Where(cc => cc.IdPelicula == pelicula.IdPelicula && cc.IdCritico == c.IdCritico)
                            .Select(cc => cc.Calificacion)
                            .FirstOrDefault()
                    }).ToList(),
                Involucrados = entities.Involucrados_Info
                    .Where(i => i.IdPelicula == pelicula.IdPelicula)
                    .Select(i => new
                    {
                        Involucrado = entities.Involucrados
                            .Where(invo => invo.IdInvolucrado == i.IdInvolucrado)
                            .Select(invo => new
                            {
                                IdInvolucrado = invo.IdInvolucrado,
                                Nombre = invo.Nombre,
                                Apellidos = invo.Apellidos,
                                Facebook = invo.Facebook,
                                Instagram = invo.Instagram,
                                Twitter = invo.Twitter,
                                Otros = invo.Otros,
                                Rol = entities.Roles
                                    .Where(r => r.IdRol == i.IdRol)
                                    .Select(r => r.Nombre)
                                    .FirstOrDefault()
                            }).FirstOrDefault()
                    }).ToList(),
                Comentarios = comentariosDePelicula
            };

            return Ok(peliculaConComentarios);
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
    [HttpGet]
    [Route("api/Peliculas/ObtenerPeliculas")]
    public IHttpActionResult ObtenerPeliculas()
    {
        try
        {
            var peliculas = entities.Peliculas.ToList();  // Traer todas las películas desde la base de datos

            var peliculasConComentarios = new List<dynamic>();

            foreach (var pelicula in peliculas)
            {
                var comentariosDePelicula = OrganizarComentariosAnidados(pelicula.IdPelicula);
                var peliculaConComentarios = new
                {
                    IdPelicula = pelicula.IdPelicula,
                    Titulo = pelicula.Nombre,
                    Criticos = entities.Criticos
                    .Where(c => c.Criticos_Calificacion.Any(p => p.IdPelicula == pelicula.IdPelicula))
                    .Select(c => new
                    {
                        IdCritico = c.IdCritico,
                        NombreCritico = c.Nombre,
                        Calificacion = entities.Criticos_Calificacion
                            .Where(cc => cc.IdPelicula == pelicula.IdPelicula && cc.IdCritico == c.IdCritico)
                            .Select(cc => cc.Calificacion)
                            .FirstOrDefault()
                    }).ToList(),
                    Involucrados = entities.Involucrados_Info
                    .Where(i => i.IdPelicula == pelicula.IdPelicula)
                    .Select(i => new
                    {
                        Involucrado = entities.Involucrados
                            .Where(invo => invo.IdInvolucrado == i.IdInvolucrado)
                            .Select(invo => new
                            {
                                IdInvolucrado = invo.IdInvolucrado,
                                Nombre = invo.Nombre,
                                Apellidos = invo.Apellidos,
                                Facebook = invo.Facebook,
                                Instagram = invo.Instagram,
                                Twitter = invo.Twitter,
                                Otros = invo.Otros
                            }).FirstOrDefault()
                    }).ToList(),
                    Comentarios = comentariosDePelicula
                };
                peliculasConComentarios.Add(peliculaConComentarios);
            }

            return Ok(peliculasConComentarios);
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
    private dynamic OrganizarComentariosAnidados(int idPelicula)
    {
        var comentariosRaiz = entities.Comentarios
            .Where(c => c.IdPelicula == idPelicula && c.IdComentarioPadre == null)
            .ToList();

        List<dynamic> comentariosJerarquicos = new List<dynamic>();

        Action<List<Proyecto_1_DataAccess.Comentarios>, dynamic> obtenerRespuestas = null;
        obtenerRespuestas = (comentarios, comentarioPadre) =>
        {
            var respuestas = comentarios
                .Where(c => c.IdComentarioPadre == comentarioPadre.IdComentario)
                .Select(c => new
                {
                    IdComentario = c.IdComentario,
                    NombreUsuario = ObtenerNombreUsuario(c.IdUsuario ?? 0),
                    ComentarioTexto = c.ComentarioTexto,
                    Fecha = c.Fecha,
                    Respuestas = new List<dynamic>()
                })
                .ToList();

            foreach (var respuesta in respuestas)
            {
                obtenerRespuestas(comentarios, respuesta);
            }

            comentarioPadre.Respuestas.AddRange(respuestas);
        };

        foreach (var comentarioRaiz in comentariosRaiz)
        {
            var comentarioConRespuestas = new
            {
                IdComentario = comentarioRaiz.IdComentario,
                NombreUsuario = ObtenerNombreUsuario(comentarioRaiz.IdUsuario ?? 0),
                ComentarioTexto = comentarioRaiz.ComentarioTexto,
                Fecha = comentarioRaiz.Fecha,
                Respuestas = new List<dynamic>()
            };

            obtenerRespuestas(entities.Comentarios.ToList(), comentarioConRespuestas);

            comentariosJerarquicos.Add(comentarioConRespuestas);
        }

        return comentariosJerarquicos;
    }
    private string ObtenerNombreUsuario(int idUsuario)
    {
        var usuario = entities.Usuarios.FirstOrDefault(u => u.IdUsuario == idUsuario);
        return usuario != null ? usuario.Nombre : "Usuario Desconocido";
    }
    [HttpGet]
    [Route("api/Peliculas/BuscarPeliculaPorNombre")]
    public IHttpActionResult BuscarPeliculaPorNombre(string nombrePelicula)
    {
        try
        {
            SqlParameter nombreParam = new SqlParameter("@NombrePelicula", nombrePelicula);
            var peliculas = entities.Database.SqlQuery<Proyecto_1.Models.Peliculas>("SP_BuscarPeliculaPorNombre @NombrePelicula", nombreParam).ToList();

            if (peliculas.Count > 0)
            {
                return Ok(peliculas);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
}
