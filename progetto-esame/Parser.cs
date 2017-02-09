using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace progetto_esame
{
    public delegate void WindowEventHandler(object sender, Window e);
    public delegate void InizioEventHandler(object sender, DateTime d);
    public delegate void EventHandler();
    //Classe globale per avere variabili globali
    static class Globals
    {
        public const int dimensioneFinestra = 20;
        public const int kSmooth = 5;
    }
    class Parser
    {

        int maxSensori;
        List<List<double>> array; // salvataggio dati
        List<List<List<double>>> mat;
        public event WindowEventHandler FinestraPiena;
        public event InizioEventHandler TempoPrimaAcquisizione;

        public event EventHandler FineStream;

        public event InfoEventHandler Info;

        private bool primaFinestra = true;

        public Parser()
        {
            maxSensori = 10;

            array = new List<List<double>>(); // Del prof
            mat = new List<List<List<double>>>(); // Aggiunto
        }

        protected virtual void OnTempoPrimaAcquisizione(DateTime d) { if (TempoPrimaAcquisizione != null) TempoPrimaAcquisizione(this, d); }
        protected virtual void OnFinestraPiena(Window e) { if (FinestraPiena != null) FinestraPiena(this, e); }
        protected virtual void OnFineStream(EventArgs e) { if (FineStream != null) FineStream(); }
        protected virtual void OnInfo(InfoEventArgs e) { if (Info != null) Info(this, e); }

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
            int n = 0; //Dimensione della finestra
            while (pacchetto.Length != 0) //finche' ci sono dati
            {
                mat.Add(new List<List<double>>()); //Matrice 3d


                #region reading
                for (int i = 0; i < numSensori; i++)
                {

                    mat[n].Add(new List<double>()); //matrice 3d

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

                        mat[n][i].Add(valore); //matrice 3d

                        t[i] += 4;
                    }
                }
                for (int x = 0; x < numSensori; x++)
                {
                    t[x] = 5 + (52 * x);
                }
                #endregion

                #region output
                /*
                for (int j = 0; j < numSensori; j++)
                {
                    for (int tr = 0; tr < 13; tr++)
                    {
                        // esempio output su console
                        Console.Write(array[j][tr] + "; ");
                    }
                    Console.WriteLine();
                    array[j].RemoveRange(0, 13); // cancellazione dati
                }

                Console.WriteLine();
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

                n++; //incremento per il numero di campioni
                if (n >= Globals.dimensioneFinestra)
                {

                    if (primaFinestra)
                    {
                        OnTempoPrimaAcquisizione(DateTime.Now);
                        primaFinestra = false;
                    }
                    //evento di debug
                    //OnInfo(new InfoEventArgs("Parser. Finestra piena.", true));

                    OnFinestraPiena(new Window(mat, 0)); //Lancia l'evento

                    for (int i = 0; i < (n / 2); i++)
                        mat[i] = mat[i + (n / 2)];
                    mat.RemoveRange(n / 2, n / 2); // cancello la seconda parte della matrice
                    n = n / 2; // leggo solamente i prossimi 250 dati (nelle 250 caselle precedenti ho gli ultimi 250 dati della lettura precedente)

                }
                   
            }

            //Evento di fine stream
            OnFineStream(new EventArgs());


            #endregion
         
        }

        //funzioni di debug
        private void Visualizza(List<List<List<double>>> m)
        {
            int nRighe = m.Count; // 500 Campioni
            int nColonne = m[0].Count; // 5 Sensori
            int nProf = m[0][0].Count; // 13 Dati
            Console.WriteLine("Dimensione (rXcXp):" + nRighe + "x" + nColonne + "x" + nProf);
            
            for (int i = 0; i < nRighe; i++)
            {
                for (int j = 0; j < nColonne; j++)
                {
                    for (int k = 0; k < nProf; k++)
                    {
                        Console.Write(m[i][j][k] + " |"); // Funziona i j k                       
                        //Console.Write(i +" "+ j + " " + k + " | ");

                    }
                    Console.Write("||");
                    Console.WriteLine();
                }
                Console.WriteLine("-------");
            }
        }
        private void Visualizza(List<List<double>> m)
        {
            int nRighe = m.Count;
            int nColonne = m[0].Count;
            Console.WriteLine("Dimensione (rXc):" + nRighe + "x" + nColonne);

            for (int i = 0; i < nRighe; i++)
            {
                for (int j = 0; j < nColonne; j++)
                {
                    Console.Write(m[i][j] + " | ");
                    //Console.Write(i + " " + j + " | ");
                }
                Console.Write("||");
                Console.WriteLine("");
            }

        }
        private void Visualizza(List<double> l)
        {
            int nElementi = l.Count;
            Console.WriteLine("Dimensione: " + nElementi);
            foreach (double item in l)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }
        //Fine funzioni di debug

    }
}
