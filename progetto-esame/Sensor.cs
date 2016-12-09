using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Sensor
    {
        //Ripenso con lista di double
        List<double> data;

        public Sensor(List<double> data)
        {
            this.data = data;
        }

        public double GetValue(int i)
        {
            return data[i];
        }
        
        public double GetModuloAcc()
        {
            double sum = 0;
            for (int i = 0; i < 3; i++)
            {
                sum += data[i] * data[i];
            }
            return Math.Sqrt(sum);
        }

        public double GetModuloGyro()
        {
            double sum = 0;
            for (int i = 3; i < 6; i++)
            {
                sum += data[i] * data[i];
            }
            return Math.Sqrt(sum);
        }

    }
}
