using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Entidades
{
    public class Libro
    {
        public int     Id      { get; set; }
        [Required]
        [StringLength(200)]
        public string  Titulo  { get; set; }
        public DateTime? FechaPublicacion {get; set;}
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros {get; set;}
        
      
    }
}