using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    class Triaxial
    {
        double _x, _y, _z;

        public Triaxial(List<double> l)
        {
            this._x = l[0];
            this._y = l[1];
            this._z = l[2];
        }

        #region get
        public double X
        {
            get
            {
                return _x;
            }
        }
        public double Y
        {
            get
            {
                return _y;
            }
        }
        public double Z
        {
            get
            {
                return _z;
            }
        }
        #endregion

        public double Modulo()
        {
            return Math.Sqrt((_x * _x) + (_y * _y) + (_z * _z));
        }

        public override string ToString()
        {
            return "(" + _x + "," + _y + "," + _z + ")";
        }
    }
}
