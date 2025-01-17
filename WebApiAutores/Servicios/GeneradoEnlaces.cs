using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Runtime.CompilerServices;
using WebApiAutores.DTOs;

namespace WebApiAutores.Servicios
{
    public class GeneradoEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradoEnlaces(IAuthorizationService authorizationService,IHttpContextAccessor httpContextAccessor,IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private async Task<bool> EsAdmin()
        {

            var httpContext = httpContextAccessor.HttpContext;
            var resutaldo =  await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return resutaldo.Succeeded;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }


        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirURLHelper();


            autorDTO.Enlaces.Add(new DatoHATEOAS(
                 enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id })
                , descripcion: "self"
                , metodo: "GET"));
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id })
               , descripcion: "autor-actualizar"
               , metodo: "PUT"));
            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("eliminarAutor", new { id = autorDTO.Id })
               , descripcion: "autor-eliminar"
               , metodo: "DELETE"));

        }

        public async Task GenerarEnlaces(LibroDTO libroDTO)
        {
            var esAdmin = await EsAdmin();
            var Url = ConstruirURLHelper();


            libroDTO.Enlaces.Add(new DatoHATEOAS(
                 enlace: Url.Link("obtenerLibro", new { id = libroDTO.Id })
                , descripcion: "self"
                , metodo: "GET"));
            libroDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("editarLibro", new { id = libroDTO.Id })
               , descripcion: "libro-actualizar"
               , metodo: "PUT"));
            libroDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("eliminarLibro", new { id = libroDTO.Id })
               , descripcion: "libro-eliminar"
               , metodo: "DELETE"));

        }


    }
}
