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
        //Da decidere bene quali eventi esporre
        public event EventHandler GirataDestra;
        public event EventHandler GirataSinistra;

        protected virtual void OnGirataDestra(EventArgs e) { if (GirataDestra != null) GirataDestra(); }
        protected virtual void OnGirataSinistra(EventArgs e) { if (GirataSinistra != null) GirataSinistra(); }

        public event EventHandler Sit;
        public event EventHandler Stand;
        public event EventHandler LaySit;
        public event EventHandler Lay;

        protected virtual void OnSit(EventArgs e) { if (Sit != null) Sit(); }
        protected virtual void OnLay(EventArgs e) { if (Lay != null) Lay(); }
        protected virtual void OnLaySit(EventArgs e) { if (LaySit != null) LaySit(); }
        protected virtual void OnStand(EventArgs e) { if (Stand != null) Stand(); }


        private int _time = 0;
        double precedente = 0.0;
        bool _isUp = false;
        bool _isDown = false;

        public Analyzer()
        {

        }
        //Questo metodo viene eseguito quando c'è una nuova finestra da alizzare
        public void Run(object sender, Window e)
        {
            //Nel paramentro e ho i dati di questa finestra da analizzare
            AnalyzeGirata(e);
            //AnalyzeMoto(e);
           AnalyzePosizionamento(e);//magari cambiamo il nome
        }


        //Ognuno dei seguenti metodi secondo la propria logica genera l'evento a lui associato
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
                    value = precedente;
                else
                    value = Math.Atan(y[i] / z[i]);

                double next = Math.Atan(y[i + 1] / z[i + 1]);

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
                }
                if (delta <= -2.5)
                {
                    _isDown = true;
                }
                if (_isUp && _isDown)
                {
                    _isDown = false;
                    _isUp = false;
                }
                #endregion
                
                if (delta < -0.08) // Girate maggiori di 5 gradi
                {//sinistra
                    OnGirataSinistra(new EventArgs());
                }
                else if (delta > 0.08) // Girate maggiori di 5 gradi
                {//destra
                    OnGirataDestra(new EventArgs());
                }
            }
        }

        private void AnalyzeMoto(Window e)
        {
            throw new NotImplementedException();
        }

        
    }
}
