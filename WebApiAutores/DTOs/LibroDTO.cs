﻿using WebApiAutores.Entidades;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class LibroDTO:Recurso
    {
        public int Id { get; set; }      
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }

        //public List<ComentarioDTO> Comentarios { get; set; }
    }
}
