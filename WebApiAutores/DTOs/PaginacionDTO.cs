namespace WebApiAutores.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int RecordPorPagina = 10;
        private readonly int cantidadMaximaPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return RecordPorPagina;
            }

            set
            {
                RecordPorPagina = (value > cantidadMaximaPagina) ? cantidadMaximaPagina:value;
            }

        }
        
    }
}
