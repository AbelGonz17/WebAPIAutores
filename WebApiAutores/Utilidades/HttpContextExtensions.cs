using Microsoft.EntityFrameworkCore;

namespace WebApiAutores.Utilidades
{
    public static class HttpContextExtensions
    {
        //esto es para que la cantidad de registros total aparezca en la cabecera
        public async static Task InsertarParametrosDePaginacionEnCabecera<T>(this HttpContext httpContext
            ,IQueryable<T> queryable)
        {
            if(httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Append("CantidadTotalRegistros", cantidad.ToString());

        }

    }
}
