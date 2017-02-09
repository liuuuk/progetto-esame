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


    delegate void DisegnaGraficiCallback(object sender, Window e);
    delegate void TextLogCallback(object sender, InfoEventArgs e);
    delegate void ChangeTextCallback();

    public partial class Form1 : Form
    {
        // Soglia delta per discontinuità di theta
        private const double SOGLIA = 2.5;
        private const double PI = Math.PI;

        private double _angolo;
        private double _precedente;
        private int _isUp;
        private int _isDown;

        private bool _isNonSmoothAcc = false;
        private bool _isNonSmoothGiro = false;
        private bool _isDebug = false;

        int x = 0;
        int xdev = 0;

        private PointPairList _pointAcc = new PointPairList(); //per accelerometro
        private PointPairList _pointGiro = new PointPairList(); //per giroscopio
        private PointPairList _pointTheta = new PointPairList(); //per orientamento
        private PointPairList _pointThetaDEBUG = new PointPairList();
        
        //debug
        private PointPairList _devstd = new PointPairList();
        private PointPairList _accX = new PointPairList();
        private PointPairList _pointRapInc = new PointPairList();
        private PointPairList _devstd2 = new PointPairList();

        //Per non Smooth
        private PointPairList _pointAccNoSmooth = new PointPairList(); //per accelerometro
        private PointPairList _pointGiroNoSmooth = new PointPairList(); //per giroscopio

        private int _time; //asse x

        public Form1()
        {
            InitializeComponent();
            EventArgs e = new EventArgs();
            _time = 0;
            _angolo = 0.0;
            _precedente = 0.0;
            _isDown = 0;
            _isUp = 0;

            zedGraphAccelerometro_Load(this, e);
            zedGraphOrientamento_Load(this, e);
            zedGraphGiroscopio_Load(this, e);
            zedGraphControl1_Load(this, e);
            zedGraphControl2_Load(this, e);

            panelDebug.Hide();
        }

        public void DisegnaStazionamento()
        {
            if (this.LabelMoto.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaStazionamento);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelMoto.Text = "Stazionamento";
                pictureAzione.Image = Image.FromFile("../../Resources/stickman-no-walk.png");
            }
        }

        public void DisegnaMoto()
        {
            if (this.LabelMoto.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaMoto);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelMoto.Text = "Movimento";
                pictureAzione.Image = Image.FromFile("../../Resources/stickman-walk.png");
            }
        }

        public void DisegnaLay()
        {
            if (this.LabelPosizione.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaLay);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelPosizione.Text = "Sdraiato";
                picturePosizione.Image = Image.FromFile("../../Resources/stickman-laying-down.png");
            }
        }

        public void DisegnaSit()
        {
            if (this.LabelPosizione.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaSit);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelPosizione.Text = "Seduto";
                picturePosizione.Image = Image.FromFile("../../Resources/stickman-sitting-down.png");

            }
        }

        public void DisegnaStand()
        {
            if (this.LabelPosizione.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaStand);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelPosizione.Text = "In Piedi";
                picturePosizione.Image = Image.FromFile("../../Resources/stickman-no-walk.png");

            }
        }

        public void DisegnaLaySit()
        {
            if (this.LabelPosizione.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaLaySit);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelPosizione.Text = "Sdraiato/Seduto";
                picturePosizione.Image = Image.FromFile("../../Resources/stickman-sitting-down.png");

            }
        }

        public void DisegnaGirataDestra()
        {
            if (this.LabelGirata.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaGirataDestra);
                this.Invoke(d, new object[] { });
            }
            else
            {
                LabelGirata.Text = "Destra";
            }
        }

        public void DisegnaGirataSinistra()
        {
            if (this.LabelGirata.InvokeRequired)
            {
                ChangeTextCallback d = new ChangeTextCallback(DisegnaGirataSinistra);
                this.Invoke(d, new object[] {});
            }
            else
            {
                LabelGirata.Text = "Sinistra";
            }
        }

        public void DisegnaGrafici(object sender, Window e)
        {
            //Forse va in or con le altre zedgraph
            if (this.zedGraphAccelerometro.InvokeRequired)
            {
                DisegnaGraficiCallback d = new DisegnaGraficiCallback(DisegnaGrafici);
                this.Invoke(d, new object[] { sender, e });
            }
            else
            {
                DisegnaModuloAcc(e);
                DisegnaModuloGiro(e);
                DisegnaModuloTheta(e);
                DisegnaDevStd(e);
                DisegnaAccX(e);
                
            }
        }

        public void TextLog(object sender, InfoEventArgs e)
        {
            if (this.textView.InvokeRequired)
            {
                TextLogCallback c = new TextLogCallback(TextLog);
                this.Invoke(c, new object[] { sender, e});
            }
            else
            {
                /*
                 * InfoDebug | ModDebug
                 * T    |   T   -> !(T AND !T) -> T
                 * T    |   F   -> !(T AND !F) -> F = Non scrivere se è un messaggio di debug ma il programma non è in mod debug
                 * F    |   T   -> !(F AND !T) -> T
                 * F    |   F   -> !(F AND !F) -> T
                 */
                if (! (e.IsDebug && !_isDebug))
                {
                    var newLine = Environment.NewLine;
                    textView.AppendText(e.Text + newLine);
                }
                
            }

        }

        private void DisegnaAccX(Window e)
        {
            List<double> acc = new List<double>();
            foreach (var item in e.GetAccelerometro(e.matriceSmooth))
            {
                acc.Add(item[0]);
            }
            _time = _accX.Count;
            foreach (var item in acc)
            {
                _accX.Add(2 * _time, item);
                _time++;
            }
            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
            zedGraphControl2.Invalidate();
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

            _time = _pointTheta.Count;


            double delta = 0.0;
            List<double> rap = new List<double>();
            List<double> dev = new List<double>();

            for (int i = -1; i < y.Count-1; i++, _time++)
            {
                #region Rimuovi-Discontinuita'
                double value = 0.0;
                if (i == -1)
                    value = _precedente;
                else
                    value = Math.Atan(y[i] / z[i]);

                double next = Math.Atan(y[i + 1] / z[i + 1]);

                //solo per debug
                _pointThetaDEBUG.Add(2 * _time, next);
                
                double myVal = value;
                if (i == y.Count-2)
                {
                    _precedente = next;
                }
                if (i == -1)
                {
                    delta = next - _precedente;
                }
                else
                {
                    delta = next - value;
                }
                if (delta >= SOGLIA)
                {
                    _isUp++;
                }
                if (delta <= -SOGLIA)
                {
                    _isDown++;
                }

                #endregion
                next = next - (_isUp * PI) + (_isDown * PI);
                
                _pointTheta.Add(2 * _time, next);
                rap.Add(next);
                dev.Add(next);
                //Conversione di un angolo da rad in gradi
                //          (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min
                _angolo = (myVal - (-1.57)) * (360 - 0) / (1.57 - (-1.57)) + 0;

                //Scrivi su label angolo l'angolo attuale
                if (Math.Round(_angolo) >= 315 && Math.Round(_angolo) < 45)
                    LabelAngolo.Text = Math.Round(_angolo).ToString() + " N";
                else if(Math.Round(_angolo) >= 45 && Math.Round(_angolo) < 135)
                    LabelAngolo.Text = Math.Round(_angolo).ToString() + " E";
                else if(Math.Round(_angolo) >= 135 && Math.Round(_angolo) < 225)
                    LabelAngolo.Text = Math.Round(_angolo).ToString() + " S";
                else if(Math.Round(_angolo) >= 225 && Math.Round(_angolo) < 315)
                    LabelAngolo.Text = Math.Round(_angolo).ToString() + " W";

            }

            List<double> myrapvero = RIFunc(rap);
            
            double ultimo = 0.0;
            foreach (var item in myrapvero)
            {
                _pointRapInc.Add(2*x, item);
                x++;
                ultimo = item;
            }
           


            int h = 1;
            for (int i = 0; i < h; i++)
            {
                _pointRapInc.Add(2 * x, ultimo);
                x++;
            }
            


            zedGraphOrientamento.AxisChange();
            zedGraphOrientamento.Refresh();
            zedGraphOrientamento.Invalidate();
            
        }
        

        private List<double> RIFunc(List<double> l)
        {
            List<double> result = new List<double>();
            int nElementi = l.Count;
            int h = 1;
            for (int i = 0; i < nElementi - h; i++)
            {
                result.Add((l[i + h] - l[i]) / h);
            }
            return result;
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
                if((2 * _time) - Globals.kSmooth * 2 >= 0)
                    _pointAccNoSmooth.Add((2 * _time) - Globals.kSmooth*2, modaccNoSmooth[i]); //Dati non Smoothati
               
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
                if((2 * _time) - Globals.kSmooth * 2 >= 0)
                    _pointGiroNoSmooth.Add((2 * _time ) - Globals.kSmooth * 2, modgirNoSmooth[i]); //Dati non Smoothati

            }



            zedGraphGiroscopio.AxisChange();
            zedGraphGiroscopio.Refresh();
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
            //DEGUB
            //LineItem myCurveDebug = myPane.AddCurve("DEBUG", _pointThetaDEBUG, Color.Black, SymbolType.None);
            //myCurveDebug.IsVisible = _isDebug;

            LineItem myCurveRapInc = myPane.AddCurve("Rap Inc", _pointRapInc, Color.Green, SymbolType.None);
            myCurveRapInc.IsVisible = _isDebug;

            LineItem myCurveDev = myPane.AddCurve("Dev", _devstd2, Color.Blue, SymbolType.None);
            myCurveDev.IsVisible = _isDebug;

            myCurve.Line.Width = 1.0F;

            myCurveRapInc.Line.Width = 2.0F;
            myCurveDev.Line.Width = 2.0F;

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

        private void checkDebug_CheckedChanged(object sender, EventArgs e)
        {//parte false
            if (_isDebug)
            {
                _isDebug = false;
                panelDebug.Hide();
            }
            else
            {
                _isDebug = true;
                panelDebug.Show();
            }
                

            //Grafico nero su girata, mostra discontinuità
            zedGraphOrientamento.GraphPane.CurveList[1].IsVisible = _isDebug;
            zedGraphOrientamento.GraphPane.CurveList[2].IsVisible = _isDebug;
            //zedGraphOrientamento.GraphPane.CurveList[3].IsVisible = _isDebug;
            zedGraphOrientamento.Refresh();
   
        }

        //DEBUG - devstd
        private void DisegnaDevStd(Window e)
        {
            List<double> devstd = new List<double>();

            List<double> a = e.ModuloAccelerometro(e.matrice);
            _time = _devstd.Count;
            foreach (var item in e.DeviazioneStandard(a))
            {
                _devstd.Add(2 * _time, item);
                _time++;
            }
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            zedGraphControl1.Invalidate();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;
            myPane.Title.Text = "Deviazione standard";
            

            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("DEVSTD", _devstd, Color.Red, SymbolType.None);
           
          

            myCurve.Line.Width = 1.0F;

            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphControl1.AxisChange();
            zedGraphControl1.Refresh();
            zedGraphControl1.Invalidate();
        }

        private void zedGraphControl2_Load(object sender, EventArgs e)
        {
            zedGraphControl2.GraphPane.CurveList.Clear();
            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl2.GraphPane;
            myPane.Title.Text = "Acc";


            // Add curves to myPane object
            LineItem myCurve = myPane.AddCurve("ACC", _accX, Color.Red, SymbolType.None);

            myCurve.Line.Width = 1.0F;

            // I add all three functions just to be sure it refeshes the plot. 
            zedGraphControl2.AxisChange();
            zedGraphControl2.Refresh();
            zedGraphControl2.Invalidate();
        }
    }
       
}
