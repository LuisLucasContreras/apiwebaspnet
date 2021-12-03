using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWeb.DTOs 
{
    public class LibroDTOCOnAutores : LibroDTO
    {
        public List<AutorDTO> Autores { get; set; }
        
    }
}