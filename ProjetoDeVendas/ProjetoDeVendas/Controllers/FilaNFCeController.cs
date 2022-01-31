using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoDeVendas.Controllers
{
    public class FilaNFCeController : WebApiController
    {
        [Route(HttpVerbs.Get, "/ObtenhaNFCeGerada/{protocolo}")]
        public async Task<string> ObtenhaNFCeGerada(long protocolo)
        {
            return await Task.Run(() => ObtenhaNFCe(protocolo));
        }

        private string ObtenhaNFCe(long protocolo)
        {
            var numeroNFCe = Program.ObtenhaFilaEmExecucao().ObtenhaNumeroNFCeGerada(protocolo);
            return numeroNFCe;
        }

        [Route(HttpVerbs.Post, "/GereNFCe/{conteudo}")]
        public async Task<long> GereNFCe(string conteudo)
        {
            return await Task.Run(() => Gere(conteudo));
        }

        private long Gere(string conteudo)
        {
            var protocolo = Program.ObtenhaFilaEmExecucao().AdicioneXmlParaProcessamento(conteudo);
            
            return protocolo;
        }

        [Route(HttpVerbs.Post, "/PersistaNotasFiscais")]
        public async Task<string> PersistaNotasFiscais()
        {
            var bytes = LeiaBytes(Request.InputStream);

            return await Task.Run(() => PersistaXmlsProcessados(bytes));
        }

        private string PersistaXmlsProcessados(byte[] bytes)
        {
            var conteudo = Encoding.UTF8.GetString(bytes);
            var dados = JsonConvert.DeserializeObject<Dictionary<long,int>>(conteudo, ObterConfiguracao());

            Program.ObtenhaFilaEmExecucao().PersistaXmlsProcessados(dados);

            return "OK";
        }

        [Route(HttpVerbs.Get, "/ObtenhaLoteDeXmlsParaGeracao/{lote}")]
        public async Task<Dictionary<long, string>> ObtenhaNFCeGerada(int lote)
        {
            return await Task.Run(() => ObtenhaLoteDeXmls(lote));
        }

        private Dictionary<long, string> ObtenhaLoteDeXmls(int lote)
        {
            var itens = Program.ObtenhaFilaEmExecucao().ObtenhaLoteDeXmlsParaProcessamento(lote);
            return itens;
        }

        public static byte[] LeiaBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static JsonSerializerSettings ObterConfiguracao()
        {
            return new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
