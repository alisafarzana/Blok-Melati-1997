namespace OOP_Project
{
    partial class Start
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
            this.arrow = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.arrow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // arrow
            // 
            this.arrow.BackColor = System.Drawing.Color.Transparent;
            this.arrow.Image = global::OOP_Project.Properties.Resources.IMG_7712;
            this.arrow.Location = new System.Drawing.Point(519, 304);
            this.arrow.Name = "arrow";
            this.arrow.Size = new System.Drawing.Size(60, 58);
            this.arrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.arrow.TabIndex = 0;
            this.arrow.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::OOP_Project.Properties.Resources.Untitled72_20260326220924;
            this.pictureBox1.Location = new System.Drawing.Point(-1, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1347, 664);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Start
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::OOP_Project.Properties.Resources.Untitled72_20260326220924;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1345, 662);
            this.Controls.Add(this.arrow);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Start";
            this.Text = "Blok Melati 1997";
            this.Load += new System.EventHandler(this.Start_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Start_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.arrow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox arrow;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}