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
        private Timer timer;
        private PointPairList pointPl = new PointPairList();
        public List<double> Lx = new List<double>();
        public List<double> Ly = new List<double>();
        double X, Y;

        public Form1()
        {
            InitializeComponent();
            EventArgs e = new EventArgs();
            zedGraphAccelerometro_Load(this, e);
            zedGraphOrientamento_Load(this, e);
            zedGraphGiroscopio_Load(this, e);
            
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            


        }

        void timer_Tick(object sender, EventArgs e)
        { 
            Y = Math.Cos(X);
            Lx.Add(X); Ly.Add(Y);

            X += 0.1;
            // pointPl.Add(x,y);
            pointPl.Add(X, Y);
            zedGraphAccelerometro.AxisChange();
            //zedGraphControl1.Refresh(); 
            zedGraphAccelerometro.Invalidate();

            zedGraphGiroscopio.AxisChange();
            //zedGraphControl1.Refresh(); 
            zedGraphGiroscopio.Invalidate();

            zedGraphOrientamento.AxisChange();
            //zedGraphControl1.Refresh(); 
            zedGraphOrientamento.Invalidate();
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
            LineItem myCurve = myPane.AddCurve("ADC", pointPl, Color.Blue, SymbolType.None);
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
