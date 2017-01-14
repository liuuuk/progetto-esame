﻿using System;
using System.Collections.Generic;
using System.IO;
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

        // Soglia delta per discontinuità di theta
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

        // prova
        int count_picchi = 0;


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

        string mypath;

        public Analyzer()
        {
            mypath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            StreamWriter file = new StreamWriter(mypath + @"\Acquisizione.txt", true);
            var newLine = Environment.NewLine;
            file.WriteLine(newLine + newLine + DateTime.Now.ToString() + " nuova acquisizione:" + newLine);
            file.Close();
            //Console.WriteLine(mypath);
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
                    string str = istante.ToLongTimeString() + " - " + fine.ToLongTimeString() + " " + posizione;
                    //Console.WriteLine(str);
                    StreamWriter file = new StreamWriter(mypath + @"\Acquisizione.txt", true);
                    file.WriteLine(str);
                    file.Close();
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

                double myVal = value;

                double next = Math.Atan(y[i + 1] / z[i + 1]);

                if (i == y.Count-1)
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

                double newDelta = 0.0;
                if (i == -1)
                {
                    newDelta = next - _precedente;
                }
                else
                {
                    newDelta = next - value;
                }
                if (newDelta < -ANGOLO_GIRATA)
                {//sinistra
                    girata = "Girata a Sinistra";
                    OnGirataSinistra(new EventArgs());
                }
                else if (newDelta > ANGOLO_GIRATA)
                {//destra
                    girata = "Girata a Destra";
                    OnGirataDestra(new EventArgs());
                }
                if (girata != girata_prec && !primo)
                {
                    DateTime fine = istante.AddSeconds(campioneGirata * FREQ);
                    // Al posto della WriteLine si scrive su file
                    string str = istante.ToLongTimeString() + " - " + fine.ToLongTimeString() + " " + girata + " ";
                    StreamWriter file = new StreamWriter(mypath + @"\Acquisizione.txt", true);
                    //Console.WriteLine(str);
                    file.WriteLine(str);
                    file.Close();
                }
                if (primo) //Per rimuovere il problema del primo valore = 0
                {
                    primo = !primo;
                    girata = "";
                }
            }
        }
        
        private void AnalyzeMoto(Window e)
        {
            
            List<double> moduloAcc = e.ModuloAccelerometro(e.matriceSmooth);
            List<double> devstd = DeviazioneStandard(moduloAcc);
            
            foreach (var item in devstd)
            {
                campioneMovimento++;
                DateTime istante = start.AddSeconds(campioneMovimento * FREQ);
                movimento_prec = movimento;
                if (item > 0.3)
                {
                    count_picchi = 0;
                    movimento = "Moto";
                    OnMoto(new EventArgs());
                }
                else
                {
                    count_picchi++;
                    if (count_picchi >= 40)
                    {
                        count_picchi = 0;
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
                    StreamWriter file = new StreamWriter(mypath + @"\Acquisizione.txt", true);
                    file.WriteLine(str);
                    file.Close();
                }
            }
        }

        private List<double> DeviazioneStandard(List<double> l)
        {
            List<double> result = new List<double>();
            double media = Media(l);
            double sum = 0;
            foreach (double item in l)
            {
                sum = (item - media) * (item - media);
                result.Add(Math.Sqrt(sum / l.Count));
            }
            return result;
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
        #endregion
    }
}
