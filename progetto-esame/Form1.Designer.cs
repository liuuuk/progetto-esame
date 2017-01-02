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
            this.pictureAzione = new System.Windows.Forms.PictureBox();
            this.LabelMoto = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.LabelPosizione = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureAzione)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            // pictureAzione
            // 
            this.pictureAzione.BackColor = System.Drawing.SystemColors.Control;
            this.pictureAzione.Image = global::progetto_esame.Properties.Resources.stickman_no_walk;
            this.pictureAzione.Location = new System.Drawing.Point(762, 365);
            this.pictureAzione.Name = "pictureAzione";
            this.pictureAzione.Size = new System.Drawing.Size(50, 50);
            this.pictureAzione.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureAzione.TabIndex = 5;
            this.pictureAzione.TabStop = false;
            // 
            // LabelMoto
            // 
            this.LabelMoto.AutoSize = true;
            this.LabelMoto.Location = new System.Drawing.Point(679, 385);
            this.LabelMoto.Name = "LabelMoto";
            this.LabelMoto.Size = new System.Drawing.Size(77, 13);
            this.LabelMoto.TabIndex = 6;
            this.LabelMoto.Text = "Stazionamento";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(679, 372);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Azione:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(679, 440);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Posizione:";
            // 
            // LabelPosizione
            // 
            this.LabelPosizione.AutoSize = true;
            this.LabelPosizione.Location = new System.Drawing.Point(679, 453);
            this.LabelPosizione.Name = "LabelPosizione";
            this.LabelPosizione.Size = new System.Drawing.Size(41, 13);
            this.LabelPosizione.TabIndex = 9;
            this.LabelPosizione.Text = "In piedi";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::progetto_esame.Properties.Resources.stickman_no_walk;
            this.pictureBox1.Location = new System.Drawing.Point(762, 440);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1344, 717);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.LabelPosizione);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LabelMoto);
            this.Controls.Add(this.pictureAzione);
            this.Controls.Add(this.SmoothGiro);
            this.Controls.Add(this.SmoothAcc);
            this.Controls.Add(this.zedGraphOrientamento);
            this.Controls.Add(this.zedGraphGiroscopio);
            this.Controls.Add(this.zedGraphAccelerometro);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Progetto";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pictureAzione)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphAccelerometro;
        private ZedGraph.ZedGraphControl zedGraphGiroscopio;
        private ZedGraph.ZedGraphControl zedGraphOrientamento;
        private System.Windows.Forms.CheckBox SmoothAcc;
        private System.Windows.Forms.CheckBox SmoothGiro;
        private System.Windows.Forms.PictureBox pictureAzione;
        private System.Windows.Forms.Label LabelMoto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label LabelPosizione;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

