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
using WebApiAutores.Utilidades;


namespace WebApiAutores.Controllers.v2
{
    [ApiController]
    [Route("api/autores")]
    [CabeceraEstaPresente("x-version", "2")]
    //[Route("api/v2/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="esAdmin")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService )
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutoresv2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {         
            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTO>>(autores);          
                                            
        }
     
        [HttpGet("{id:int}",Name = "obtenerAutorv2")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
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

        [HttpGet("{nombre}", Name ="obtenerAutorePorNombrev2")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute]string nombre,IMapper mapper)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            var autorDTO = mapper.Map<List<AutorDTO>>(autores);

            return Ok(autorDTO);

        }
           
        [HttpPost(Name = "crearAutorv2")]
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

        [HttpPut("{id:int}", Name = "actualizarAutorv2")]
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

        [HttpDelete("{id:int}", Name = "eliminarAutorv2")]
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
