using System.Collections.Immutable;
using System.Text;
using System.Security.Principal;
using ApiWeb.Filtros;
using ApiWeb.Middlewares;
using ApiWeb.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using ApiWeb.Utilidades;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ApiWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones => 
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            } ).AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore).AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));




            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Keyjwt"])),
                    ClockSkew = TimeSpan.Zero
                
            });

            services.AddSwaggerGen( c => 
            {
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme 
                {
                    Name= "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            }
            );

            services.AddAutoMapper(typeof(Startup)); 

            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthorization(options => 
            {
                options.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
            });

            services.AddDataProtection();

            services.AddTransient<HashService>();

            services.AddCors(options => 
            {
                options.AddDefaultPolicy(builder => 
                {
                    builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader().
                    WithExposedHeaders(new string[] {"cantidadTotalRegistros"});
                });
            });

            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            app.UseLoguearRespuestaHTTP(); 



            
                app.UseSwagger();
                app.UseSwaggerUI();
        

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}