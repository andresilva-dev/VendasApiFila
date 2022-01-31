using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class NFCeProcessamento
    {
        private long _protocolo;
        private string _conteudo;
        private DateTime? _dataObtencaoParaProcessamento;
        private int? _numeroNFCe;

        public NFCeProcessamento(long protocolo, string conteudo)
        {
            if (protocolo <= 0)
            {
                throw new Exception("Protocolo inválido.");
            }

            if (string.IsNullOrWhiteSpace(conteudo))
            {
                throw new Exception("O conteúdo informado para geração nota fiscal é inválido.");
            }

            _protocolo = protocolo;
            _conteudo = conteudo;
        }

        public long Protocolo => _protocolo;
        public string Conteudo => _conteudo;
        public DateTime? DataObtencaoParaProcessamento
        {
            get => _dataObtencaoParaProcessamento;
            set
            {
                if (value == null)
                {
                    throw new Exception("A data informada para o início do processamento não pode ser nula.");
                }

                _dataObtencaoParaProcessamento = value;
            }
        }
        public int? NumeroNFCe
        {
            get => _numeroNFCe;
            set
            {
                if (value == null)
                {
                    throw new Exception("O número da nota fiscal gerada não pode ser nulo.");
                }

                if (value <= 0)
                {
                    throw new Exception("O número da nota fiscal gerada não menor ou igual a 0.");
                }

                var ano = DateTime.Now.Year.ToString();

                if (!value.ToString().StartsWith(ano))
                {
                    throw new Exception("Valor inválido, o número informado para nota fiscal deve iniciar com o ano corrente.");
                }

                _numeroNFCe = value;
            }
        }

        public override bool Equals(object obj)
        {
            return (obj is NFCeProcessamento objetoNFCe) && objetoNFCe.Protocolo == Protocolo;
        }

        public override string ToString()
        {
            return $"NFCeProcessamento (protocolo): {Protocolo}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
