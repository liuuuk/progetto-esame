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
        private PointPairList pointAcc = new PointPairList(); //per accelerometro
        private PointPairList pointGiro = new PointPairList(); //per giroscopio
        private int time; //asse x

        public Form1()
        {
            InitializeComponent();
            EventArgs e = new EventArgs();
            time = 0;
            zedGraphAccelerometro_Load(this, e);
            zedGraphOrientamento_Load(this, e);
            zedGraphGiroscopio_Load(this, e);
        }


        //Per scrivere sulle zedgraph
        public void Disegna(object sender, Window e)
        {
            if (this.zedGraphAccelerometro.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Disegna);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                DisegnaModuloAcc(e);
                DisegnaModuloGiro(e);
            }   
        }

        private void DisegnaModuloAcc(Window e)
        {
            List<double> modacc = new List<double>();
            modacc = e.ModuloAccelerometro(e.matrice);
            time = pointAcc.Count;
            for (int i = 0; i < modacc.Count; i++, time++)
            {
                pointAcc.Add(2 * time, modacc[i]);
            }
            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();
        }

        private void DisegnaModuloGiro(Window e)
        {
            List<double> modgir = new List<double>();
            modgir = e.ModuloGiroscopio(e.matrice);
            time = pointGiro.Count;
            for (int i = 0; i < modgir.Count; i++, time++)
            {
                pointGiro.Add(2 * time, modgir[i]);
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
            LineItem myCurve = myPane.AddCurve("ADC", pointAcc, Color.Red, SymbolType.None);
           
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
            myPane.Title.Text = "Orientamento";
            myPane.XAxis.Title.Text = "Time(ms)";
            myPane.YAxis.Title.Text = "Degree";
            
            // Add curves to myPane object
            //LineItem myCurve = myPane.AddCurve("ADC", point, Color.Green, SymbolType.None);
           
            //myCurve.Line.Width = 1.0F;
            
            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();
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
            LineItem myCurve = myPane.AddCurve("ADC", pointGiro, Color.Blue, SymbolType.None);
            //myCurve.Line.IsVisible = false;
            myCurve.Line.Width = 1.0F;
            
            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();
        }
    }
}
