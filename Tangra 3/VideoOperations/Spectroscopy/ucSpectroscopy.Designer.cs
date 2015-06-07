namespace Tangra.VideoOperations.Spectroscopy
{
    partial class ucSpectroscopy
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.picSpectra = new System.Windows.Forms.PictureBox();
            this.btnMeasure = new System.Windows.Forms.Button();
            this.lblSelectStarNote = new System.Windows.Forms.Label();
            this.picSpectraGraph = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // picSpectra
            // 
            this.picSpectra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSpectra.Location = new System.Drawing.Point(13, 16);
            this.picSpectra.Name = "picSpectra";
            this.picSpectra.Size = new System.Drawing.Size(221, 27);
            this.picSpectra.TabIndex = 0;
            this.picSpectra.TabStop = false;
            this.picSpectra.Visible = false;
            // 
            // btnMeasure
            // 
            this.btnMeasure.Enabled = false;
            this.btnMeasure.Location = new System.Drawing.Point(13, 179);
            this.btnMeasure.Name = "btnMeasure";
            this.btnMeasure.Size = new System.Drawing.Size(221, 23);
            this.btnMeasure.TabIndex = 1;
            this.btnMeasure.Text = "Multi-Frame Measurement";
            this.btnMeasure.UseVisualStyleBackColor = true;
            this.btnMeasure.Click += new System.EventHandler(this.btnMeasure_Click);
            // 
            // lblSelectStarNote
            // 
            this.lblSelectStarNote.AutoSize = true;
            this.lblSelectStarNote.Location = new System.Drawing.Point(42, 22);
            this.lblSelectStarNote.Name = "lblSelectStarNote";
            this.lblSelectStarNote.Size = new System.Drawing.Size(154, 13);
            this.lblSelectStarNote.TabIndex = 2;
            this.lblSelectStarNote.Text = "Please select a star to measure";
            // 
            // picSpectraGraph
            // 
            this.picSpectraGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSpectraGraph.Location = new System.Drawing.Point(13, 49);
            this.picSpectraGraph.Name = "picSpectraGraph";
            this.picSpectraGraph.Size = new System.Drawing.Size(221, 124);
            this.picSpectraGraph.TabIndex = 3;
            this.picSpectraGraph.TabStop = false;
            this.picSpectraGraph.Visible = false;
            // 
            // ucSpectroscopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picSpectraGraph);
            this.Controls.Add(this.lblSelectStarNote);
            this.Controls.Add(this.btnMeasure);
            this.Controls.Add(this.picSpectra);
            this.Name = "ucSpectroscopy";
            this.Size = new System.Drawing.Size(249, 243);
            ((System.ComponentModel.ISupportInitialize)(this.picSpectra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectraGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.PictureBox picSpectra;
		private System.Windows.Forms.Button btnMeasure;
		private System.Windows.Forms.Label lblSelectStarNote;
        private System.Windows.Forms.PictureBox picSpectraGraph;
    }
}
