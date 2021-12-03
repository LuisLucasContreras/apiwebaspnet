using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ApiWeb.Servicios 
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor
        )
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirUrlHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return resultado.Succeeded;
        }

        public async Task GeneralEnlaces(AutorDTO autorDTO)
        {

            var esAdmin = await EsAdmin();
            var Url = ConstruirUrlHelper();

            autorDTO.Enlaces.Add(new DatosHATEOAS(
                enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }),
                descripcion: "self",
                metodo: "GET"
            ));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new DatosHATEOAS(
                enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }),
                descripcion: "autor-actualizar",
                metodo: "PUT"
            ));

                autorDTO.Enlaces.Add(new DatosHATEOAS(
                    enlace: Url.Link("crearAutor", new { id = autorDTO.Id }),
                    descripcion: "self",
                    metodo: "DELETE"
                ));
            }
        }
    }
}