using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using WebApiAutores.Utilidades;


namespace WebApiAutores.Controllers.V1
{
    //atributos
    [ApiController]
    [Route("api/autores")]
    //[Route("api/v1/autores")]
    [CabeceraEstaPresente("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esAdmin")]
   // [ApiConventionType(typeof(DefaultApiConventions))] esto es una forma de documentar los status code 
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerAutoresv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosDePaginacionEnCabecera(queryable);
            var autores = await queryable.OrderBy(autor => autor.Nombre).paginar(paginacionDTO).ToListAsync();          
            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpGet("{id:int}", Name = "obtenerAutorv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOSAutorFilterAttribute))]
        //[ProducesResponseType(404)]; esto es una forma de documentar el status code 
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
        {
            var autor = await context.Autores
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

        [HttpGet("{nombre}", Name = "obtenerAutorePorNombrev1")]
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre, IMapper mapper)
        {
            var autores = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            var autorDTO = mapper.Map<List<AutorDTO>>(autores);

            return Ok(autorDTO);

        }

        [HttpPost(Name = "crearAutorv1")]
        public async Task<ActionResult> Post([FromBody] CrearAutorDTO crearActorDTO, IMapper mapper)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == crearActorDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"ya existe un autor con el nombre {crearActorDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(crearActorDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorv1")]
        public async Task<ActionResult> Put(int id, CrearAutorDTO crearAutorDTO)
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

        //con esta manera damos mas informacion en el swagger
        /// <summary>
        /// Borra un autor
        /// </summary>
        /// <param name="id">Id del autor al Borrar</param>
        /// <returns></returns>
        [HttpDelete("{id:int}", Name = "eliminarAutorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            context.Remove(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
