using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace progetto_esame
{


    delegate void SetTextCallback(object sender, Window e);

    public partial class Form1 : Form
    {


        double precedente = 0.0;
        bool _isUp = false;
        bool _isDown = false;

        private bool _isNonSmoothAcc = false;
        private bool _isNonSmoothGiro = false;
        private bool _isNonSmoothTheta = false;

        private PointPairList _pointAcc = new PointPairList(); //per accelerometro
        private PointPairList _pointGiro = new PointPairList(); //per giroscopio
        private PointPairList _pointTheta = new PointPairList(); //per orientamento
        private PointPairList _pointThetaDEBUG = new PointPairList();
        private PointPairList _pointLinea = new PointPairList();

        //Per non Smooth
        private PointPairList _pointAccNoSmooth = new PointPairList(); //per accelerometro
        private PointPairList _pointGiroNoSmooth = new PointPairList(); //per giroscopio
        private PointPairList _pointThetaNoSmooth = new PointPairList(); //per orientamento

        private int _time; //asse x

        public Form1()
        {
            InitializeComponent();
            EventArgs e = new EventArgs();
            _time = 0;
            zedGraphAccelerometro_Load(this, e);
            zedGraphOrientamento_Load(this, e);
            zedGraphGiroscopio_Load(this, e);
        }


        //Per scrivere sulle zedgraph
        public void DisegnaGrafici(object sender, Window e)
        {
            if (this.zedGraphAccelerometro.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(DisegnaGrafici);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                DisegnaModuloAcc(e);
                DisegnaModuloGiro(e);
                DisegnaModuloTheta(e);
            }
        }


        private void DisegnaModuloTheta(Window e)
        {
            List<double> y = new List<double>();
            List<double> z = new List<double>();
            foreach (var item in e.GetMagnetometro(e.matriceSmooth))
            {
                y.Add(item[1]);
                z.Add(item[2]);
            }

            List<double> yNoSmooth = new List<double>();
            List<double> zNoSmooth = new List<double>();
            foreach (var item in e.GetMagnetometro(e.matrice))
            {
                yNoSmooth.Add(item[1]);
                zNoSmooth.Add(item[2]);
            }

            _time = _pointTheta.Count;


            double delta = 0.0;
            
            for (int i = -1; i < y.Count-1; i++, _time++)
            {
                double value = 0.0;
                if (i == -1)
                    value = precedente;
                else
                    value = Math.Atan(y[i] / z[i]);

                double next = Math.Atan(y[i+1] / z[i+1]);
                _pointThetaDEBUG.Add(2*_time, value);

                //double valueNoSmooth = Math.Atan(yNoSmooth[i] / zNoSmooth[i]);

                if (i == 4)
                {
                    precedente = next;
                }
                if (i == -1)
                {
                    delta = next - precedente;
                }
                else
                {
                    delta = next - value; 
                }

                if (_isUp)
                {
                    value -= 3.14;
                }

                if (_isDown)
                {
                    value += 3.14;
                }

                if (delta >= 2.5)
                {
                    _isUp = true;
                    richTextBox1.AppendText("Find up " + 2 * _time + "D " + delta + Environment.NewLine);
                }
                if (delta <= -2.5)
                {
                    _isDown = true;
                    richTextBox1.AppendText("Find down " + 2 * _time + "D " + delta + Environment.NewLine);
                }

                if (_isUp && _isDown)
                {
                    _isDown = false;
                    _isUp = false;
                }
                
                _pointTheta.Add(2 * _time, value);
               // _pointThetaNoSmooth.Add(2 * _time, valueNoSmooth);
            }

            

            zedGraphOrientamento.AxisChange();
            zedGraphOrientamento.Refresh();
            zedGraphOrientamento.Invalidate();

        }

        private void DisegnaModuloAcc(Window e)
        {
            List<double> modacc = new List<double>(); //Dati Smoothati
            List<double> modaccNoSmooth = new List<double>();

            modacc = e.ModuloAccelerometro(e.matriceSmooth); //Dati Smoothati
            modaccNoSmooth = e.ModuloAccelerometro(e.matrice);

            _time = _pointAcc.Count;
            for (int i = 0; i < modacc.Count; i++, _time++)
            {
                _pointAcc.Add(2 * _time, modacc[i]);
                _pointAccNoSmooth.Add(2 * _time, modaccNoSmooth[i]); //Dati Smoothati
            }



            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();
        }

        private void DisegnaModuloGiro(Window e)
        {
            List<double> modgir = new List<double>(); //Dati Smoothati
            List<double> modgirNoSmooth = new List<double>();

            modgir = e.ModuloGiroscopio(e.matriceSmooth); //Dati Smoothati
            modgirNoSmooth = e.ModuloGiroscopio(e.matrice);

            _time = _pointGiro.Count;
            for (int i = 0; i < modgir.Count; i++, _time++)
            {
                _pointGiro.Add(2 * _time, modgir[i]);
                _pointGiroNoSmooth.Add(2 * _time, modgirNoSmooth[i]); //Dati Smoothati
            }



            zedGraphGiroscopio.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphGiroscopio.Invalidate();
        }


        private void zedGraphAccelerometro_Load(object sender, EventArgs e)
        {
            zedGraphAccelerometro.GraphPane.CurveList.Clear();
            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphAccelerometro.GraphPane;
            myPane.Title.Text = "Modulo Accelerometro";
            myPane.XAxis.Title.Text = "Time(ms)";
            myPane.YAxis.Title.Text = "g(m/sec^2)";

            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("Smooth", _pointAcc, Color.Red, SymbolType.None);
            LineItem myCurveNoSmooth = myPane.AddCurve("No Smooth", _pointAccNoSmooth, Color.Blue, SymbolType.None);
            myCurveNoSmooth.IsVisible = _isNonSmoothAcc;

            myCurve.Line.Width = 1.0F;

            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();

        }

        private void zedGraphOrientamento_Load(object sender, EventArgs e)
        {
            zedGraphOrientamento.GraphPane.CurveList.Clear();
            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphOrientamento.GraphPane;
            myPane.Title.Text = "Theta";
            myPane.XAxis.Title.Text = "Time(ms)";
            myPane.YAxis.Title.Text = "Degree";

           

            // Add curves to myPane object
            
            LineItem myCurve = myPane.AddCurve("Smooth", _pointTheta, Color.Red, SymbolType.None);
            LineItem myCurveNoSmooth = myPane.AddCurve("No Smooth", _pointThetaNoSmooth, Color.Blue, SymbolType.None);

            LineItem myCurveDebug = myPane.AddCurve("No Smooth", _pointThetaDEBUG, Color.Black, SymbolType.None);
            LineItem myLine = myPane.AddCurve("", _pointLinea, Color.Blue, SymbolType.None);
            
            myCurveNoSmooth.IsVisible = _isNonSmoothTheta;

            myCurve.Line.Width = 1.0F;

            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphOrientamento.AxisChange();
            zedGraphOrientamento.Refresh();
            zedGraphOrientamento.Invalidate();
        }

        private void zedGraphGiroscopio_Load(object sender, EventArgs e)
        {
            zedGraphGiroscopio.GraphPane.CurveList.Clear();
            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphGiroscopio.GraphPane;
            myPane.Title.Text = "Modulo Giroscopio";
            myPane.XAxis.Title.Text = "Time(ms)";
            myPane.YAxis.Title.Text = "g(rad/sec)";

            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("Smooth", _pointGiro, Color.Red, SymbolType.None);
            LineItem myCurveNoSmooth = myPane.AddCurve("No Smooth", _pointGiroNoSmooth, Color.Blue, SymbolType.None);
            myCurveNoSmooth.IsVisible = _isNonSmoothAcc;
            //myCurve.Line.IsVisible = false;
            myCurve.Line.Width = 1.0F;

            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();
        }

        private void SmoothAcc_CheckedChanged(object sender, EventArgs e)
        {//Parte a false
            if (_isNonSmoothAcc)
                _isNonSmoothAcc = false;
            else
                _isNonSmoothAcc = true;

            zedGraphAccelerometro.GraphPane.CurveList[1].IsVisible = _isNonSmoothAcc;
            zedGraphAccelerometro.Refresh();
        }

        private void SmoothGiro_CheckedChanged(object sender, EventArgs e)
        {//Parte a false
            if (_isNonSmoothGiro)
                _isNonSmoothGiro = false;
            else
                _isNonSmoothGiro = true;

            zedGraphGiroscopio.GraphPane.CurveList[1].IsVisible = _isNonSmoothGiro;
            zedGraphGiroscopio.Refresh();
        }

        private void SmoothTheta_CheckedChanged(object sender, EventArgs e)
        {
            if (_isNonSmoothTheta)
                _isNonSmoothTheta = false;
            else
                _isNonSmoothTheta = true;

            zedGraphOrientamento.GraphPane.CurveList[1].IsVisible = _isNonSmoothTheta;
            zedGraphOrientamento.Refresh();
        }
    }
       
}
