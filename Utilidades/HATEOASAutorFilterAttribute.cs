using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.DTOs;
using ApiWeb.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiWeb.Utilidades 
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFilterAttribute(GeneradorEnlaces generadorEnlaces )
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);

            if(!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var autorDTO = resultado.Value as AutorDTO;
            if(autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ?? throw new ArgumentException("Se esperaba una instamcia de AurorDTO");

                autoresDTO.ForEach(async autor => await generadorEnlaces.GeneralEnlaces(autor));
                resultado.Value = autoresDTO;
            }
            else
            {
                await generadorEnlaces.GeneralEnlaces(autorDTO);
            }
            
            await next();
        }
    }
}