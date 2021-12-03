using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWeb.DTOs 
{
    public class coleccionDeRecursos<T> : Recurso where T: Recurso
    {
        public List<T> Valores { get; set; }
        
        
    }
}