using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Sensor
    { 
        List<double> accelerometer;
        List<double> gyroscope;
        List<double> magnetometer;
        List<double> quaternion;
        public Sensor()
        { //non testato, non utile
            accelerometer = new List<double>(3); //x y z
            gyroscope = new List<double>(3); //x y z
            magnetometer = new List<double>(3); //x y z
            quaternion = new List<double>(4); //x y z
        }

        public Sensor(List<double> a, List<double> g, List<double> m, List<double> q)
        {// non testato, forse non serve nemmeno

            if (a.Count == 3 && g.Count == 3 && m.Count == 3 && q.Count == 4)
            {
                accelerometer = a;
                gyroscope = g;
                magnetometer = m;
                quaternion = q;
            }
        }
        
        public Sensor(List<double> data)
        {
            int c = data.Count;
            accelerometer = new List<double>(data.GetRange(0, 3));
            gyroscope = new List<double>(data.GetRange(3, 3));
            magnetometer = new List<double>(data.GetRange(6, 3));
            quaternion = new List<double>(data.GetRange(9, 3));
        }
        //metodi da specifiche vedi pagina 5
        public double ModuloAcc()
        {
            double m = Math.Sqrt(accelerometer[0] * accelerometer[0] +
                                accelerometer[1] * accelerometer[1] +
                                accelerometer[2] * accelerometer[2]);

            return m;
        }

        public double ModuloGyro()
        {
            double m = Math.Sqrt(gyroscope[0] * gyroscope[0] +
                                gyroscope[1] * gyroscope[1] +
                                gyroscope[2] * gyroscope[2]);

            return m;
        }

        public double ModuloMagn()
        {
            double m = Math.Sqrt(magnetometer[0] * magnetometer[0] +
                                magnetometer[1] * magnetometer[1] +
                                magnetometer[2] * magnetometer[2]);

            return m;
        }
        
        public override string ToString()
        {
            string s = "";
            s = "Acc(x,y,z): (" + accelerometer[0] + ", " + accelerometer[1] + ", " + accelerometer[2] + ") ";
            s += "Gyr(x,y,z): (" + gyroscope[0] + ", " + gyroscope[1] + ", " + gyroscope[2] + ") ";
            s += "Mag(x,y,z): (" + magnetometer[0] + ", " + magnetometer[1] + ", " + magnetometer[2] + ") ";

            return s;
        }
    }
}
