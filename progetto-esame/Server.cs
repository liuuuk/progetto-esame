using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace progetto_esame
{
    class Server
    {
        #region vars
        Int32 port; //Tcp Port
        IPAddress address; //IP Address
        TcpListener l; 
        TcpClient client;

        //Parser p;
        int count_client; //Number of client
        #endregion

        public Server(string address, Int32 port)
        {
            this.port = port;
            this.address = IPAddress.Parse(address);
            this.count_client = 0;

            l = new TcpListener(this.address, this.port);
            //p = new Parser();

            Console.WriteLine("Server Created.");
        }

        public void Listen(object caller)
        {  
            Console.WriteLine("Listening..."); //Status
            try
            {
                while (true)
                {
                    l.Start(); //Listening
                    Console.WriteLine("Active connections: " + count_client); //Status
                    Console.Write("Waiting for a connection... "); //Status

                    client = l.AcceptTcpClient();
                    count_client++;
                    Console.WriteLine("Connected with " + count_client); //Status

                    Parser p = new Parser(); //Creo il parser

                    //Creo un thread per la form (VIEW)
                    Thread tForm = new Thread(new ParameterizedThreadStart(View));
                    tForm.Start(p); //new Form passando il parser

                    //Creo un thread per la connessione (PARSING)
                    Thread tConnect = new Thread(new ParameterizedThreadStart(Connect));
                    tConnect.Start(p); //New connection passando il parser
                    
                    //Creo un thread per l'analisi
                    //Codice...
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Data);
            }
        }

        /*
         * View è il thread responsabile della visualizzazione dei dati.
         * Si occupa di creare la Form la quale gestisce tutta la grafica.
         */
        public void View(object sender)
        {
            Parser p = (Parser)sender;

            //preparo la form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 myForm = new Form1();

            string fileName = "LOG_" + DateTime.Now.ToString().Replace(":", "_").Replace("/", "-") + ".txt";

            //Creo un oggetto Analisi
            Analyzer a = new Analyzer(fileName);

            p.FinestraPiena += new WindowEventHandler(a.Run);
            p.TempoPrimaAcquisizione += new InizioEventHandler(a.SetStart);
            p.Info += new InfoEventHandler(myForm.TextLog);

            //Sottoscrivo i vari eventi generati dall'analisi dei dati
            a.GirataDestra += new EventHandler(myForm.DisegnaGirataDestra);
            a.GirataSinistra += new EventHandler(myForm.DisegnaGirataSinistra);

            a.Lay += new EventHandler(myForm.DisegnaLay);
            a.LaySit += new EventHandler(myForm.DisegnaLaySit);
            a.Stand += new EventHandler(myForm.DisegnaStand);
            a.Sit += new EventHandler(myForm.DisegnaSit);

            a.Moto += new EventHandler(myForm.DisegnaMoto);
            a.Stazionamento += new EventHandler(myForm.DisegnaStazionamento);

            a.Info += new InfoEventHandler(myForm.TextLog);

            //Sottoscrivo l'evento finestra piena del parser al metodo test(che scrive sulle zedgraph)
            p.FinestraPiena += new WindowEventHandler(myForm.DisegnaGrafici);

            //Lancio la form
            Application.Run(myForm);
        }

        /*
         * Connect è il thread responsabile del parsing dei dati.
         */
        public void Connect(object sender)
        {
            Parser p = (Parser)sender;

            NetworkStream stream = client.GetStream();
            BinaryReader bin = new BinaryReader(stream);
            
            p.Parse(bin);
        }
    }
}
