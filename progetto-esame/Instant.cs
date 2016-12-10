using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Instant
    {
        List<Sensor> i;

        public Instant() { i = new List<Sensor>(); }
        public Instant(List<List<double>> m) //crea un istante a partire da una matrice
        {
            i = new List<Sensor>();
            for (int i = 0; i < m.Count; i++)
            {
                Sensor s = new Sensor(m[i]);
                Add(s);
            }
        }

        public void Add(Sensor s)
        {
            i.Add(s);
        }

        public Sensor GetSensor(int index) {
            return i[index];
        }

        public int Count()
        {
            return i.Count;
        }

        public List<List<double>> InstantToMatrix()
        {
            List<List<double>> m = new List<List<double>>();

            for (int i = 0; i < Count(); i++)
            {
                m[i] = GetSensor(i).SensorToList();
            }

            return m;
        }

        

        public override string ToString()
        {
            string str = "";
            foreach (var item in i)
            {
                str += item.ToString()+System.Environment.NewLine;
            }
            return str;
        }
    }
}
