using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    public delegate void EventHandler();
    class Analyzer
    {
        // Soglia sopra la quale riconoscere le girate, sia dx che sx
        // 0.08 rad = 5 gradi
        private const double ANGOLO_GIRATA = 0.08;
  
        // Soglia delta per discontinuità
        private const double SOGLIA = 2.0;
        private const double PI = Math.PI;

        #region Eventi
        public event EventHandler GirataDestra;
        public event EventHandler GirataSinistra;
        public event EventHandler Sit;
        public event EventHandler Stand;
        public event EventHandler LaySit;
        public event EventHandler Lay;
        public event EventHandler Stazionamento;
        public event EventHandler Moto;
        #endregion

        private int _time;
        private double _precedente;
        private int _isUp;
        private int _isDown;

        public Analyzer()
        {
            _time = 0;
            _precedente = 0.0;
            _isUp = 0;
            _isDown = 0;
        }

        public void Run(object sender, Window e)
        {
            AnalyzeGirata(e);
            AnalyzeMoto(e);
            AnalyzePosizionamento(e);
        }

        private void AnalyzePosizionamento(Window e)
        {
            List<double> acc=new List<double>();
            foreach (var item in  e.GetAccelerometro(e.matriceSmooth))
            {
                acc.Add(item[0]);
            }

            foreach (var item in acc)
            {
                if (item < 2.7)
                {
                    OnLay(new EventArgs());
                }
                else if (item < 3.7) {
                    OnLaySit(new EventArgs());
                } 
                else if (item < 7)
                {
                    OnSit(new EventArgs());
                }
                else
                {
                    OnStand(new EventArgs());
                }
            }
            
        }

        private void AnalyzeGirata(Window e)
        {
            List<double> y = new List<double>();
            List<double> z = new List<double>();
            foreach (var item in e.GetMagnetometro(e.matriceSmooth))
            {
                y.Add(item[1]);
                z.Add(item[2]);
            }

            double delta = 0.0;

            for (int i = -1; i < y.Count - 1; i++, _time++)
            {
                #region Rimuovi-Discontinuita'
                double value = 0.0;
                if (i == -1)
                    value = _precedente;
                else
                    value = Math.Atan(y[i] / z[i]);

                double next = Math.Atan(y[i + 1] / z[i + 1]);

                if (i == 4)
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

                if (delta < -ANGOLO_GIRATA) // Girate maggiori di 5 gradi
                {//sinistra

                    OnGirataSinistra(new EventArgs());

                }
                else if (delta > ANGOLO_GIRATA) // Girate maggiori di 5 gradi
                {//destra
                    OnGirataDestra(new EventArgs());
                }
            }
        }

        //private void AnalyzeMoto(Window e)
        //{
        //    List<List<double>> accellerometri = e.GetAccelerometro(e.matriceSmooth);
        //    List<double> moduloAcc = e.ModuloAccelerometro(e.matriceSmooth);
        //    List<double> devstd = e.DeviazioneStandard(moduloAcc);
        //    foreach (double item in devstd)
        //    {
        //        Console.WriteLine("valore devstd: " + item);
        //        if (item <= 1)
        //            OnStazionamento(new EventArgs());
        //        else
        //            OnMoto(new EventArgs());
        //    }
        //}

        private void AnalyzeMoto(Window e)
        {
            List<List<double>> accellerometri = e.GetAccelerometro(e.matriceSmooth);
            List<double> moduloAcc = e.ModuloAccelerometro(e.matriceSmooth);

            double devstd = DeviazioneStandard(moduloAcc, e.Media(moduloAcc));
            double valoreMedio = e.Media(moduloAcc);
            double diff = Math.Abs(Math.Abs(devstd - valoreMedio) - 9.81);

            //Console.WriteLine(diff);

            if (diff < 0.1)
                OnStazionamento(new EventArgs());
            else
                OnMoto(new EventArgs());
        }

        /*
         * Deviazione standard
         * Input: Una lista di double
         * Output: La deviazione standard.
         */
        private double DeviazioneStandard(List<double> l, double media)
        {
            double sum = 0;
            foreach (double item in l)
            {
                sum += (item - media) * (item - media);
            }
            return Math.Sqrt(sum / l.Count);
        }

        #region Metodi-Eventi
        protected virtual void OnGirataDestra(EventArgs e) { if (GirataDestra != null) GirataDestra(); }
        protected virtual void OnGirataSinistra(EventArgs e) { if (GirataSinistra != null) GirataSinistra(); }
        protected virtual void OnSit(EventArgs e) { if (Sit != null) Sit(); }
        protected virtual void OnLay(EventArgs e) { if (Lay != null) Lay(); }
        protected virtual void OnLaySit(EventArgs e) { if (LaySit != null) LaySit(); }
        protected virtual void OnStand(EventArgs e) { if (Stand != null) Stand(); }
        protected virtual void OnStazionamento(EventArgs e) { if (Stazionamento != null) Stazionamento(); }
        protected virtual void OnMoto(EventArgs e) { if (Moto != null) Moto(); }
        #endregion
    }
}
