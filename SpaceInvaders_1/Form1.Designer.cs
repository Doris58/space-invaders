namespace SpaceInvaders_1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.letjelica = new System.Windows.Forms.PictureBox();
            this.labelaZivoti = new System.Windows.Forms.Label();
            this.labelaBodovi = new System.Windows.Forms.Label();
            this.gumbStart = new System.Windows.Forms.Button();
            this.formaStartSlika2 = new System.Windows.Forms.PictureBox();
            this.formaStartSlika1 = new System.Windows.Forms.PictureBox();
            this.labelaNaslov = new System.Windows.Forms.Label();
            this.labelaRazina = new System.Windows.Forms.Label();
            this.labelaScore = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.letjelica)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formaStartSlika2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formaStartSlika1)).BeginInit();
            this.SuspendLayout();
            // 
            // letjelica
            // 
            this.letjelica.BackColor = System.Drawing.Color.Transparent;
            this.letjelica.Image = global::SpaceInvaders_1.Properties.Resources.letjelica;
            this.letjelica.Location = new System.Drawing.Point(303, 344);
            this.letjelica.Margin = new System.Windows.Forms.Padding(2);
            this.letjelica.Name = "letjelica";
            this.letjelica.Size = new System.Drawing.Size(64, 54);
            this.letjelica.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.letjelica.TabIndex = 1;
            this.letjelica.TabStop = false;
            // 
            // labelaZivoti
            // 
            this.labelaZivoti.AutoSize = true;
            this.labelaZivoti.BackColor = System.Drawing.Color.Transparent;
            this.labelaZivoti.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelaZivoti.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelaZivoti.Location = new System.Drawing.Point(0, 0);
            this.labelaZivoti.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelaZivoti.Name = "labelaZivoti";
            this.labelaZivoti.Size = new System.Drawing.Size(104, 26);
            this.labelaZivoti.TabIndex = 2;
            this.labelaZivoti.Text = "Životi:  3";
            // 
            // labelaBodovi
            // 
            this.labelaBodovi.AutoSize = true;
            this.labelaBodovi.BackColor = System.Drawing.Color.Transparent;
            this.labelaBodovi.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelaBodovi.ForeColor = System.Drawing.Color.White;
            this.labelaBodovi.Location = new System.Drawing.Point(272, -1);
            this.labelaBodovi.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelaBodovi.Name = "labelaBodovi";
            this.labelaBodovi.Size = new System.Drawing.Size(112, 26);
            this.labelaBodovi.TabIndex = 3;
            this.labelaBodovi.Text = "Bodovi: 0";
            // 
            // gumbStart
            // 
            this.gumbStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.gumbStart.AutoSize = true;
            this.gumbStart.BackColor = System.Drawing.Color.MediumOrchid;
            this.gumbStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.gumbStart.ForeColor = System.Drawing.Color.White;
            this.gumbStart.Location = new System.Drawing.Point(142, 254);
            this.gumbStart.Name = "gumbStart";
            this.gumbStart.Size = new System.Drawing.Size(130, 40);
            this.gumbStart.TabIndex = 4;
            this.gumbStart.Text = "button1";
            this.gumbStart.UseVisualStyleBackColor = false;
            this.gumbStart.Click += new System.EventHandler(this.GumbStart_Click);
            // 
            // formaStartSlika2
            // 
            this.formaStartSlika2.BackColor = System.Drawing.Color.Transparent;
            this.formaStartSlika2.Location = new System.Drawing.Point(32, 305);
            this.formaStartSlika2.Name = "formaStartSlika2";
            this.formaStartSlika2.Size = new System.Drawing.Size(78, 72);
            this.formaStartSlika2.TabIndex = 6;
            this.formaStartSlika2.TabStop = false;
            // 
            // formaStartSlika1
            // 
            this.formaStartSlika1.BackColor = System.Drawing.Color.Transparent;
            this.formaStartSlika1.Location = new System.Drawing.Point(287, 54);
            this.formaStartSlika1.Name = "formaStartSlika1";
            this.formaStartSlika1.Size = new System.Drawing.Size(69, 68);
            this.formaStartSlika1.TabIndex = 7;
            this.formaStartSlika1.TabStop = false;
            // 
            // labelaNaslov
            // 
            this.labelaNaslov.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelaNaslov.AutoSize = true;
            this.labelaNaslov.Font = new System.Drawing.Font("Segoe Script", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelaNaslov.Location = new System.Drawing.Point(131, 161);
            this.labelaNaslov.Name = "labelaNaslov";
            this.labelaNaslov.Size = new System.Drawing.Size(149, 61);
            this.labelaNaslov.TabIndex = 5;
            this.labelaNaslov.Text = "label2";
            this.labelaNaslov.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelaRazina
            // 
            this.labelaRazina.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelaRazina.AutoSize = true;
            this.labelaRazina.BackColor = System.Drawing.Color.Transparent;
            this.labelaRazina.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelaRazina.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelaRazina.Location = new System.Drawing.Point(137, -1);
            this.labelaRazina.Name = "labelaRazina";
            this.labelaRazina.Size = new System.Drawing.Size(99, 25);
            this.labelaRazina.TabIndex = 8;
            this.labelaRazina.Text = "Razina: \r\n";
            // 
            // labelaScore
            // 
            this.labelaScore.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelaScore.AutoSize = true;
            this.labelaScore.BackColor = System.Drawing.Color.Transparent;
            this.labelaScore.Font = new System.Drawing.Font("Corbel", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.labelaScore.ForeColor = System.Drawing.Color.White;
            this.labelaScore.Location = new System.Drawing.Point(154, 161);
            this.labelaScore.Name = "labelaScore";
            this.labelaScore.Size = new System.Drawing.Size(99, 39);
            this.labelaScore.TabIndex = 9;
            this.labelaScore.Text = "label1";
            this.labelaScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(384, 409);
            this.Controls.Add(this.labelaScore);
            this.Controls.Add(this.labelaRazina);
            this.Controls.Add(this.labelaNaslov);
            this.Controls.Add(this.gumbStart);
            this.Controls.Add(this.labelaBodovi);
            this.Controls.Add(this.labelaZivoti);
            this.Controls.Add(this.letjelica);
            this.Controls.Add(this.formaStartSlika1);
            this.Controls.Add(this.formaStartSlika2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Space Invaders";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint_1);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.letjelica)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formaStartSlika2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formaStartSlika1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox letjelica;
        private System.Windows.Forms.Label labelaZivoti;
        private System.Windows.Forms.Label labelaBodovi;
        private System.Windows.Forms.Button gumbStart;
        private System.Windows.Forms.PictureBox formaStartSlika2;
        private System.Windows.Forms.PictureBox formaStartSlika1;
        private System.Windows.Forms.Label labelaNaslov;
        private System.Windows.Forms.Label labelaRazina;
        private System.Windows.Forms.Label labelaScore;
    }
}