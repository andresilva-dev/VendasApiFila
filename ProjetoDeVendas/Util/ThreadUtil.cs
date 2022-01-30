using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Util
{
    public class ThreadUtil
    {
        public static Thread IniciarThread(ThreadStart ts)
        {
            return IniciarThreads(ts, 1, ThreadPriority.Highest)[0];
        }

        public static List<Thread> IniciarThreads(ThreadStart ts, int numero, ThreadPriority tp)
        {
            var lista = new List<Thread>();
            for (int i = 1; i <= numero; i++)
            {
                void tsTratada()
                {
                    try
                    {
                        ts();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ArquivoUtil.SalvarLogErro(e, null);
                    }
                }

                var t = new Thread(tsTratada) { Priority = tp };
                t.Start();
                lista.Add(t);
            }

            return lista;
        }

        public static bool ObterUltimos<T>(List<T> lista, int quantidade, out List<T> valores)
        {
            lock (lista)
            {
                if (lista.Count() > 0)
                {
                    valores = new List<T>();

                    for (int i = 0; i < quantidade; i++)
                    {
                        if (lista.Count() > 0)
                        {
                            int idx = lista.Count() - 1;
                            valores.Add(lista[idx]);
                            lista.RemoveAt(idx);
                        }
                        else
                        {
                            break;
                        }
                    }

                    return true;
                }

                valores = null;

                return false;
            }
        }
    }
}
