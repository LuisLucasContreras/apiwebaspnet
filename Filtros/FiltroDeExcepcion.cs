using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ApiWeb.Filtros
{
    public class FiltroDeExcepcion : ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroDeExcepcion> logger;

        public FiltroDeExcepcion(ILogger<FiltroDeExcepcion> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }
    }
}