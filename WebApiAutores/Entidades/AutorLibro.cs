using System.Reflection.Metadata.Ecma335;

namespace WebApiAutores.Entidades
{
    public class AutorLibro
    {
        public int  AutorId { get; set; }
        public int LibroId { get; set; }
        public int Orden { get; set; }
        public Libro libro { get; set; }
        public  Autor Autor { get; set; }

    }
}
