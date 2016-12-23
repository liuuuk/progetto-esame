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
        

        public Analyzer()
        {

        }
        //Questo metodo viene eseguito quando c'è una nuova finestra da alizzare
        public void Run(object sender, Window e)
        {
            //Nel paramentro e ho i dati di questa finestra da analizzare
            analyzeGirata(e);
            AnalyzeMoto(e);
            AnalyzePosizionamento(e);//magari cambiamo il nome
        }


        //Ognuno dei seguenti metodi secondo la propria logica genera l'evento a lui associato
        private void AnalyzePosizionamento(Window e)
        {
            throw new NotImplementedException();
        }

        private void analyzeGirata(Window e)
        {
            throw new NotImplementedException();
        }

        private void AnalyzeMoto(Window e)
        {
            throw new NotImplementedException();
        }

        
    }
}
