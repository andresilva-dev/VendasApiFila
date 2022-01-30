using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Util;

namespace ProjetoDeVendas
{
    public class FilaNFCe
    {
        private List<NFCeProcessamento> _listaDeXmlsRecebidos = new List<NFCeProcessamento>();
        private List<NFCeProcessamento> _listaDeXmlsEmProcessamento = new List<NFCeProcessamento>();
        private List<NFCeProcessamento> _listaDeXmlsProcessados = new List<NFCeProcessamento>();
        public FilaNFCe()
        {
            ThreadUtil.IniciarThread(VerifiqueItensNaoProcessados);
        }

        public long AdicioneXmlParaProcessamento(string xmlEnviado) 
        {
            var protocolo = DateTime.Now.Ticks;

            var nfceProcessamento = new NFCeProcessamento(protocolo, xmlEnviado);

            lock (_listaDeXmlsRecebidos)
            {
                _listaDeXmlsRecebidos.Add(nfceProcessamento);
            }
            
            return protocolo;
        }

        public string ObtenhaNumeroNFCE(long protocolo)
        {
            string retorno = "Não existe uma nota fiscal com este protocolo";

            if(_listaDeXmlsRecebidos.Any(n => n.Protocolo == protocolo) || _listaDeXmlsEmProcessamento.Any(n => n.Protocolo == protocolo))
                retorno = "Nota fiscal ainda não processada";

            var nfceGerada = _listaDeXmlsProcessados.FirstOrDefault(n => n.Protocolo == protocolo);
            
            if (nfceGerada != null)
            {
                retorno = nfceGerada.NumeroNFCe.ToString();
                lock (_listaDeXmlsProcessados)
                {
                    _listaDeXmlsProcessados.Remove(nfceGerada);
                }
            }
               
            return retorno;
        }

        public Dictionary<long, string> ObtenhaLoteDeXmlsParaProcessamento(int lote)
        {
            ThreadUtil.ObterUltimos(_listaDeXmlsRecebidos, lote, out List<NFCeProcessamento> dados);

            if (dados == null)
                return new Dictionary<long, string>();

            lock (_listaDeXmlsEmProcessamento)
            {
                foreach (NFCeProcessamento dado in dados)
                {
                    dado.DataObtencaoParaProcessamento = DateTime.Now;
                    _listaDeXmlsEmProcessamento.Add(dado);
                }
            }

            var conteudoObtido = _listaDeXmlsEmProcessamento.ToDictionary(i => i.Protocolo, i => i.Conteudo);

            return conteudoObtido;
        }

        public void PersistaXmlsProcessados(Dictionary<long, int> notasGeradas)
        {
            lock (_listaDeXmlsProcessados)
            {
                foreach (var notaFiscal in notasGeradas) 
                {
                    var item = _listaDeXmlsEmProcessamento.First(i => i.Protocolo == notaFiscal.Key);
                    item.NumeroNFCe = notaFiscal.Value;
                    
                    _listaDeXmlsProcessados.Add(item);
                }
            }

            lock (_listaDeXmlsEmProcessamento)
            {
                _listaDeXmlsEmProcessamento.RemoveAll(x => notasGeradas.Keys.Contains(x.Protocolo));
            }
        }

        public virtual void RegistrarLog()
        {
            while (true)
            {
                Thread.Sleep(3000);
                try
                {
                    Console.WriteLine("########## LOG ############");

                    StringBuilder sb = new StringBuilder();
                    sb.Append(DateTime.Now.ToString() + "\n");
                    sb.Append("Qtd de xmls na Fila: " + _listaDeXmlsRecebidos.Count + "\n");
                    sb.Append("Itens em processamento: " + _listaDeXmlsEmProcessamento.Count + " | " + " Processados: " + _listaDeXmlsProcessados.Count +"\n");

                    sb.Append("Protocolos de itens processados: \n");
                    lock (_listaDeXmlsProcessados)
                    {
                        int contador = 0;
                        foreach (var item in _listaDeXmlsProcessados)
                        {
                            sb.Append(item.Protocolo + "\n");
                            contador++;
                            if (contador == 5)
                            {
                                break;
                            }
                        }
                    }

                    Console.WriteLine(sb.ToString());
                }
                catch (Exception)
                { }
            }
        }

        private void VerifiqueItensNaoProcessados()
        {
            while (true)
            {
                Thread.Sleep(10000);
                lock (_listaDeXmlsEmProcessamento)
                {
                    var listaRemover = new List<long>();
                    foreach (var item in _listaDeXmlsEmProcessamento)
                    {
                        if (!item.DataObtencaoParaProcessamento.HasValue)
                            continue;

                        DateTime inicio = item.DataObtencaoParaProcessamento.Value;
                        long minutos = ObtenhaDiferencaHoras(DateTime.Now, inicio);
                        if (minutos >= 20)
                        {
                            listaRemover.Add(item.Protocolo);
                        }
                    }

                    _listaDeXmlsEmProcessamento.RemoveAll(m => listaRemover.Contains(m.Protocolo));
                }
            }
        }

        private long ObtenhaDiferencaMs(DateTime maior, DateTime menor)
        {
            long diferenca = maior.Ticks - menor.Ticks;
            return diferenca / TimeSpan.TicksPerMillisecond;
        }

        private long ObtenhaDiferencaHoras(DateTime maior, DateTime menor)
        {
            return ObtenhaDiferencaMs(maior, menor) / 1000 / 60 / 60;
        }
    }
}
