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
            this.rbBZero = new System.Windows.Forms.RadioButton();
            this.rbZeroOut = new System.Windows.Forms.RadioButton();
            this.nudBZero = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudBZero)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Negative Pixels Handling:";
            // 
            // rbBZero
            // 
            this.rbBZero.AutoSize = true;
            this.rbBZero.Location = new System.Drawing.Point(137, 8);
            this.rbBZero.Name = "rbBZero";
            this.rbBZero.Size = new System.Drawing.Size(54, 17);
            this.rbBZero.TabIndex = 26;
            this.rbBZero.TabStop = true;
            this.rbBZero.Text = "BZero";
            this.rbBZero.UseVisualStyleBackColor = true;
            this.rbBZero.CheckedChanged += new System.EventHandler(this.rbBZero_CheckedChanged);
            // 
            // rbZeroOut
            // 
            this.rbZeroOut.AutoSize = true;
            this.rbZeroOut.Location = new System.Drawing.Point(311, 8);
            this.rbZeroOut.Name = "rbZeroOut";
            this.rbZeroOut.Size = new System.Drawing.Size(67, 17);
            this.rbZeroOut.TabIndex = 27;
            this.rbZeroOut.TabStop = true;
            this.rbZeroOut.Text = "Zero Out";
            this.rbZeroOut.UseVisualStyleBackColor = true;
            // 
            // nudBZero
            // 
            this.nudBZero.Location = new System.Drawing.Point(197, 7);
            this.nudBZero.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudBZero.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.nudBZero.Name = "nudBZero";
            this.nudBZero.ReadOnly = true;
            this.nudBZero.Size = new System.Drawing.Size(67, 20);
            this.nudBZero.TabIndex = 28;
            this.nudBZero.Value = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            // 
            // ucNegativePixelsTreatment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nudBZero);
            this.Controls.Add(this.rbZeroOut);
            this.Controls.Add(this.rbBZero);
            this.Controls.Add(this.label1);
            this.Name = "ucNegativePixelsTreatment";
            this.Size = new System.Drawing.Size(424, 33);
            ((System.ComponentModel.ISupportInitialize)(this.nudBZero)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbBZero;
        private System.Windows.Forms.RadioButton rbZeroOut;
        private System.Windows.Forms.NumericUpDown nudBZero;
    }
}
