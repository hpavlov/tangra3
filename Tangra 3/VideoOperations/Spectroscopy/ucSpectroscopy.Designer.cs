﻿namespace Tangra.VideoOperations.Spectroscopy
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
            this.btnShowSpectra = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picSpectra)).BeginInit();
            this.SuspendLayout();
            // 
            // picSpectra
            // 
            this.picSpectra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picSpectra.Location = new System.Drawing.Point(13, 16);
            this.picSpectra.Name = "picSpectra";
            this.picSpectra.Size = new System.Drawing.Size(221, 46);
            this.picSpectra.TabIndex = 0;
            this.picSpectra.TabStop = false;
            this.picSpectra.Visible = false;
            // 
            // btnMeasure
            // 
            this.btnMeasure.Enabled = false;
            this.btnMeasure.Location = new System.Drawing.Point(13, 83);
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
            this.lblSelectStarNote.Location = new System.Drawing.Point(22, 25);
            this.lblSelectStarNote.Name = "lblSelectStarNote";
            this.lblSelectStarNote.Size = new System.Drawing.Size(198, 26);
            this.lblSelectStarNote.TabIndex = 2;
            this.lblSelectStarNote.Text = "Please select a star to measure and \r\nthen roughly align the line on the spectra";
            // 
            // btnShowSpectra
            // 
            this.btnShowSpectra.Location = new System.Drawing.Point(13, 83);
            this.btnShowSpectra.Name = "btnShowSpectra";
            this.btnShowSpectra.Size = new System.Drawing.Size(221, 23);
            this.btnShowSpectra.TabIndex = 3;
            this.btnShowSpectra.Text = "Display Spectra";
            this.btnShowSpectra.UseVisualStyleBackColor = true;
            this.btnShowSpectra.Visible = false;
            this.btnShowSpectra.Click += new System.EventHandler(this.btnShowSpectra_Click);
            // 
            // ucSpectroscopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblSelectStarNote);
            this.Controls.Add(this.picSpectra);
            this.Controls.Add(this.btnMeasure);
            this.Controls.Add(this.btnShowSpectra);
            this.Name = "ucSpectroscopy";
            this.Size = new System.Drawing.Size(249, 243);
            ((System.ComponentModel.ISupportInitialize)(this.picSpectra)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.PictureBox picSpectra;
		private System.Windows.Forms.Button btnMeasure;
		private System.Windows.Forms.Label lblSelectStarNote;
		private System.Windows.Forms.Button btnShowSpectra;
    }
}
