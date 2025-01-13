using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;


namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="esAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet(Name = "obtenerAutores")]
        
        public async Task<ActionResult<List<AutorDTO>>> Get(IMapper mapper)
        {         
            var autores = await context.Autores.ToListAsync();
            var autorDTO = mapper.Map<List<AutorDTO>>(autores);
            return autorDTO;
        }
     
        [HttpGet("{id:int}",Name = "obtenerAutor")]
        [AllowAnonymous]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id,IMapper mapper)
        {
            var autor =  await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autoresLibroDB => autoresLibroDB.libro)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (autor is null)
            {
                return NotFound();
            }
            var autorDTO = mapper.Map<AutorDTOConLibros>(autor);

            return Ok(autorDTO);          
        }

        [HttpGet("{nombre}", Name ="obtenerAutorePorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre,IMapper mapper)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            var autorDTO = mapper.Map<List<AutorDTO>>(autores);

            return Ok(autorDTO);

        }
           
        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post([FromBody] CrearAutorDTO crearActorDTO, IMapper mapper)
        {
            var existeAutorConElMismoNombre =  await context.Autores.AnyAsync(x => x.Nombre == crearActorDTO.Nombre);

            if(existeAutorConElMismoNombre)
            {
                return BadRequest($"ya existe un autor con el nombre {crearActorDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(crearActorDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutor")]
        public async Task<ActionResult> Put(int id,CrearAutorDTO crearAutorDTO)
        {       

            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            autor = mapper.Map(crearAutorDTO, autor);
            autor.Id = id;

            
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "elimiarAutor")]
        public async Task<ActionResult> Delete(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if(autor == null)
            {
                return NotFound();
            }

            context.Remove(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
