using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWeb.Controllers
{


    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatosHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatosHATEOAS>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");

            datosHateoas.Add(new DatosHATEOAS(enlace: Url.Link("ObtenerRoot", new { }),
                descripcion: "self",
                metodo: "GET"
            ));

            datosHateoas.Add(new DatosHATEOAS(enlace: Url.Link("obtenerAutores", new { }),
                descripcion: "autores",
                metodo: "Get"
            ));
            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatosHATEOAS(enlace: Url.Link("crearAutor", new { }),
                               descripcion: "autor-crear",
                               metodo: "POST"
                           ));

                datosHateoas.Add(new DatosHATEOAS(enlace: Url.Link("crearLibro", new { }),
                    descripcion: "libro-crear",
                    metodo: "POST"
                ));
            }


            return datosHateoas;
        }

    }
}