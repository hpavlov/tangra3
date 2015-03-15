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
            this.rbBias = new System.Windows.Forms.RadioButton();
            this.pnlExposure = new System.Windows.Forms.Panel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlSettings.SuspendLayout();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumFrames)).BeginInit();
            this.pnlExposure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Number Frames to Process:";
            // 
            // pnlSettings
            // 
            this.pnlSettings.Controls.Add(this.pnlExposure);
            this.pnlSettings.Controls.Add(this.groupControl1);
            this.pnlSettings.Controls.Add(this.nudNumFrames);
            this.pnlSettings.Controls.Add(this.label1);
            this.pnlSettings.Location = new System.Drawing.Point(3, 3);
            this.pnlSettings.Name = "pnlSettings";
            this.pnlSettings.Size = new System.Drawing.Size(254, 164);
            this.pnlSettings.TabIndex = 7;
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.rbBias);
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
            this.rbFlat.Location = new System.Drawing.Point(95, 31);
            this.rbFlat.Name = "rbFlat";
            this.rbFlat.Size = new System.Drawing.Size(60, 19);
            this.rbFlat.TabIndex = 12;
            this.rbFlat.Text = "Flat";
            this.rbFlat.CheckedChanged += new System.EventHandler(this.FrameTypeChanged);
            // 
            // rbDark
            // 
            this.rbDark.Checked = true;
            this.rbDark.Location = new System.Drawing.Point(20, 31);
            this.rbDark.Name = "rbDark";
            this.rbDark.Size = new System.Drawing.Size(54, 19);
            this.rbDark.TabIndex = 11;
            this.rbDark.Text = "Dark";
            this.rbDark.CheckedChanged += new System.EventHandler(this.FrameTypeChanged);
            // 
            // nudNumFrames
            // 
            this.nudNumFrames.Location = new System.Drawing.Point(148, 122);
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
            this.btnProcess.Location = new System.Drawing.Point(6, 197);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(146, 23);
            this.btnProcess.TabIndex = 8;
            this.btnProcess.Text = "Produce Averaged Frame";
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // pbar
            // 
            this.pbar.Location = new System.Drawing.Point(6, 173);
            this.pbar.Name = "pbar";
            this.pbar.Size = new System.Drawing.Size(242, 18);
            this.pbar.TabIndex = 9;
            this.pbar.Visible = false;
            // 
            // rbBias
            // 
            this.rbBias.Location = new System.Drawing.Point(172, 31);
            this.rbBias.Name = "rbBias";
            this.rbBias.Size = new System.Drawing.Size(52, 19);
            this.rbBias.TabIndex = 13;
            this.rbBias.Text = "Bias";
            this.rbBias.CheckedChanged += new System.EventHandler(this.FrameTypeChanged);
            // 
            // pnlExposure
            // 
            this.pnlExposure.Controls.Add(this.label3);
            this.pnlExposure.Controls.Add(this.numericUpDown1);
            this.pnlExposure.Controls.Add(this.label2);
            this.pnlExposure.Location = new System.Drawing.Point(1, 72);
            this.pnlExposure.Name = "pnlExposure";
            this.pnlExposure.Size = new System.Drawing.Size(250, 44);
            this.pnlExposure.TabIndex = 11;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 2;
            this.numericUpDown1.Location = new System.Drawing.Point(107, 12);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(62, 20);
            this.numericUpDown1.TabIndex = 8;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Effective Exposure:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(175, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "sec";
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
            this.Load += new System.EventHandler(this.ucMakeDarkFlatField_Load);
            this.pnlSettings.ResumeLayout(false);
            this.pnlSettings.PerformLayout();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudNumFrames)).EndInit();
            this.pnlExposure.ResumeLayout(false);
            this.pnlExposure.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
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
        private System.Windows.Forms.RadioButton rbBias;
        private System.Windows.Forms.Panel pnlExposure;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label2;
    }
}
