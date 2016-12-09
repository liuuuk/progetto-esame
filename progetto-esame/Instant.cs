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
        public Sensor getSensor(int index) {
            return i[index];
        }
        public override string ToString()
        {
            string str;
            foreach (var item in i)
            {
                str += item.ToSTring()+System.Environment.NewLine;
            }
            return str;
        }
    }
}
