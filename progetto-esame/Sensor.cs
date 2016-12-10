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
        List<double> data; //ax, ay, az, gx, gy, gz, mx, my, mz, q0, q1, q2, q3

        public Sensor(List<double> data)
        {
            this.data = data;
        }

        public List<double> SensorToList()
        {
            return data;
        }

        public double GetValue(int i)
        {
            return data[i];
        }

        public double GetRoll()
        {
            //Per chiarezza
            double q0 = GetValue(9);
            double q1 = GetValue(10);
            double q2 = GetValue(11);
            double q3 = GetValue(11);

            double roll = Math.Atan(((2 * q2 * q3) + (2 * q0 * q1)) /
                                    ((2 * q0 * q0) + (2 * q3 * q3) - 1));


            return roll;
        }

        public double GetPitch()
        {
            //Per chiarezza
            double q0 = GetValue(9);
            double q1 = GetValue(10);
            double q2 = GetValue(11);
            double q3 = GetValue(11);

            double pitch = -Math.Asin((2 * q1 * q3) - (2 * q0 * q2));

            return pitch;
        }

        public double GetYaw()
        {
            //Per chiarezza
            double q0 = GetValue(9);
            double q1 = GetValue(10);
            double q2 = GetValue(11);
            double q3 = GetValue(11);

            double yaw = Math.Atan(((2 * q1 * q2) + (2 * q0 * q3)) /
                                    (2 * q0 * q0) + (2 * q1 * q1) - 1);

            return yaw;
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
