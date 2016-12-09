using System;
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
                result.Add(item.GetSensor(sensor).GetModAcc());
            }

            return result;
        }

        public List<double> GetModGyro(int sensor)
        {
            List<double> result = new List<double>();
            foreach (var item in w)
            {
                result.Add(item.GetSensor(sensor).GetModGyro());
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
                    smooth.Add(w.GetInstant(j));
                }
                w[i] = smooth.Mean();
            }
        }


        private Instant Mean()
        {
            int numSensori = 9;
            List<List<double>> m = w.GetInstant(0);
            for (int i = 1; i < w.Count; i++)
            {
                for (int j = 0; j < w.GetInstant(i).Count; j++)
                {
                    for (int k = 0; k < numSensori; k++)
                    {
                        m[j][k] += w.GetInstant(i).GetSensor(j).GetValue(k);
                    }

                }
            }
            for (int j = 0; j < w.GetInstant(i).Count; j++)
            {
                for (int k = 0; k < numSensori; k++)
                {
                    m[j][k] = m[j][k] / (w.Count);
                }

            }
            return m; 
        }

        public List<double> ModAcc(int s)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < w.Count; i++)
            {
                result.Add(w.GetInstance(i).GetSensor(s).GetModuloAcc());
            }
            return result;
        }

        public List<double> ModGyro(int s)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < w.Count; i++)
            {
                result.Add(w.GetInstance(i).GetSensor(s).GetModuloGyro());
            }
            return result;
        }
    }
}
