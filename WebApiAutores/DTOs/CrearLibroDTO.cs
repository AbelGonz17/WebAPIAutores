using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs
{
    public class CrearLibroDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength:250)]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<int> AutoresIds { get; set; }
        
    }
}
