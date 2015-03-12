namespace Tangra.VideoOperations.MakeDarkFlatField
{
    partial class ucMakeDarkFlatField
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
			this.pnlSettings = new System.Windows.Forms.Panel();
			this.groupControl1 = new System.Windows.Forms.GroupBox();
			this.rbFlat = new System.Windows.Forms.RadioButton();
			this.rbDark = new System.Windows.Forms.RadioButton();
			this.nudNumFrames = new System.Windows.Forms.NumericUpDown();
			this.btnProcess = new System.Windows.Forms.Button();
			this.pbar = new System.Windows.Forms.ProgressBar();
			this.pnlSettings.SuspendLayout();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumFrames)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 89);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(137, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Number Frames to Process:";
			// 
			// pnlSettings
			// 
			this.pnlSettings.Controls.Add(this.groupControl1);
			this.pnlSettings.Controls.Add(this.nudNumFrames);
			this.pnlSettings.Controls.Add(this.label1);
			this.pnlSettings.Location = new System.Drawing.Point(3, 3);
			this.pnlSettings.Name = "pnlSettings";
			this.pnlSettings.Size = new System.Drawing.Size(254, 121);
			this.pnlSettings.TabIndex = 7;
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.rbFlat);
			this.groupControl1.Controls.Add(this.rbDark);
			this.groupControl1.Location = new System.Drawing.Point(3, 3);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(242, 63);
			this.groupControl1.TabIndex = 10;
			this.groupControl1.TabStop = false;
			this.groupControl1.Text = "Type";
			// 
			// rbFlat
			// 
			this.rbFlat.Location = new System.Drawing.Point(132, 31);
			this.rbFlat.Name = "rbFlat";
			this.rbFlat.Size = new System.Drawing.Size(75, 19);
			this.rbFlat.TabIndex = 12;
			this.rbFlat.Text = "Flat Frame";
			this.rbFlat.CheckedChanged += new System.EventHandler(this.rbFlat_CheckedChanged);
			// 
			// rbDark
			// 
			this.rbDark.Checked = true;
			this.rbDark.Location = new System.Drawing.Point(32, 31);
			this.rbDark.Name = "rbDark";
			this.rbDark.Size = new System.Drawing.Size(75, 19);
			this.rbDark.TabIndex = 11;
			this.rbDark.TabStop = true;
			this.rbDark.Text = "Dark Frame";
			// 
			// nudNumFrames
			// 
			this.nudNumFrames.Location = new System.Drawing.Point(146, 84);
			this.nudNumFrames.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
			this.nudNumFrames.Name = "nudNumFrames";
			this.nudNumFrames.Size = new System.Drawing.Size(57, 20);
			this.nudNumFrames.TabIndex = 6;
			this.nudNumFrames.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
			// 
			// btnProcess
			// 
			this.btnProcess.Location = new System.Drawing.Point(3, 168);
			this.btnProcess.Name = "btnProcess";
			this.btnProcess.Size = new System.Drawing.Size(146, 23);
			this.btnProcess.TabIndex = 8;
			this.btnProcess.Text = "Produce Averaged Frame";
			this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
			// 
			// pbar
			// 
			this.pbar.Location = new System.Drawing.Point(3, 144);
			this.pbar.Name = "pbar";
			this.pbar.Size = new System.Drawing.Size(243, 18);
			this.pbar.TabIndex = 9;
			this.pbar.Visible = false;
			// 
			// ucMakeDarkFlatField
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.pbar);
			this.Controls.Add(this.btnProcess);
			this.Controls.Add(this.pnlSettings);
			this.Name = "ucMakeDarkFlatField";
			this.Size = new System.Drawing.Size(264, 262);
			this.pnlSettings.ResumeLayout(false);
			this.pnlSettings.PerformLayout();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nudNumFrames)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlSettings;
		private System.Windows.Forms.Button btnProcess;
		private System.Windows.Forms.NumericUpDown nudNumFrames;
		private System.Windows.Forms.ProgressBar pbar;
		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.RadioButton rbFlat;
		private System.Windows.Forms.RadioButton rbDark;
    }
}
