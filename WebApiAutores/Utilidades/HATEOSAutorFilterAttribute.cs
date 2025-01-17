using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Validations;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOSAutorFilterAttribute:HATEOASFiltroAttribute
    {
        private readonly GeneradoEnlaces generadoEnlaces;

        public HATEOSAutorFilterAttribute(GeneradoEnlaces generadoEnlaces)
        {
            this.generadoEnlaces = generadoEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if(!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var autorDTO = resultado.Value as AutorDTO;
            if(autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ?? throw new ArgumentException("se esperaba una instancia de autorDTO o list<autorDTO>");

                autoresDTO.ForEach(async autor => await generadoEnlaces.GenerarEnlaces(autor));
                resultado.Value = autoresDTO;
            }
            else
            {
                await generadoEnlaces.GenerarEnlaces(autorDTO);

            }           
            await next();
        }
    }
}
