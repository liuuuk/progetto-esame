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
            
            Console.WriteLine("Listening...");
            try
            {
                while (true)
                {
                    l.Start(); //Listening
                    Console.WriteLine("Active connections: " + count_client);
                    Console.Write("Waiting for a connection... "); //Status

                    client = l.AcceptTcpClient();
                    count_client++;
                    Console.WriteLine("Connected with " + count_client); //Status

                    Parser p = new Parser(); //Creo il parser
                    //Creo un thread per la form
                    Thread tForm = new Thread(new ParameterizedThreadStart(View));
                    tForm.Start(p); //new Form Passando il parser
                    Thread tConnect = new Thread(new ParameterizedThreadStart(Connect));
                    tConnect.Start(p); //New connection Passando il parser
                    
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Data);
               
            }
        }

        public void View(object sender)
        {
            Parser p = (Parser)sender;
            //preparo la form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 myForm = new Form1();

            //Sottoscrivo l'evento finestra piena del parser al metodo test(che scrive sulle zedgraph)
            p.FinestraPiena += new WindowEventHandler(myForm.Disegna);

            //Lancio la form
            Application.Run(myForm);
        }

        public void Connect(object sender)
        {
            Parser p = (Parser)sender;
            //Console.WriteLine("Connected with " + (string)sender); //Status

            NetworkStream stream = client.GetStream();
            BinaryReader bin = new BinaryReader(stream);
            
            p.Parse(bin);
        }
    }
}
