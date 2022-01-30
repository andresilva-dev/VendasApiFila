using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace GeradorDeNotasFiscais
{
    class Program
    {
        private static int _contador;
        static void Main(string[] args)
        {
            bool podeIniciar = false;
            while (true)
            {
                try
                {
                    if (!podeIniciar)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                       
                    var itens = ObtenhaXmlsParaGeracao(2);
                    Thread.Sleep(800);
                    CrieEEnvieNotasFiscais(itens);
                    Thread.Sleep(500);
                }
                catch (Exception)
                {
                }
            }
        }

        private static void CrieEEnvieNotasFiscais(Dictionary<long, string> itens)
        {
            var listaDeNotasFiscaisGeradas = new Dictionary<long, int>();
            foreach (var item in itens)
            {
                _contador++;
                listaDeNotasFiscaisGeradas.Add(item.Key, Convert.ToInt32($"{DateTime.Now.Year}{_contador}"));
            }

            var url = "http://localhost:9696/api/PersistaNotasFiscais";
            ValidarResultadoRequestOk(HttpMethod.Post, url, listaDeNotasFiscaisGeradas);
        }

        private static Dictionary<long, string> ObtenhaXmlsParaGeracao(int quantidade)
        {
            var url = "http://localhost:9696/api/ObtenhaLoteDeXmlsParaGeracao/" + quantidade;

            var resultado = ObterResultadoRequest(HttpMethod.Get, url);

            var itens = new Dictionary<long, string>();
            if (resultado != null)
            {
                itens = JsonConvert.DeserializeObject<Dictionary<long, string>>(resultado);
            }

            return itens;
        }

        private static void ValidarResultadoRequestOk(HttpMethod metodo, string url, object conteudo)
        {
            var resultado = ObterResultadoRequest(metodo, url, conteudo);

            if (!resultado.Equals("\"OK\""))
            {
                throw new Exception($"Erro {resultado}: {url}");
            }
        }
        private static string ObterResultadoRequest(HttpMethod metodo, string url, object conteudo)
        {
            HttpClient clienteHttp = new HttpClient();
            clienteHttp.Timeout = TimeSpan.FromMinutes(10);
            var json = JsonConvert.SerializeObject(conteudo);
            var request = new HttpRequestMessage(metodo, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            var response = clienteHttp.SendAsync(request).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Erro {response.StatusCode}: {url}");
            }

            var resultado = response.Content.ReadAsStringAsync().Result;

            return resultado;
        }

        private static string ObterResultadoRequest(HttpMethod metodo, string url)
        {
            HttpClient clienteHttp = new HttpClient();
            clienteHttp.Timeout = TimeSpan.FromMinutes(10);
            var request = new HttpRequestMessage(metodo, url);
            var response = clienteHttp.SendAsync(request).Result;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Erro {response.StatusCode}: {url}");
            }

            var resultado = response.Content.ReadAsStringAsync().Result;

            return resultado;
        }
    }
}
