using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace progetto_esame
{
    static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Server s = new Server("127.0.0.1", 45555); //Creo il server
            Thread t = new Thread(new ParameterizedThreadStart(s.Listen)); //Preparo il thread
            t.Start("Server");//lancio il thread
        }
    }
}
