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
        public Sensor GetSensor(int index) {
            return i[index];
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
