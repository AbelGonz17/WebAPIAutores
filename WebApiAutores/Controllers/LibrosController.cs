using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers
{
    [ApiController]
    [Route("api/Libros")]
    public class LibrosController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<LibroDTO>>> Get()
        {
            var libro = await context.Libros.ToListAsync();
            return mapper.Map<List<LibroDTO>>(libro);
        }

        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
        {
            var libro =  await context.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .FirstOrDefaultAsync(x => x.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOConAutores>(libro);
        }
        [HttpPost]
        public async Task<ActionResult> Post(CrearLibroDTO crearLibroDTO)
        {
            if (crearLibroDTO.AutoresIds.IsNullOrEmpty())
            {
                return BadRequest("No se puede crear libro sin autores");
            }

            var autoresIds = await context.Autores
                .Where(x => crearLibroDTO.AutoresIds
                .Contains(x.Id)).Select(x => x.Id).ToListAsync();

            if(crearLibroDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("no existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(crearLibroDTO);

           

            context.Add(libro);
            await context.SaveChangesAsync();
            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put (int id, CrearLibroDTO crearLibroDTO)
        {
            var libroDB = await context.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(libroDB == null)
            {
                return NotFound();
            }

            libroDB = mapper.Map(crearLibroDTO, libroDB);

            OrdenLibro(libroDB); 

            await context.SaveChangesAsync();
            return NoContent();
        }



        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            context.Remove(libro);
            await context.SaveChangesAsync();
            return Ok();
        }

        private void OrdenLibro(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();

            }

            var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

            if(libro == null)
            {
                return NotFound();  
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libro);

            patchDocument.ApplyTo(libroDTO);

            var esValido = TryValidateModel(libroDTO);

            if(!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libro);


            await context.SaveChangesAsync();
            return NoContent();
        }


    }

    
}
