using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.Validaciones;

namespace ApiWeb.DTOs
{
    public class LibroPatchDTO
    {
        [Required(ErrorMessage = "El Campo es requerido")]  
        [PrimeraLetraMayusculaAttribute]
        public string Titulo {get; set;}
        public DateTime FechaPublicacion {get; set;}
        
    }
}