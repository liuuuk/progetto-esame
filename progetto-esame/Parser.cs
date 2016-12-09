using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Parser
    {

        int maxSensori;

        List<List<double>> array = new List<List<double>>(); // salvataggio dati

        Window sampWin = new Window();
       
        
        

        public Parser()
        {
            maxSensori = 10;
            
        }

        public void Parse(BinaryReader bin)
        {
            #region Parser
            byte[] readd = bin.ReadBytes(1);

            #region init
            int byteToRead;
            byte[] len = new byte[2];
            byte[] tem = new byte[3];

            while (!(tem[0] == 0xFF && tem[1] == 0x32)) // cerca la sequenza FF-32
            {
                tem[0] = tem[1];
                tem[1] = tem[2];
                byte[] read = bin.ReadBytes(1);
                tem[2] = read[0];
            }
            if (tem[2] != 0xFF) // modalità normale
            {
                byteToRead = tem[2]; // byte da leggere
            }
            else  // modalità extended-length
            {
                len = new byte[2];
                len = bin.ReadBytes(2);
                byteToRead = (len[0] * 256) + len[1]; // byte da leggere
            }

            byte[] data = new byte[byteToRead + 1];
            data = bin.ReadBytes(byteToRead + 1); // lettura dei dati

            byte[] pacchetto;

            if (tem[2] != 0xFF)
            {
                pacchetto = new byte[byteToRead + 4]; // creazione pacchetto
            }
            else
            {
                pacchetto = new byte[byteToRead + 6];
            }
            int numSensori = (byteToRead - 2) / 52; // calcolo del numero di sensori
            pacchetto[0] = 0xFF; // copia dei primi elementi
            pacchetto[1] = 0x32;
            pacchetto[2] = tem[2];

            if (tem[2] != 0xFF)
            {
                data.CopyTo(pacchetto, 3); // copia dei dati
            }
            else
            {
                pacchetto[3] = len[0];
                pacchetto[4] = len[1];
                data.CopyTo(pacchetto, 5); // copia dei dati
            }


            int[] t = new int[maxSensori];

            for (int x = 0; x < numSensori; x++)
            {
                array.Add(new List<double>()); // una lista per ogni sensore
                
                t[x] = 5 + (52 * x);
            } 
            #endregion

            float valore;
            int dim = 0;
            while (dim < 500)
            {
                #region reading
                List<Sensor> l = new List<Sensor>();
                for (int i = 0; i < numSensori; i++)
                {
                    byte[] temp = new byte[4];
                   
                    for (int tr = 0; tr < 13; tr++)// 13 campi, 3 * 3 + 4
                    {
                        if (numSensori < 5)
                        {
                            temp[0] = pacchetto[t[i] + 3]; // lettura inversa
                            temp[1] = pacchetto[t[i] + 2];
                            temp[2] = pacchetto[t[i] + 1];
                            temp[3] = pacchetto[t[i]];
                        }
                        else
                        {
                            temp[0] = pacchetto[t[i] + 5];
                            temp[1] = pacchetto[t[i] + 4];
                            temp[2] = pacchetto[t[i] + 3];
                            temp[3] = pacchetto[t[i] + 2];
                        }
                        valore = BitConverter.ToSingle(temp, 0); // conversione
                        array[i].Add(valore); // memorizzazione

                        t[i] += 4;


                    }
                    l.Add(new Instant(array[i])); //AGGIUNGO ALLA MIA LISTA
                }
                sampwin.Add(l); //Per il tempo
                for (int x = 0; x < numSensori; x++)
                {
                    t[x] = 5 + (52 * x);
                }
                #endregion

                #region output


                
                /* Stampo
                for (int j = 0; j < numSensori; j++)
                {
                    Console.WriteLine("Sensor: #" + j + " " + l[j].ToString());
                    Console.WriteLine();
                    array[j].RemoveRange(0, 13); // cancellazione dati
                }
                Console.Write("Dimensione " + dim);
                Console.WriteLine("");
                */

                #endregion

                #region next-data
                if (numSensori < 5) // lettura pacchetto seguente
                {
                    pacchetto = bin.ReadBytes(byteToRead + 4);
                }
                else
                {
                    pacchetto = bin.ReadBytes(byteToRead + 6);
                }
                #endregion
               
                dim++;
                
            }
            #endregion
            Console.WriteLine("First Window sent.");
        }
    }
}
