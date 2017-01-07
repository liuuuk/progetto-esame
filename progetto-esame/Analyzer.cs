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
        //0,174533 rad = 10 gradi
        private const double ANGOLO_GIRATA = 0.174533;

        // Soglia delta per discontinuità
        private const double SOGLIA = 2.0;
        private const double PI = Math.PI;
        // Campioni ogni 0.02 secondi
        private const double FREQ = 0.02;

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

       
        private double _precedente;
        private int _isUp;
        private int _isDown;

        private int campionePosizione = 0;
        private int campioneGirata = 0;
        private int campioneMovimento = 0;

        // Conta quante finestre sono state inviate finora
        private int nFinestra;

        // Tempo della prima acquisizione
        DateTime start;

        string posizione = "";
        string posizione_prec = "";

        string girata = "";
        string girata_prec = "";

        string movimento = "";
        string movimento_prec = "";
        private bool primo = true;

        public Analyzer()
        {
            _precedente = 0.0;
            _isUp = 0;
            _isDown = 0;
        }

        public void SetStart(object sender, DateTime d)
        {
            start = d;
            Console.WriteLine("Inizio " + start.ToString());
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
                campionePosizione++;
                DateTime istante = start.AddSeconds(campionePosizione * FREQ);
                posizione_prec = posizione;
                if (item < 2.7)
                {
                    posizione = "Straiato";
                    OnLay(new EventArgs());
                }
                else if (item < 3.7)
                {
                    posizione = "Seduto/Sdraiato";
                    OnLaySit(new EventArgs());
                } 
                else if (item < 7)
                {
                    posizione = "Seduto";
                    OnSit(new EventArgs());
                }
                else
                {
                    posizione = "In Piedi";
                    OnStand(new EventArgs());
                }
                if(posizione != posizione_prec)
                {
                    DateTime fine = istante.AddSeconds(campionePosizione * FREQ);
                    // Al posto della WriteLine si scrive su file
                    Console.WriteLine(istante.ToString() + " - " + fine.ToString() + " " + posizione);
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

            for (int i = -1; i < y.Count - 1; i++)
            {
                campioneGirata++;
                DateTime istante = start.AddSeconds(campionePosizione * FREQ);
                girata_prec = girata;

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

                if (delta < -ANGOLO_GIRATA)
                {//sinistra
                    girata = "Girata a Sinistra";
                    OnGirataSinistra(new EventArgs());
                }
                else if (delta > ANGOLO_GIRATA)
                {//destra
                    girata = "Girata a Destra";
                    OnGirataDestra(new EventArgs());
                }
                if (girata != girata_prec && !primo)
                {
                    DateTime fine = istante.AddSeconds(campioneGirata * FREQ);
                    // Al posto della WriteLine si scrive su file
                    Console.WriteLine(istante.ToString() + " - " + fine.ToString() + " " + girata);
                }
                if (primo)
                {
                    primo = !primo;
                    girata = "";
                }
            }
        }

        private void AnalyzeMoto(Window e)
        {
            List<List<double>> accelerometri = e.GetAccelerometro(e.matriceSmooth);
            List<double> moduloAcc = e.ModuloAccelerometro(e.matriceSmooth);
            List<double> devstd = e.DeviazioneStandard(moduloAcc);
            foreach (double item in devstd)
            {
                Console.WriteLine("valore devstd: " + item);
                if (item < 1)
                    OnStazionamento(new EventArgs());
                else
                    OnMoto(new EventArgs());
            }
        }

        //private void AnalyzeMoto(Window e)
        //{
        //    List<List<double>> accelerometri = e.GetAccelerometro(e.matriceSmooth);
        //    List<double> moduloAcc = e.ModuloAccelerometro(e.matriceSmooth);
        //    /*
        //     * Stavo dando un occhiata a questo codice per aggiungere la scrittura su file e mi e venuto un dubbio.
        //     * Come mai non ci sono cicli? 
        //     * Mi spiego in accelerometri hai una matrice che è fatta (ax ay az) x Campioni
        //     * dove i campioni sono nel tempo. Secondo me la differenza tra devstd e media va calcolata nel tempo.
        //     * Del resto sia la girata (mia) che la posizione (di pane) trattano una matrice bidimensionale come la tua,
        //     * con la differenza che al posto dei dati dell'accelerometro io ho i magnetometri.
        //     * 
        //     * Quindi secondo me è giusta la devstd che avevo prima che restituisce una lista, perchè poi tu la differenza la dovresti
        //     * calcolare punto per punto nel tempo. 
        //     * 
        //     * (Come il metodo che c'e sopra)
        //     * 
        //     * Luca
        //     * 
        //     */
        //    double devstd = DeviazioneStandard(moduloAcc, e.Media(moduloAcc));
        //    double valoreMedio = e.Media(moduloAcc);
        //    double diff = Math.Abs(Math.Abs(devstd - valoreMedio) - 9.81);

        //    //Console.WriteLine(diff);

        //    if (diff < 0.1)
        //    {

        //        OnStazionamento(new EventArgs());
        //    }
        //    else
        //    {
        //        OnMoto(new EventArgs());
        //    }

        //}

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
