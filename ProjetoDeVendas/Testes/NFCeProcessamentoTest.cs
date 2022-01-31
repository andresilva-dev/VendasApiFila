using Entidades;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Testes
{
    [TestClass]
    public class NFCeProcessamentoTest
    {
        [TestMethod]
        public void Deve_disparar_excecao_caso_o_conteudo_informado_seja_invalido()
        {
            var conteudo = "";

            Assert.ThrowsException<Exception>(() => NFCeFactory.Crie(conteudo));
        }

        [TestMethod]
        public void Deve_disparar_excecao_caso_haja_tentativa_de_atribuicao_de_data_invalida()
        {
            var conteudo = "<?xmlversion=\"1.0\"><nfce><itemid=\"1\"><Descricao>Cadeira</Descricao></item></nfce>";

            var nfceProcessamento = NFCeFactory.Crie(conteudo);
            Assert.ThrowsException<Exception>(() => nfceProcessamento.DataObtencaoParaProcessamento = null);
        }

        [TestMethod]
        public void Deve_disparar_excecao_caso_haja_tentativa_de_atribuicao_de_numero_de_nota_fiscal_invalido()
        {
            var conteudo = "<?xmlversion=\"1.0\"><nfce><itemid=\"1\"><Descricao>Cadeira</Descricao></item></nfce>";

            var nfceProcessamento = NFCeFactory.Crie(conteudo);
            Assert.ThrowsException<Exception>(() => nfceProcessamento.NumeroNFCe = null);

            Assert.ThrowsException<Exception>(() => nfceProcessamento.NumeroNFCe = -5);

            Assert.ThrowsException<Exception>(() => nfceProcessamento.NumeroNFCe =  201515);
        }

        [TestMethod]
        public void Deve_gerar_objeto_caso_as_informcaoes_estejam_validas()
        {
            var conteudo = "<?xmlversion=\"1.0\"><nfce><itemid=\"1\"><Descricao>Cadeira</Descricao></item></nfce>";

            var nfceProcessamento = NFCeFactory.Crie(conteudo);
            nfceProcessamento.DataObtencaoParaProcessamento = DateTime.Now;
            nfceProcessamento.NumeroNFCe = Convert.ToInt32($"{DateTime.Now.Year}{31}");

            Assert.ThrowsException<Exception>(() => nfceProcessamento.DataObtencaoParaProcessamento = null);
        }
    }
}
