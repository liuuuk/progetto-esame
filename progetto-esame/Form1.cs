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
    


    public partial class Form1 : Form
    {

        

       
        private PointPairList pointPl = new PointPairList(); //per accelerometro
        private PointPairList pointP2 = new PointPairList(); //per giroscopio
       

        public Form1()
        {
            InitializeComponent();
            EventArgs e = new EventArgs();

            zedGraphAccelerometro_Load(this, e);
            zedGraphOrientamento_Load(this, e);
            zedGraphGiroscopio_Load(this, e);

        }


        //Per scrivere sulle zedgraph
        public void test(object sender, Window e)
        {
            
            List<double> modacc = new List<double>();
            List<double> modgir = new List<double>();

            modgir = e.ModuloGiroscopio(e.matrice);
            modacc = e.ModuloAccelerometro(e.matrice);

            for (int i = 0; i < modacc.Count; i++)
            {
                pointPl.Add(2*i, modacc[i]);
                pointP2.Add(2*i, modgir[i]);
            }
            zedGraphAccelerometro.AxisChange();
           // zedGraphAccelerometro.Refresh();  //Da problemi di cross-threading
            zedGraphAccelerometro.Invalidate();

            zedGraphGiroscopio.AxisChange();
            //zedGraphAccelerometro.Refresh(); 
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

            

            // myPane.YAxis.Scale.MagAuto = false;
            // PointPairList holds the data for plotting, X and Y arrays

            // RollingPointPairList aaa = new RollingPointPairList(i);
            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("ADC", pointPl, Color.Red, SymbolType.None);
            //myCurve.Line.IsVisible = false;
            myCurve.Line.Width = 1.0F;
            // LineItem myCurve = myPane.AddCurve("ADC", aaa, Color.Blue, SymbolType.None);
            // BarItem mybar = myPane.AddBar("plotting", x , y, Color.Red);
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

            // myPane.YAxis.Scale.MagAuto = false;
            // PointPairList holds the data for plotting, X and Y arrays

            // RollingPointPairList aaa = new RollingPointPairList(i);
            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("ADC", pointPl, Color.Green, SymbolType.None);
            //myCurve.Line.IsVisible = false;
            myCurve.Line.Width = 1.0F;
            // LineItem myCurve = myPane.AddCurve("ADC", aaa, Color.Blue, SymbolType.None);
            // BarItem mybar = myPane.AddBar("plotting", x , y, Color.Red);
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

            // myPane.YAxis.Scale.MagAuto = false;
            // PointPairList holds the data for plotting, X and Y arrays

            // RollingPointPairList aaa = new RollingPointPairList(i);
            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("ADC", pointP2, Color.Blue, SymbolType.None);
            //myCurve.Line.IsVisible = false;
            myCurve.Line.Width = 1.0F;
            // LineItem myCurve = myPane.AddCurve("ADC", aaa, Color.Blue, SymbolType.None);
            // BarItem mybar = myPane.AddBar("plotting", x , y, Color.Red);
            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphAccelerometro.AxisChange();
            zedGraphAccelerometro.Refresh();
            zedGraphAccelerometro.Invalidate();
        }
    }
}
