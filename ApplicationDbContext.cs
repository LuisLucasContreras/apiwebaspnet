using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiWeb.Entidades;

namespace ApiWeb
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<AutorLibro>().HasKey(al => new {al.AutorId, al.LibroId});
        }
        public DbSet<Autor>  Autores {get; set;}
        public DbSet<Libro> Libros  {get; set;}
        public DbSet<Comentario> Comentarios {get; set;}
        public DbSet<AutorLibro> AutoresLibros {get;set;}
    }
}

