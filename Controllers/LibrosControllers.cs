using System.Linq;
using ApiWeb;
using ApiWeb.DTOs;
using ApiWeb.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Namespace
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosControllers : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public LibrosControllers(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet(Name = "obtenerLibros")]
        public async Task<ActionResult<List<LibroDTO>>> Get()
        {
            var libros = await context.Libros.ToListAsync();

            return mapper.Map<List<LibroDTO>>(libros);
        }



        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOCOnAutores>> Get(int id)
        {
            var libro = await context.Libros
            .Include(librodb => librodb.AutoresLibros)
            .ThenInclude(autorLibroDb => autorLibroDb.Autor)
            .FirstOrDefaultAsync(libro => libro.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();
            return mapper.Map<LibroDTOCOnAutores>(libro);

        }

        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {

            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear libros sin autor");
            }

            var autoresIds = await context.Autores
            .Where(autoresDb => libroCreacionDTO.AutoresIds.Contains(autoresDb.Id))
            .Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            AsignarOrden(libro);

            await context.AddAsync(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);


            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroDb = await context.Libros
            .Include(libroDb => libroDb.AutoresLibros)
            .FirstOrDefaultAsync(libroDb => libroDb.Id == id);

            if (libroDb == null)
            {
                return NotFound();
            }

            libroDb = mapper.Map(libroCreacionDTO, libroDb);

            AsignarOrden(libroDb);
            await context.SaveChangesAsync();
            return NoContent();
        }

        private void AsignarOrden(Libro libro)
        {
              if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }

            var libroDb = await context.Libros.FirstOrDefaultAsync(libroDb => libroDb.Id == id);

            if(libroDb == null)
            {
                return NotFound();
            }   

            var libroDto = mapper.Map<LibroPatchDTO>(libroDb);

            patchDocument.ApplyTo(libroDto, ModelState);

            var esValido = TryValidateModel(libroDto);

            if(!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDto, libroDb);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "borrarLibro")] // api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

             
    }


}

