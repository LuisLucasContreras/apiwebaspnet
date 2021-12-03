using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.DTOs;
using ApiWeb.Entidades;
using AutoMapper;


namespace ApiWeb.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDTOConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, options => options.MapFrom(MapAutorDTOLIbros));

            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, optiones => optiones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
            CreateMap<Libro, LibroDTOCOnAutores>()
                .ForMember(libroDTO => libroDTO.Autores, options => options.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPatchDTO, Libro>().ReverseMap();

            CreateMap<ComentarioCreacionDTO,Comentario>();
            CreateMap<Comentario,ComentarioDTO>();
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();

            if(libroCreacionDTO.AutoresIds == null) {return resultado;}

            foreach (var autorId in libroCreacionDTO.AutoresIds)
            {
                resultado.Add(new AutorLibro() {AutorId = autorId});
            }

            return resultado;
        }

        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var resultado = new List<AutorDTO>();

            if(libro.AutoresLibros == null) {return resultado;}

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            } 

            return resultado;
            
        }

        private List<LibroDTO> MapAutorDTOLIbros (Autor autor, AutorDTO autorDTO)
        {
            var resultado = new List<LibroDTO>();

            if(autor.AutoresLibros == null) {return resultado;}

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTO()
                {
                    Id = autorLibro.AutorId,
                    Titulo = autorLibro.Libro.Titulo
                });
            } 

            return resultado;
        }
    }
}