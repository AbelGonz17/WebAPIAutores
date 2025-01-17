using WebApiAutores.DTOs;

namespace WebApiAutores.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> paginar<T> (this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable
                .Skip((paginacionDTO.Pagina - 1) * paginacionDTO.RecordsPorPagina)
                .Take(paginacionDTO.RecordsPorPagina);
        }
    }
}
