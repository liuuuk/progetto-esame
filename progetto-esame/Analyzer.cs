using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progetto_esame
{
    
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
        int count_sx = 0;
        int count_dx = 0;

        private double _precedente;
        private int _isUp;
        private int _isDown;

        private int campionePosizione = 0;
        private int campioneGirata = 0;
        private int campioneMovimento = 0;

        // Tempo della prima acquisizione
        DateTime startPos;
        DateTime startMot;
        DateTime startGir;

        DateTime start;
        DateTime istante = DateTime.Now;
        DateTime inizio;

        bool primaSx = true;
        bool primaDx = true;

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
            startPos = d;
            startMot = d;
            startGir = d;
            PrepareFile();
        }

        public void Run(object sender, Window e)
        {
            AnalyzeGirata(e);
            AnalyzePosizionamento(e);
            AnalyzeMoto(e);

        }

        public void Stop()
        {

            //Posizionamento
            istante = startPos.AddSeconds(campionePosizione * FREQ);

            // Al posto della WriteLine si scrive su file
            string str = startPos.ToLongTimeString() + " - " + istante.ToLongTimeString() + " " + posizione_prec;
            //Console.WriteLine(str);
            StreamWriter file = new StreamWriter(mypath + myFilename, true);
            file.WriteLine(str);
            file.Close();
            OnInfo(new InfoEventArgs(str, false));

            startPos = istante;

            //Movimento
            istante = startMot.AddSeconds(campioneMovimento * FREQ);

            // Al posto della WriteLine si scrive su file
            str = startMot.ToLongTimeString() + " - " + istante.ToLongTimeString() + " " + movimento_prec;
            //Console.WriteLine(str);
            file = new StreamWriter(mypath + myFilename, true);
            file.WriteLine(str);
            file.Close();
            OnInfo(new InfoEventArgs(str, false));

            startMot = istante;



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
                if(posizione != posizione_prec && posizione_prec != "")
                {
                    istante = startPos.AddSeconds(campionePosizione * FREQ);

                    // Al posto della WriteLine si scrive su file
                    string str = startPos.ToLongTimeString() + " - " + istante.ToLongTimeString() + " " + posizione_prec;
                    //Console.WriteLine(str);
                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    file.WriteLine(str);
                    file.Close();
                    OnInfo(new InfoEventArgs(str, false));

                    startPos = istante;
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

        
            double gradi = 0.0;
            foreach (var item in tan)
            {
                campioneGirata++;
                //istante = start.AddSeconds(campioneGirata * FREQ);
                //DateTime fine = istante;
                girata_prec = girata;
                if (item > 0)
                {
                    
                    //Girata a sx
                    count_sx++;
                    OnGirataSinistra(new EventArgs());
                    if (count_picchi_girata_dx <= -1.3 && count_dx < 35)
                    {
                        inizio = startGir;
                        istante = startGir.AddSeconds(campioneGirata * FREQ);
                        girata = "Dx ";
                        gradi = count_picchi_girata_dx;
                    }
                    if (primaSx)
                    {
                        startGir = istante;
                        primaSx = false;
                    }
                    primaDx = true;
                    count_dx = 0;
                    count_picchi_girata_dx = 0;
                    count_picchi_girata_sx += item;
                    //Console.WriteLine("Sx " + count_picchi_girata_sx);
                    
                }
                else if (item < 0)
                {
                    
                    //Destra
                    count_dx++;
                    OnGirataDestra(new EventArgs());
                    if (count_picchi_girata_sx >= 1.3 && count_sx < 35)
                    {
                        inizio = startGir;
                        istante = startGir.AddSeconds(campioneGirata * FREQ);
                        girata = "Sx ";
                        gradi = count_picchi_girata_sx;
                    }
                    if (primaDx)
                    {
                        startGir = istante;
                        primaDx = false;
                    }
                    primaSx = true;

                    count_sx = 0;
                    count_picchi_girata_sx = 0;
                    count_picchi_girata_dx += item;
                    //Console.WriteLine("Dx " + count_picchi_girata_dx);
                    
                }
               
                if(girata != girata_prec)
                {
                    Console.WriteLine("Girata " + girata + " " + gradi);
                    gradi = Math.Abs(gradi);
                    //Da vedere
                    string angolo = "";
                    if (gradi > 2.6)
                        angolo = ">180";
                    else if (gradi <= 2.6 && gradi > 1.65)
                        angolo = "180";
                    else if (gradi >= 1.3 && gradi <= 1.65) // 1.6 per il file camm semp 2 - 1.65 per file camm 
                        angolo = "90";
                    else
                        angolo = "???";

                    

                    // Al posto della WriteLine si scrive su file
                    string str = inizio.ToLongTimeString() + " - " + istante.ToLongTimeString() + " " + girata + " " + angolo;
                    //Console.WriteLine(str);
                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    file.WriteLine(str);
                    file.Close();
                    OnInfo(new InfoEventArgs(str, false));

                    
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
        
        private void AnalyzeMoto(Window e)
        {
            
            List<double> moduloAcc = e.ModuloAccelerometro(e.matrice);
            List <double> devstd = e.DeviazioneStandard(moduloAcc);
            
            foreach (var item in devstd)
            {
                campioneMovimento++;
                istante = start.AddSeconds(campioneMovimento * FREQ);
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
                if (movimento != movimento_prec && movimento_prec != "")
                {
                    istante = startMot.AddSeconds(campioneMovimento * FREQ);

                    // Al posto della WriteLine si scrive su file
                    string str = startMot.ToLongTimeString() + " - " + istante.ToLongTimeString() + " " + movimento_prec;
                    //Console.WriteLine(str);
                    StreamWriter file = new StreamWriter(mypath + myFilename, true);
                    file.WriteLine(str);
                    file.Close();
                    OnInfo(new InfoEventArgs(str, false));

                    startMot = istante;
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
