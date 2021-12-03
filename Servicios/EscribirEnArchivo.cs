using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiWeb.Servicios
{
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Arcgivo 1.txt";
        private Timer timer;

        public EscribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            Escribir("Proceso Iniciado");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Escribir("Proceso Finalizado");
            return Task.CompletedTask;
        }
        public void DoWork(object state)
        {
            Escribir("Proceso en ejecucion" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }



        private void Escribir(string mensjae)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensjae);
            }
        }
    }
}