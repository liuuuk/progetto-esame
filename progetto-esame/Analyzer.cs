using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    public delegate void EventHandler();
    public delegate void InfoEventHandler(object sender, InfoEventArgs e);

    public class InfoEventArgs : EventArgs
    {
        private string _text;
        private bool _isDebug;

        public InfoEventArgs(string t, bool d)
        {
            _text = t;
            _isDebug = d;
        }

        public string Text
        {
            get { return _text; }
        }
        public bool IsDebug
        {
            get { return _isDebug; }
        }
    }

    class Analyzer
    {
        // Soglia sopra la quale riconoscere le girate, sia dx che sx
        // 0.08 rad = 5 gradi
        //0,174533 rad = 10 gradi
        private const double ANGOLO_GIRATA_LOCALE = 0.17;
        private const double ANGOLO_GIRATA_TOTALE = 0.53;
        // Soglia delta per discontinuità di theta
        private const double SOGLIA = 2.5;
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

        public event InfoEventHandler Info;
        #endregion

        // count non 
        int count_picchi_moto = 0;
        int count_picchi_posizione = 0;
        double count_picchi_girata_sx = 0.0;
        double count_picchi_girata_dx = 0.0;
        int count = 0;

        private double _precedente;
        private int _isUp;
        private int _isDown;
        bool isdisc = false;

        private int campionePosizione = 0;
        private int campioneGirata = 0;
        private int campioneMovimento = 0;

        // Tempo della prima acquisizione
        DateTime start;

        string posizione = "";
        string posizione_prec = "";

        string girata = "";
        string girata_prec = "";

        string movimento = "";
        string movimento_prec = "";

        private bool primo = true;

        string mypath;
        string myFilename;

        public Analyzer(string filename)
        {
            myFilename = @"\" + filename;
            _precedente = 0.0;
            _isUp = 0;
            _isDown = 0;
        }

        private void PrepareFile()
        {
            var newLine = Environment.NewLine;
            mypath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            string t = newLine + "Pronto a scrivere sul file: " + newLine + mypath + myFilename + newLine;
            t += "********************************************";
            
            OnInfo(new InfoEventArgs(t, false));
        }

        public void SetStart(object sender, DateTime d)
        {
            start = d;
            PrepareFile();
        }

        public void Run(object sender, Window e)
        {
            AnalyzeGirata(e);
            AnalyzePosizionamento(e);
            AnalyzeMoto(e);
            
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
                    count_picchi_posizione = 0;
                    posizione = "Sdraiato";
                    OnLay(new EventArgs());
                }
                else if (item < 3.65)
                {
                    count_picchi_posizione = 0;
                    posizione = "Seduto/Sdraiato";
                    OnLaySit(new EventArgs());
                } 
                else if (item < 7.5)
                {
                    count_picchi_posizione++;
                    if ((count_picchi_posizione >= 35 && posizione == "In Piedi") || posizione != "In Piedi")
                    {
                        posizione = "Seduto";
                        OnSit(new EventArgs());
                    }
                }
                else
                {
                    count_picchi_posizione = 0;
                    posizione = "In Piedi";
                    OnStand(new EventArgs());
                }
                if(posizione != posizione_prec)
                {
                    DateTime fine = istante.AddSeconds(campionePosizione * FREQ);
                    // Al posto della WriteLine si scrive su file
                    string str = istante.ToLongTimeString() + " - " + fine.ToLongTimeString() + " " + posizione;
                    //Console.WriteLine(str);
                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    file.WriteLine(str);
                    file.Close();
                    OnInfo(new InfoEventArgs(str, false));
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
            List<double> theta = new List<double>();
            for (int i = -1; i < y.Count - 1; i++)
            {
                #region Rimuovi-Discontinuita'
                double value = 0.0;
                if (i == -1)
                    value = _precedente;
                else
                    value = Math.Atan(y[i] / z[i]);

                double next = Math.Atan(y[i + 1] / z[i + 1]);

                double myVal = value;
                if (i == y.Count - 2)
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

                theta.Add(next);
            }
            List<double> tan = RIFunc(theta);
            
            /*
            double m = Media(tan);
            if ( Math.Abs(m) > 0.1 )
            {
                //DateTime fine = istante.AddSeconds(campioneGirata * FREQ);
                // Al posto della WriteLine si scrive su file
                //string str = istante.ToLongTimeString() + " - " + fine.ToLongTimeString() + " " + girata_prec;
                string str;
                if (m > 0)
                    str = "Girata Sinistra " +  m;
                else
                    str = "Girata Destra " + m;
                StreamWriter file = new StreamWriter(mypath + myFilename, true);
                OnInfo(new InfoEventArgs(str, false));
                file.WriteLine(str);
                file.Close();
            }
            */

            foreach (var item in tan)
            {
                double max = 0.0;
                girata_prec = girata;
                if(item > 0)
                {
                      
                    count_picchi_girata_dx = 0;
                    count_picchi_girata_sx+=item;
                    Console.WriteLine("Sx " + count_picchi_girata_sx);
                    if(count_picchi_girata_sx >= 1.3) //inizio girata a sx
                    {
                        if (count_picchi_girata_sx > max)
                            max = count_picchi_girata_sx;
                        //count_picchi_girata_sx = 0;
                        OnGirataSinistra(new EventArgs());
                        girata = "Sx " + count_picchi_girata_sx.ToString();
                    }
                    
                else if(item < 0)
                {
                    
                    count_picchi_girata_sx = 0;
                    count_picchi_girata_dx+=item;
                    Console.WriteLine("Dx " + count_picchi_girata_dx);
                    if (count_picchi_girata_dx <= -1.3)
                    {
                        //count_picchi_girata_dx = 0;
                        OnGirataDestra(new EventArgs());
                        girata = "Dx " + count_picchi_girata_dx.ToString();
                    }
                }
                if(girata != girata_prec)
                {

                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    OnInfo(new InfoEventArgs(girata, false));
                    file.WriteLine(girata);
                    file.Close();
                }
            }
             
        }


        /*
         * RIFunc (Rapporto incrementale)
         * Input: Una lista di double.
         * Output: Una lista di n-1 double. Fissato di default h = 1.
         */
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
        /*
        bool sD = false, eD = false, sS = false, eS = false;
        double acc_delta = 0.0;
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

                #region vecchio
                
                #region Rimuovi-Discontinuita'
                double value = 0.0;
                if (i == -1)
                    value = _precedente;
                else
                    value = Math.Atan(y[i] / z[i]);

                if (_isUp != 0 || _isDown != 0)
                    isdisc = true;

                double myVal = value;

                double next = Math.Atan(y[i + 1] / z[i + 1]);

                
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
                if(isdisc)
                    value = value - (_isUp * PI) + (_isDown * PI);

                if (i == y.Count - 2)
                {
                    _precedente = next;
                }

                double newDelta = 0.0;
            
                if (i == -1)
                {
                    newDelta = next - _precedente;
                }
                else
                {
                    newDelta = next - value;
                }
                
                if (newDelta < -ANGOLO_GIRATA_LOCALE)
                {//sinistra
                    
                    count_picchi_girata_sx -= newDelta;
                    if(count_picchi_girata_sx >= ANGOLO_GIRATA_TOTALE)
                    {
                        count_picchi_girata_dx = 0;
                        //Console.WriteLine("newDelta " + newDelta);
                        girata = "Girata a Sinistra";
                        acc_delta += newDelta;
                        OnGirataSinistra(new EventArgs());
                    }
                }
                else if (newDelta > ANGOLO_GIRATA_LOCALE)
                {//destra
                    
                    count_picchi_girata_dx += newDelta;
                    if(count_picchi_girata_dx >= ANGOLO_GIRATA_TOTALE)
                    {
                        count_picchi_girata_sx = 0;
                        //Console.WriteLine("newDelta " + newDelta);
                        girata = "Girata a Destra";
                        acc_delta += newDelta;
                        OnGirataDestra(new EventArgs());
                    }
                   
                }
                
                if (girata != girata_prec && !primo && girata_prec != "")
                {
                    acc_delta -= newDelta;

                    double gradi = acc_delta * 180 / PI;
                    

                    DateTime fine = istante.AddSeconds(campioneGirata * FREQ);
                    // Al posto della WriteLine si scrive su file
                    string str = istante.ToLongTimeString() + " - " + fine.ToLongTimeString() + " " + girata_prec ;
                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    OnInfo(new InfoEventArgs(str, false));
                    file.WriteLine(str);
                    file.Close();

                    acc_delta = newDelta;
                }
                
                #endregion

                if (primo) //Per rimuovere il problema del primo valore = 0
                {
                    primo = !primo;
                    girata = "";
                }
            }
        }
        */
        private void AnalyzeMoto(Window e)
        {
            
            List<double> moduloAcc = e.ModuloAccelerometro(e.matrice);
            List <double> devstd = e.DeviazioneStandard(moduloAcc);
            
            foreach (var item in devstd)
            {
                campioneMovimento++;
                DateTime istante = start.AddSeconds(campioneMovimento * FREQ);
                movimento_prec = movimento;
                if (item > 0.3)
                {
                    count_picchi_moto = 0;
                    movimento = "Moto";
                    OnMoto(new EventArgs());
                }
                else
                {
                    count_picchi_moto++;
                    if (count_picchi_moto >= 40)
                    {
                        count_picchi_moto = 0;
                        movimento = "Stazionamento";
                        OnStazionamento(new EventArgs());
                    }
                }
                //Console.WriteLine("Count: " + count_picchi + " Campione #" + campioneMovimento*FREQ);
                if (movimento != movimento_prec)
                {
                    DateTime fine = istante.AddSeconds(campioneMovimento * FREQ);
                    // Al posto della WriteLine si scrive su file
                    string str = istante.ToLongTimeString() + " - " + fine.ToLongTimeString() + " " + movimento;
                    //Console.WriteLine(str);
                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    file.WriteLine(str);
                    file.Close();
                    OnInfo(new InfoEventArgs(str, false));
                }
            }
        }

        /*
         * Media
         * Input: Lista di double
         * Output: La media dei valori contenuti nella lista.
         */
        public double Media(List<double> l)
        {
            double sum = 0;
            foreach (double item in l)
            {
                sum += item;
            }
            return sum / l.Count;
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

        protected virtual void OnInfo(InfoEventArgs e) { if (Info != null) Info(this, e); }
        #endregion
    }
}
