using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Util
{
    public class ArquivoUtil
    {
        public static void SalvarLogErro(Exception e, string textoAdicionar)
        {
            var sb = new StringBuilder();
            sb.Append("Data: ").Append(ObtenhaStringFormatadaParaDiaMesAnoHoraMinuto(DateTime.Now)).Append('\n');
            if (textoAdicionar != null)
            {
                sb.Append(textoAdicionar).Append('\n');
            }

            sb.Append("TMP_").Append(e.ToString()).Append('\n');
            sb.Append('\n');
            SalvarLogErro(sb.ToString());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void SalvarLogErro(string texto)
        {
            string arquivo = ObterCaminhoArquivoLogErro();
            SalvarArquivo(arquivo, texto, FileMode.Append);
        }

        public static string ObterCaminhoArquivoLogErro()
        {
            return ObterDiretorioAplicacao() + Path.DirectorySeparatorChar + "Log.txt";
        }

        public static string ObterDiretorioAplicacao()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        private static string ObtenhaStringFormatadaParaDiaMesAnoHoraMinuto(DateTime data)
        {
            string shora = data.Hour > 9 ? data.Hour.ToString() : ("0" + data.Hour);
            string sminuto = data.Minute > 9 ? data.Minute.ToString() : ("0" + data.Minute);

            return ToDiaMesAno(data) + " " + shora + ":" + sminuto;
        }

        private static string ToMesAno(DateTime data)
        {
            int mes = data.Month;
            string smes = mes > 9 ? mes.ToString() : ("0" + mes);

            return smes + "/" + data.Year;
        }

        private static string ToDiaMesAno(DateTime data)
        {
            int dia = data.Day;
            string sdia = dia > 9 ? dia.ToString() : ("0" + dia);

            return sdia + "/" + ToMesAno(data);
        }

        public static void SalvarArquivo(string arquivo, string texto, FileMode fm)
        {
            using (FileStream fs = new FileStream(arquivo, fm))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    sw.Write(texto);
                    sw.Flush();
                }
            }
        }
      
    }
}