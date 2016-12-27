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
            this.SmoothAcc = new System.Windows.Forms.CheckBox();
            this.SmoothGiro = new System.Windows.Forms.CheckBox();
            this.SmoothTheta = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
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
            this.zedGraphGiroscopio.Location = new System.Drawing.Point(12, 365);
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
            // SmoothAcc
            // 
            this.SmoothAcc.AutoSize = true;
            this.SmoothAcc.Location = new System.Drawing.Point(562, 22);
            this.SmoothAcc.Name = "SmoothAcc";
            this.SmoothAcc.Size = new System.Drawing.Size(79, 17);
            this.SmoothAcc.TabIndex = 3;
            this.SmoothAcc.Text = "No Smooth";
            this.SmoothAcc.UseVisualStyleBackColor = true;
            this.SmoothAcc.CheckedChanged += new System.EventHandler(this.SmoothAcc_CheckedChanged);
            // 
            // SmoothGiro
            // 
            this.SmoothGiro.AutoSize = true;
            this.SmoothGiro.Location = new System.Drawing.Point(562, 375);
            this.SmoothGiro.Name = "SmoothGiro";
            this.SmoothGiro.Size = new System.Drawing.Size(79, 17);
            this.SmoothGiro.TabIndex = 4;
            this.SmoothGiro.Text = "No Smooth";
            this.SmoothGiro.UseVisualStyleBackColor = true;
            this.SmoothGiro.CheckedChanged += new System.EventHandler(this.SmoothGiro_CheckedChanged);
            // 
            // SmoothTheta
            // 
            this.SmoothTheta.AutoSize = true;
            this.SmoothTheta.Location = new System.Drawing.Point(1231, 22);
            this.SmoothTheta.Name = "SmoothTheta";
            this.SmoothTheta.Size = new System.Drawing.Size(79, 17);
            this.SmoothTheta.TabIndex = 5;
            this.SmoothTheta.Text = "No Smooth";
            this.SmoothTheta.UseVisualStyleBackColor = true;
            this.SmoothTheta.CheckedChanged += new System.EventHandler(this.SmoothTheta_CheckedChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(679, 365);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(319, 338);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 697);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.SmoothTheta);
            this.Controls.Add(this.SmoothGiro);
            this.Controls.Add(this.SmoothAcc);
            this.Controls.Add(this.zedGraphOrientamento);
            this.Controls.Add(this.zedGraphGiroscopio);
            this.Controls.Add(this.zedGraphAccelerometro);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progetto";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphAccelerometro;
        private ZedGraph.ZedGraphControl zedGraphGiroscopio;
        private ZedGraph.ZedGraphControl zedGraphOrientamento;
        private System.Windows.Forms.CheckBox SmoothAcc;
        private System.Windows.Forms.CheckBox SmoothGiro;
        private System.Windows.Forms.CheckBox SmoothTheta;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

