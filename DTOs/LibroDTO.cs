using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWeb.DTOs
{
    public class LibroDTO
    {
        public int     Id      { get; set; }
        public string  Titulo  { get; set; }
        public DateTime FechaPublicacion { get; set; }
        
        
        
        /* public List<ComentarioDTO> comentarios {get; set;} */
    }
}