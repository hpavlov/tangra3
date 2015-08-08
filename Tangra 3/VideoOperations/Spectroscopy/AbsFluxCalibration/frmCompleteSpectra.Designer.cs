namespace Tangra.VideoOperations.Spectroscopy.AbsFluxCalibration
{
    partial class frmCompleteSpectra
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
            this.rbProgramStar = new System.Windows.Forms.RadioButton();
            this.rbCalibrationStar = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rbProgramStar
            // 
            this.rbProgramStar.AutoSize = true;
            this.rbProgramStar.Checked = true;
            this.rbProgramStar.Location = new System.Drawing.Point(26, 25);
            this.rbProgramStar.Name = "rbProgramStar";
            this.rbProgramStar.Size = new System.Drawing.Size(122, 17);
            this.rbProgramStar.TabIndex = 0;
            this.rbProgramStar.TabStop = true;
            this.rbProgramStar.Text = "Add as Program Star";
            this.rbProgramStar.UseVisualStyleBackColor = true;
            // 
            // rbCalibrationStar
            // 
            this.rbCalibrationStar.AutoSize = true;
            this.rbCalibrationStar.Location = new System.Drawing.Point(26, 48);
            this.rbCalibrationStar.Name = "rbCalibrationStar";
            this.rbCalibrationStar.Size = new System.Drawing.Size(132, 17);
            this.rbCalibrationStar.TabIndex = 1;
            this.rbCalibrationStar.Text = "Add as Calibration Star";
            this.rbCalibrationStar.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(135, 160);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(216, 160);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // frmCompleteSpectra
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 196);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.rbCalibrationStar);
            this.Controls.Add(this.rbProgramStar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCompleteSpectra";
            this.Text = "Add Spectra";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbProgramStar;
        private System.Windows.Forms.RadioButton rbCalibrationStar;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}