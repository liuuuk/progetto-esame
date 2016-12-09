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

        //we ciccio
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
        
    }
}
