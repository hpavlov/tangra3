namespace Tangra.Video.FITS
{
    partial class ucNegativePixelsTreatment
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
            this.label1 = new System.Windows.Forms.Label();
            this.rbMinPixVal = new System.Windows.Forms.RadioButton();
            this.rbZero = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Zero-out negative pixels at:";
            // 
            // rbMinPixVal
            // 
            this.rbMinPixVal.AutoSize = true;
            this.rbMinPixVal.Checked = true;
            this.rbMinPixVal.Location = new System.Drawing.Point(144, 1);
            this.rbMinPixVal.Name = "rbMinPixVal";
            this.rbMinPixVal.Size = new System.Drawing.Size(58, 17);
            this.rbMinPixVal.TabIndex = 26;
            this.rbMinPixVal.TabStop = true;
            this.rbMinPixVal.Text = "-10000";
            this.rbMinPixVal.UseVisualStyleBackColor = true;
            // 
            // rbZero
            // 
            this.rbZero.AutoSize = true;
            this.rbZero.Location = new System.Drawing.Point(236, 1);
            this.rbZero.Name = "rbZero";
            this.rbZero.Size = new System.Drawing.Size(31, 17);
            this.rbZero.TabIndex = 27;
            this.rbZero.Text = "0";
            this.rbZero.UseVisualStyleBackColor = true;
            this.rbZero.CheckedChanged += new System.EventHandler(this.rbZero_CheckedChanged);
            // 
            // ucNegativePixelsTreatment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rbZero);
            this.Controls.Add(this.rbMinPixVal);
            this.Controls.Add(this.label1);
            this.Name = "ucNegativePixelsTreatment";
            this.Size = new System.Drawing.Size(277, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbMinPixVal;
        private System.Windows.Forms.RadioButton rbZero;
    }
}
