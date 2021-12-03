using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ApiWeb.Entidades 
{
    public class Comentario
    {
        public int id {get; set;}
        public string Contenido {get;set;}
        public int LibroId {get; set;}
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
        public Libro Libro {get;set;}
    }
}