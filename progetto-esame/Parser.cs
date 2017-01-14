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
    class Parser
    {

        int maxSensori;
        List<List<double>> array; // salvataggio dati
        List<List<List<double>>> mat;
        int dimensioneFinestra;
        public event WindowEventHandler FinestraPiena;
        public event InizioEventHandler TempoPrimaAcquisizione;

        private bool primaFinestra = true;

        public Parser()
        {
            maxSensori = 10;
            dimensioneFinestra = 20;

            array = new List<List<double>>(); // Del prof
            mat = new List<List<List<double>>>(); // Aggiunto
        }

        protected virtual void OnTempoPrimaAcquisizione(DateTime d) { if (TempoPrimaAcquisizione != null) TempoPrimaAcquisizione(this, d); }
        protected virtual void OnFinestraPiena(Window e) { if (FinestraPiena != null) FinestraPiena(this, e); }

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
                if (n >= dimensioneFinestra)
                {

                    if (primaFinestra)
                    {
                        OnTempoPrimaAcquisizione(DateTime.Now);
                        primaFinestra = false;
                    }

                    OnFinestraPiena(new Window(mat, 0)); //Lancia l'evento

                    for (int i = 0; i < (n / 2); i++)
                        mat[i] = mat[i + (n / 2)];
                    mat.RemoveRange(n / 2, n / 2); // cancello la seconda parte della matrice
                    n = n / 2; // leggo solamente i prossimi 250 dati (nelle 250 caselle precedenti ho gli ultimi 250 dati della lettura precedente)

                }
                   
            }
            


            #endregion
         
        }

        //I SEGUENTI METODI SONO STATI SPOSTATI NELLA CLASSE MatriceEventArgs.cs 
        //E' ANCORA DA CAPIRE SE QUELLO SIA IL POSTO GIUSTO

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


        /*
         * FissaSensore: Data una matrice a 3 dimensioni restituisce 
         * la matrice a due dimensioni fissata una colonna (sensore)
         */
        private List<List<double>> FissaSensore(List<List<List<double>>> m, int sensor)
        {
            List<List<double>> result = new List<List<double>>();
            int nRighe = m.Count;
            //Salto le colonne perchè rappresentano il sensore, ed è fissato da parametro
            int nProf = m[0][0].Count;

            for (int i = 0; i < nRighe; i++)
            {
                result.Add(new List<double>());
                for (int k = 0; k < nProf; k++)
                {
                    result[i].Add(m[i][sensor][k]);
                }
            }
            return result;
        }


        /*
         * I metodi seguenti sono pura matematica. E' solo l'implementazione
         * delle formule che si trovano a pagina 7 delle specifiche.
         */
        private double Roll(double q0, double q1, double q2, double q3)
        {
            double numeratore = (2 * q2 * q3) + (2 * q0 * q1);
            double denominatore = (2 * q0 * q0) + (2 * q3 * q3) - 1;
            return Math.Atan(numeratore / denominatore);
        }
        private double Pitch(double q0, double q1, double q2, double q3)
        {
            double arg = (2 * q1 * q3) - (2 * q0 * q2);
            return -Math.Asin(arg);
        }
        private double Yaw(double q0, double q1, double q2, double q3)
        {
            double numeratore = (2 * q1 * q2) + (2 * q0 * q3);
            double denominatore = (2 * q0 * q0) + (2 * q1 * q1) - 1;
            return Math.Atan(numeratore / denominatore);
        }


        /*
         * Deviazione standard
         * Input: Una lista di double
         * Output: La deviazione standard.
         */
        private double DeviazioneStandard(List<double> l)
        {
            double media = Media(l);
            double sum = 0;
            foreach (double item in l)
            {
                sum += (item - media) * (item - media);
            }
            return Math.Sqrt(sum / l.Count);
        }

        /*
         * RIFunc (Rapporto incrementale)
         * Input: Una lista di double.
         * Output: Una lista di n-1 double. Fissato di default h = 1.
         */
        private List<double> RIFunc(List<double> l)
        {
            List<double> result = new List<double>();
            int nElementi = l.Count;
            int h = 1;
            for (int i = 0; i < nElementi - 1; i++)
            {
                result.Add((l[i + h] - l[i])/h);
            }
            return result;
        }

        /*
         * Modulo
         * Input: Tre double che rappresentano x, y e z
         * Output: La radice quadrata della somma dei tre quadrati.
         */
        private double Modulo(double x, double y, double z)
        {
            double sum = 0.0;
            sum = (x * x) + (y * y) + (z * z);
            return Math.Sqrt(sum);
        }


        /*
         * ModuloAccelerometro
         * Input: Una lista di lista di double. (Rappresenta i dati di un sensore nel tempo)
         * Output: Una lista di double lunga N, dove N rappresenta il numero di istanti di tempo.
         *         In posizione i si trova il modulo dell'accelerometro all'istante i.
         */
        private List<double> ModuloAccelerometro(List<List<double>> m)
        {
            List<double> result = new List<double>();
            int nRighe = m.Count; //nCampioni
            for (int i = 0; i < nRighe; i++)
            {
                //Passo il primo il secondo e il terzo elemento della i-esima colonna
                result.Add(Modulo(m[i][0], m[i][1], m[i][2]));
            }

            return result;
        }


        /*
         * ModuloGiroscopio
         * Input: Una lista di lista di double. (Rappresenta i dati di un sensore nel tempo)
         * Output: Una lista di double lunga N, dove N rappresenta il numero di istanti di tempo.
         *         In posizione i si trova il modulo del giroscopio all'istante i.
         */
        private List<double> ModuloGiroscopio(List<List<double>> m)
        {
            List<double> result = new List<double>();
            int nRighe = m.Count; //nCampioni
            for (int i = 0; i < nRighe; i++)
            {
                //Passo il quarto il quinto e il sesto elemento della i-esima colonna
                result.Add(Modulo(m[i][3], m[i][4], m[i][5]));
            }

            return result;
        }


        /*
         * Smooth
         * Input: Lista di lista di double. (Rappresenta i dati di un sensore nel tempo)
         * Output: Una lista di lista di double, nella quale ogni riga è data
         *         dalla media di 2k+1 vettori riga. Da specifiche k = 10
         */
        private List<List<double>> Smooth(List<List<double>> m)
        {
            List<List<double>> result = new List<List<double>>();
            int nRighe = m.Count;
            int nColonne = m[0].Count;
            int k = 10; // Da specifiche di progetto k=10
            //Per ogni riga
            for (int i = 0; i < nRighe; i++)
            {
                //Calcola gli indici
                int start, end;
                if (i - k < 0)
                    start = 0;
                else
                    start = i - k;
                if (i + k > nRighe)
                    end = nRighe;
                else
                    end = i + k;
                
                //Calcola la media sulla sotto matrice di righe da start ad end
                List<double> media = Media(m.GetRange(start, end-start));

                //aggiungi il vettore media in posizione i
                result.Add(media);
            }

            return result;
        }


        /*
         * Media
         * Input: Lista di Lista di double. (Rappresenta i dati di un sensore nel tempo)
         * Output: Una lista lunga il numero di colonne dell'input.
         *         Nella posizione i della lista di ritorno è presente la media
         *         Del vettore colonna i-esimo.
         */
        private List<double> Media(List<List<double>> m)
        {
            // si potrebbe sfruttare la Media su Lista di double per effettuare la media su matrice
            int nRighe = m.Count;
            int nColonne = m[0].Count;

            List<double> result = new List<double>();
            for (int i = 0; i < nColonne; i++)
            {
                result.Add(0);
            }
            for (int i = 0; i < nColonne; i++)
            {
                
                for (int j = 0; j < nRighe; j++)
                {
                    result[i] += m[j][i];
                }
            }
            for (int i = 0; i < nColonne; i++)
            {
                result[i] = result[i] / nRighe;
            }
            return result;
        }


        /*
         * Media
         * Input: Lista di double
         * Output: La media dei valori contenuti nella lista.
         */
        private double Media(List<double> l)
        {
            double sum = 0;
            foreach (double item in l)
            {
                sum += item;
            }
            return sum / l.Count;
        }

    }
}
