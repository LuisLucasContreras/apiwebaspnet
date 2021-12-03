using System.Net;
using Microsoft.AspNetCore.Mvc;
using ApiWeb.Entidades;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ApiWeb.Filtros;
using ApiWeb.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiWeb.Utilidades;

namespace ApiWeb.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "esAdmin")]
    public class AutoresControllers : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public ILogger<AutoresControllers> logger { get; }

        public AutoresControllers(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
            this.logger = logger;
        }


        [HttpGet(Name = "obtenerAutores")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable   = context.Autores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var autores     = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);

        }


        [HttpGet("{id:int}", Name = "obtenerAutor")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id,[FromHeader] string incluirHATEOAS)
        {
            var autor = await context.Autores
            .Include(autorDb => autorDb.AutoresLibros)
            .ThenInclude(autorLibroDb => autorLibroDb.Libro)
            .FirstOrDefaultAsync(autor => autor.Id == id);
            if (autor == null)
            {
                return NotFound($"No se encontro el autor con el id:{id}");
            }

            var dto = mapper.Map<AutorDTOConLibros>(autor);
            return dto;
        }


        

        [HttpGet("{nombre}", Name = "obtenerAutorPorNombre")]
        public async Task<ActionResult<List<AutorDTO>>> GetByName(string nombre)
        {
            var autores = await context.Autores.Where(autor => autor.Nombre.Contains(nombre)).ToListAsync();
            if (autores == null)
            {
                return NotFound($"No se encontro el autor con el nombre:{nombre}");
            }

            return mapper.Map<List<AutorDTO>>(autores);
        }

        [HttpPut("{id}", Name = "actualizarAutor")]
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {
            var autorexiste = await context.Autores.AnyAsync(autorDB => autorDB.Id == id);

            if (!autorexiste)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(autor => autor.Nombre == autorCreacionDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre: {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("obtenerAutor", new { id = autor.Id }, autorDTO);
        }

        [HttpDelete("{id:int}", Name = "borrarAutor")] // api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }



    }
}