using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Window
    {
        List<Instant> w;
        public Window()
        {
            w = new List<Instant>();
        }
        
        public Instant GetInstant(int i)
        {
            return w[i];
        }

        public void Add(Instant i)
        {
            w.Add(i);
        }

        public override string ToString()
        {
            string s = "";

            foreach (var item in w)
            {
                s +=  item.ToString();
            }

            return s;
        }

        public List<double> GetModAcc(int sensor)
        {
            List<double> result = new List<double>();
            foreach (var item in w)
            {
                result.Add(item.GetSensor(sensor).GetModuloAcc());
            }

            return result;
        }

        public List<double> GetModGyro(int sensor)
        {
            List<double> result = new List<double>();
            foreach (var item in w)
            {
                result.Add(item.GetSensor(sensor).GetModuloGyro());
            }

            return result;
        }

        public void Smooth(int k)
        {

            Window smooth = new Window();
            for (int i = 0; i < w.Count; i++)
            {
                int end, start;
                if (i - k < 0)
                    start = 0;
                else
                    start = i - k;
                if (i + k > w.Count)
                    end = w.Count;
                else
                    end = i + k;
                for (int j = start; j <= end; j++)
                {
                    smooth.Add(GetInstant(j));
                }
                w[i] = smooth.Mean();
            }
        }


        private Instant Mean()
        {
            int numDati = 13; //9 o 13 dipende se si usano i quaternioni
            Instant m = GetInstant(0);
            List<List<double>> tmp = new List<List<double>>();

            tmp = m.InstantToMatrix();

            for (int i = 1; i < w.Count; i++) //Per ogni istante
            {
                for (int j = 0; j < GetInstant(i).Count(); j++) //Per ogni sensore
                {
                    for (int k = 0; k < numDati; k++) //Per ogni dato
                    {
                        tmp[j][k] += GetInstant(i).GetSensor(j).GetValue(k);
                    }

                }
            }
            for (int k = 0; k < numDati; k++) //Per ogni valore
            {

                for (int j = 0; j < tmp.GetLength(0); j++) //Per ogni sensore
                {
                    tmp[j][k] = m.GetSensor(j).GetValue(k) / (w.Count);
                }

            }

            //convertire la matrice in Instant e ritornarlo
            Instant r = new Instant(tmp);

            return r; 
        }

        public List<double> ModAcc(int s)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < w.Count; i++)
            {
                result.Add(GetInstant(i).GetSensor(s).GetModuloAcc());
            }
            return result;
        }

        public List<double> ModGyro(int s)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < w.Count; i++)
            {
                result.Add(GetInstant(i).GetSensor(s).GetModuloGyro());
            }
            return result;
        }

        //rapporto incrementale per calcolo derivata funzione 3
        //per ogni punto i si calcola il rapporto incrementale tra i e i+1 
        //e lo si scrive in un vettore RI[] nel posto i
        //???DILEMMA ????
        //C'è scritto di usare i e i+1 del vettore in ingresso come valori
        //la funzione del RI però è f(x+h)-f(x)/h
        //h nel mio caso + 1 quindi ho omesso(GIUSTO??)
         
        public List<double> RIfunc(List<double> valori)
        {
            List<double> RI;
            for (int i = 0; i < valori.Count-1; i++)
            {
                double result = valori[i + 1] - valori[i];
                RI.Add(result); //Errore da vedere
            }
            return RI;
        }
    }
}
