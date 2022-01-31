using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public static class NFCeFactory
    {
        public static NFCeProcessamento Crie(string conteudo)
        {
            var protocolo = DateTime.Now.Ticks;

            return new NFCeProcessamento(protocolo, conteudo);
        }
    }
}
