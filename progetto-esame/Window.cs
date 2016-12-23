using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    public class Window : EventArgs
    {
        /*
         * Questa classe è stata creata per risolvere il problema degli eventi.
         * Cosi posso sollevare un evento dalla classe parser nel memento in cui
         * La finestra raggiunge le dimensioni adatte. Di conseguenza questa è la 
         * Classe che Davide chiama Acquisizione. (Cambiare il nome se si vuole)
         * Da capire se è giusto lasciare i metodi i questa classe.
         */
        public List<List<double>> matrice;
        public List<List<double>> matriceSmooth;

        public Window(List<List<List<double>>> m, int sensore)
        {
            matrice = FissaSensore(m, sensore);
            matriceSmooth = Smooth(matrice);//AGGIUNTO
            
        }
        //ATTENZIONE: ALCUNI DEI SEGUENTI METODI VANNO NELLA CLASSE DI ANALISI (ANCORA DA FARE)


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
                result.Add((l[i + h] - l[i]) / h);
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
        public List<double> ModuloAccelerometro(List<List<double>> m)
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
        public List<double> ModuloGiroscopio(List<List<double>> m)
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
           
            int k = 10; // Da specifiche di progetto k=10
            /*Smooth su una finestra più piccola(da k a nRighe-k)
            *Idea di aggiornare la finestra di continuo
            */
            int i;/*
            for ( i = 0; i < k; i++)
            {
                result.Add(m[i]);
            }*/
            
            for ( i = k; i < nRighe-k; i++)
            {
                List<double> media = Media(m.GetRange(i-k, 2*k));
                //aggiungi il vettore media in posizione i
                result.Add(media);
            }
            /*
            for (; i < nRighe; i++)
            {
                result.Add(m[i]);
            }*/
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


        private List<double> DeviazioneStandard(List<double> l) /* MIA */
        {
            List<double> result = new List<double>(l.Count());
            List<double> appoggio;
            int t = 10; // dimensione finestra
            int s = 0, e = 0;
            double sum = 0, media = 0;

            for (int i = 0; i < l.Count(); i++)
            {
                sum = 0;
                s = i - t;
                if (s < 0)
                    s = 0;
                e = i + t;
                if (e >= l.Count())
                    e = l.Count() - 1;
                appoggio = l.GetRange(s, e);

                for(int j = 0; j < appoggio.Count(); j++)
                {
                    media = Media(appoggio);
                    sum += (appoggio[j] - media) * (appoggio[j] - media);
                }

                result[i] = Math.Sqrt(sum / l.Count);
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
