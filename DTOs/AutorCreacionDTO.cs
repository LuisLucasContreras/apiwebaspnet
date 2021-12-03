using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.Validaciones;

namespace ApiWeb.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El Campo es requerido")]  
        [StringLength(8, ErrorMessage = "el campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 6)]   
        [PrimeraLetraMayusculaAttribute]
        public string Nombre {get; set;}
    }
}