using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using ProjetoDeVendas.Controllers;
using Swan.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoDeVendas
{
    class Program
    {
        private static FilaNFCe _filaNFCe;
        static void Main(string[] args)
        {
            _filaNFCe = new FilaNFCe();
            var taskAPI = Task.Factory.StartNew(() => IniciarServidorWeb());
            var taskLog = Task.Factory.StartNew(() => RegistrarLogsNFe());
            
            Task.WaitAll(taskAPI, taskLog);
        }

        public static FilaNFCe ObtenhaFilaEmExecucao() 
        {
            return _filaNFCe;
        }

        private static void RegistrarLogsNFe()
        {
            Console.WriteLine("#################### FILA DE NOTAS FISCAIS ####################");
            _filaNFCe.RegistrarLog();
            Console.WriteLine("");
        }
        private static void IniciarServidorWeb()
        {
            var url = "http://localhost:9696/";
            var server = CreateWebServer(url);
            server.RunAsync().Wait();
        }

        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // First, we will configure our web server by adding Modules.
                .WithLocalSessionManager()
                .WithWebApi("/api", m => m
                    .WithController<FilaNFCeController>())
                    .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

            // Listen for state changes.
            server.StateChanged += (s, e) => $"WebServer Novo Estado - {e.NewState}".Info();

            return server;
        }
    }
}
