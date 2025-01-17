using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOASLibroFilterAttribute:HATEOASFiltroAttribute
    {
        private readonly GeneradoEnlaces generadoEnlaces;

        public HATEOASLibroFilterAttribute(GeneradoEnlaces generadoEnlaces)
        {
            this.generadoEnlaces = generadoEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var libroDTO = resultado.Value as LibroDTO;
            if (libroDTO == null)
            {
                var librosDTO = resultado.Value as List<LibroDTO> ?? throw new ArgumentException("se esperaba una instancia de libroDTO o list<libroDTO>");

                librosDTO.ForEach(async libro => await generadoEnlaces.GenerarEnlaces(libro));
                resultado.Value = librosDTO;
            }
            else
            {
                await generadoEnlaces.GenerarEnlaces(libroDTO);

            }
            await next();
        }
    }
}
