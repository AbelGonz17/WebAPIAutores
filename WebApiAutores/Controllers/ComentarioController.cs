using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentarioController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentarioController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "obtenerComentariosLibro")]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existe)
            {
                return BadRequest($"el ID {libroId} del libro que seleccionaste no existe");
            }

            var comentario = await context.Comentarios
                .Where(x => x.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentario);
        }


        [HttpGet("{id:int}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var comentario = await context.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTO>(comentario);
        }

        [HttpPost(Name = "crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, CrearComentarioDTO crearComentarioDTO)
        {
            var emailClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "email");
            var email = emailClaim.Value;

            var usuario= await userManager.FindByEmailAsync(email);

            var usuarioId = usuario.Id;


            var existe = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existe)
            {
                return BadRequest($"el ID {libroId} del libro que seleccionaste no existe");
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);

            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId = libroId }, comentarioDTO);

        }

        [HttpPut("{id:int}",Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId,int id, CrearComentarioDTO crearComentarioDTO)
        {

            var existe = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existe)
            {
                return NotFound();
            }

            var existeComentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if(existeComentario == null )
            {
                return NotFound();
            }

            existeComentario= mapper.Map(crearComentarioDTO,existeComentario);
            existeComentario.Id = id;
            
            await context.SaveChangesAsync();
            return NoContent();

        }


    }
}
