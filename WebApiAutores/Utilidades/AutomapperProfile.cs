using AutoMapper;
using Microsoft.Identity.Client;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutomapperProfile:Profile
    {
        public AutomapperProfile()
        {
            CreateMap<CrearAutorDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));

            CreateMap<CrearLibroDTO, Libro>()
                .ForMember(Libro => Libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));

            CreateMap<LibroPatchDTO, Libro>().ReverseMap();


            CreateMap<CrearComentarioDTO , Comentario>();
            CreateMap<Comentario, ComentarioDTO >();
        }

        private List<LibroDTO> MapAutorDTOLibros(Autor autor,AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if(autor.AutoresLibros == null) { return resultado; }

            foreach (var autorlibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorlibro.LibroId,
                    Titulo = autorlibro.libro.Titulo

                });
            }


            return resultado;
        }



        private List<AutorDTO> MapLibroDTOAutores(Libro libro , LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if(libro.AutoresLibros == null) { return resultado; }

            foreach(var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;

        }

        private List<AutorLibro> MapAutoresLibros(CrearLibroDTO crearLibro, Libro libro)
        {      
            var resultado = new List<AutorLibro>();

            if(crearLibro.AutoresIds == null) { return resultado; }

            foreach (var autorId in crearLibro.AutoresIds)
            {
                resultado.Add(new AutorLibro() {AutorId = autorId});
            }

            return resultado;
        }
    }
}
