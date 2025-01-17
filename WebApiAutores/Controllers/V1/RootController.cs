using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAutores.DTOs;

namespace WebApiAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }


        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datoHateoas = new List<DatoHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            if (esAdmin.Succeeded)
            {
                datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { })
                     , descripcion: "autores", metodo: "GET"));

                datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { })
                    , descripcion: "autor-crear", metodo: "POST"));

            }

            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { })
                , descripcion: "self", metodo: "GET"));        

            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerLibros", new { })
               , descripcion: "libros", metodo: "GET"));

            datoHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { })
               , descripcion: "libros-crear", metodo: "POST"));


            return datoHateoas;
        }


    }
}
