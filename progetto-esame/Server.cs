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

namespace progetto_esame
{
    class Server
    {
        #region vars
        Int32 port; //Tcp Port
        IPAddress address; //IP Address
        TcpListener l; 
        TcpClient client;

        Parser p;
        int count_client; //Number of client
        #endregion

        public Server(string address, Int32 port)
        {
            this.port = port;
            this.address = IPAddress.Parse(address);
            this.count_client = 0;

            l = new TcpListener(this.address, this.port);
            p = new Parser();

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

                    Thread t = new Thread(new ParameterizedThreadStart(Connect));
                    t.Start("#" + count_client); //New connection
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Data);
               
            }
        }

        public void Connect(object sender)
        {
            Console.WriteLine("Connected with " + (string)sender); //Status

            NetworkStream stream = client.GetStream();
            BinaryReader bin = new BinaryReader(stream);
            
            p.Parse(bin);
        }
    }
}
