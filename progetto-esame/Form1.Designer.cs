namespace progetto_esame
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.zedGraphAccelerometro = new ZedGraph.ZedGraphControl();
            this.zedGraphGiroscopio = new ZedGraph.ZedGraphControl();
            this.zedGraphOrientamento = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zedGraphAccelerometro
            // 
            this.zedGraphAccelerometro.Location = new System.Drawing.Point(12, 12);
            this.zedGraphAccelerometro.Name = "zedGraphAccelerometro";
            this.zedGraphAccelerometro.ScrollGrace = 0D;
            this.zedGraphAccelerometro.ScrollMaxX = 0D;
            this.zedGraphAccelerometro.ScrollMaxY = 0D;
            this.zedGraphAccelerometro.ScrollMaxY2 = 0D;
            this.zedGraphAccelerometro.ScrollMinX = 0D;
            this.zedGraphAccelerometro.ScrollMinY = 0D;
            this.zedGraphAccelerometro.ScrollMinY2 = 0D;
            this.zedGraphAccelerometro.Size = new System.Drawing.Size(653, 338);
            this.zedGraphAccelerometro.TabIndex = 0;
            this.zedGraphAccelerometro.Load += new System.EventHandler(this.zedGraphAccelerometro_Load);
            // 
            // zedGraphGiroscopio
            // 
            this.zedGraphGiroscopio.Location = new System.Drawing.Point(12, 356);
            this.zedGraphGiroscopio.Name = "zedGraphGiroscopio";
            this.zedGraphGiroscopio.ScrollGrace = 0D;
            this.zedGraphGiroscopio.ScrollMaxX = 0D;
            this.zedGraphGiroscopio.ScrollMaxY = 0D;
            this.zedGraphGiroscopio.ScrollMaxY2 = 0D;
            this.zedGraphGiroscopio.ScrollMinX = 0D;
            this.zedGraphGiroscopio.ScrollMinY = 0D;
            this.zedGraphGiroscopio.ScrollMinY2 = 0D;
            this.zedGraphGiroscopio.Size = new System.Drawing.Size(653, 338);
            this.zedGraphGiroscopio.TabIndex = 1;
            this.zedGraphGiroscopio.Load += new System.EventHandler(this.zedGraphGiroscopio_Load);
            // 
            // zedGraphOrientamento
            // 
            this.zedGraphOrientamento.Location = new System.Drawing.Point(679, 12);
            this.zedGraphOrientamento.Name = "zedGraphOrientamento";
            this.zedGraphOrientamento.ScrollGrace = 0D;
            this.zedGraphOrientamento.ScrollMaxX = 0D;
            this.zedGraphOrientamento.ScrollMaxY = 0D;
            this.zedGraphOrientamento.ScrollMaxY2 = 0D;
            this.zedGraphOrientamento.ScrollMinX = 0D;
            this.zedGraphOrientamento.ScrollMinY = 0D;
            this.zedGraphOrientamento.ScrollMinY2 = 0D;
            this.zedGraphOrientamento.Size = new System.Drawing.Size(653, 338);
            this.zedGraphOrientamento.TabIndex = 2;
            this.zedGraphOrientamento.Load += new System.EventHandler(this.zedGraphOrientamento_Load);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 697);
            this.Controls.Add(this.zedGraphOrientamento);
            this.Controls.Add(this.zedGraphGiroscopio);
            this.Controls.Add(this.zedGraphAccelerometro);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progetto";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphAccelerometro;
        private ZedGraph.ZedGraphControl zedGraphGiroscopio;
        private ZedGraph.ZedGraphControl zedGraphOrientamento;
    }
}

