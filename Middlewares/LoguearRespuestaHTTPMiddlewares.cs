using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWeb.Middlewares
{

    public static class LoguearRespuestaHTTPMiddlewaresExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddlewares>();
        }
    }
    public class LoguearRespuestaHTTPMiddlewares
    {
        private readonly RequestDelegate siguente;
        private readonly ILogger<LoguearRespuestaHTTPMiddlewares> logger;

        public LoguearRespuestaHTTPMiddlewares(RequestDelegate siguente, ILogger<LoguearRespuestaHTTPMiddlewares> logger)
        {
            this.siguente = siguente;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                var original = contexto.Response.Body;
                contexto.Response.Body = ms;

                await siguente(contexto);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(original);
                contexto.Response.Body = original;

                logger.LogInformation(respuesta);
            }
        }
    }
}