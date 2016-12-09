using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Sensor
    {
        Triaxial accelerometer;
        Triaxial gyroscope;
        Triaxial magnetometer;
        //Sensor quaternion; //Non usati

        public Sensor(List<double> data)
        {
            accelerometer = new Triaxial(data.GetRange(0, 3));
            gyroscope = new Triaxial(data.GetRange(3, 3));
            magnetometer = new Triaxial(data.GetRange(6, 3));
            //quaternion = new Sensor(data.GetRange(9, 3)); //Non usati
        }

        public Triaxial GetAcc()
        {
            return accelerometer;
        }
        public Triaxial GetGryo()
        {
            return gyroscope;
        }
        public Triaxial GetMang()
        {
            return magnetometer;
        }
        /*
        public Triaxial GetQuat()
        {
            return quaternion;
        }
        */
        public override string ToString()
        {
            string s = "";
            s = "Acc(x,y,z): " + accelerometer.ToString();
            s += " Gyr(x, y, z): " + gyroscope.ToString();
            s += " Mag(x, y, z):" + magnetometer.ToString();
            //s += " Qua(q1,q2,q2,q)3ac:" + quaternion.ToString(); //Non usati
            return s;
        }

        public double GetModAcc()
        {
            return accelerometer.Modulo();
        }

        public double GetModGyro()
        {
            return gyroscope.Modulo();
        }
    }
}
