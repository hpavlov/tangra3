namespace Tangra.VideoOperations.Spectroscopy
{
    partial class frmViewSpectra
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmViewSpectra));
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlClient = new System.Windows.Forms.Panel();
            this.picSpectra = new System.Windows.Forms.PictureBox();
            this.picSpectraGraph = new System.Windows.Forms.PictureBox();
            this.pnlClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 477);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(789, 74);
            this.pnlBottom.TabIndex = 1;
            // 
            // pnlClient
            // 
            this.pnlClient.Controls.Add(this.picSpectraGraph);
            this.pnlClient.Controls.Add(this.picSpectra);
            this.pnlClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlClient.Location = new System.Drawing.Point(0, 0);
            this.pnlClient.Name = "pnlClient";
            this.pnlClient.Size = new System.Drawing.Size(789, 477);
            this.pnlClient.TabIndex = 2;
            // 
            // picSpectra
            // 
            this.picSpectra.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.picSpectra.Location = new System.Drawing.Point(0, 427);
            this.picSpectra.Name = "picSpectra";
            this.picSpectra.Size = new System.Drawing.Size(789, 50);
            this.picSpectra.TabIndex = 0;
            this.picSpectra.TabStop = false;
            // 
            // picSpectraGraph
            // 
            this.picSpectraGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picSpectraGraph.Location = new System.Drawing.Point(0, 0);
            this.picSpectraGraph.Name = "picSpectraGraph";
            this.picSpectraGraph.Size = new System.Drawing.Size(789, 427);
            this.picSpectraGraph.TabIndex = 1;
            this.picSpectraGraph.TabStop = false;
            // 
            // frmViewSpectra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 551);
            this.Controls.Add(this.pnlClient);
            this.Controls.Add(this.pnlBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmViewSpectra";
            this.Text = "frmViewSpectra";
            this.Load += new System.EventHandler(this.frmViewSpectra_Load);
            this.pnlClient.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSpectra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlClient;
        private System.Windows.Forms.PictureBox picSpectra;
        private System.Windows.Forms.PictureBox picSpectraGraph;
    }
}