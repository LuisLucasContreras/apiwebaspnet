using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.Validaciones;

namespace ApiWeb.Entidades
{
    public class Autor
    {
        public int Id {get; set;}   
        public string Nombre {get; set;}     
        public List<AutorLibro> AutoresLibros {get;set;}

       
    }
}