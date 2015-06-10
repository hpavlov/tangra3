namespace Tangra.VideoOperations.Spectroscopy
{
	partial class frmRunMultiFrameSpectroscopy
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunMultiFrameSpectroscopy));
			this.gbConfig = new System.Windows.Forms.GroupBox();
			this.picAreas = new System.Windows.Forms.PictureBox();
			this.cbxCombineMethod = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.nudBackgroundWing = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nudAreaWing = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbxBackgroundMethod = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.ucFrameInterval = new Tangra.VideoOperations.Astrometry.ucFrameInterval();
			this.label3 = new System.Windows.Forms.Label();
			this.nudNumberMeasurements = new System.Windows.Forms.NumericUpDown();
			this.btnPrevious = new System.Windows.Forms.Button();
			this.btnNext = new System.Windows.Forms.Button();
			this.gbConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.picAreas)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBackgroundWing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAreaWing)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).BeginInit();
			this.SuspendLayout();
			// 
			// gbConfig
			// 
			this.gbConfig.Controls.Add(this.picAreas);
			this.gbConfig.Controls.Add(this.cbxCombineMethod);
			this.gbConfig.Controls.Add(this.label6);
			this.gbConfig.Controls.Add(this.nudBackgroundWing);
			this.gbConfig.Controls.Add(this.label7);
			this.gbConfig.Controls.Add(this.label4);
			this.gbConfig.Controls.Add(this.nudAreaWing);
			this.gbConfig.Controls.Add(this.label2);
			this.gbConfig.Controls.Add(this.label1);
			this.gbConfig.Controls.Add(this.cbxBackgroundMethod);
			this.gbConfig.Controls.Add(this.label5);
			this.gbConfig.Controls.Add(this.ucFrameInterval);
			this.gbConfig.Controls.Add(this.label3);
			this.gbConfig.Controls.Add(this.nudNumberMeasurements);
			this.gbConfig.Location = new System.Drawing.Point(12, 13);
			this.gbConfig.Name = "gbConfig";
			this.gbConfig.Size = new System.Drawing.Size(441, 282);
			this.gbConfig.TabIndex = 19;
			this.gbConfig.TabStop = false;
			this.gbConfig.Text = "Spectroscopy";
			// 
			// picAreas
			// 
			this.picAreas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picAreas.Location = new System.Drawing.Point(11, 196);
			this.picAreas.Name = "picAreas";
			this.picAreas.Size = new System.Drawing.Size(419, 74);
			this.picAreas.TabIndex = 36;
			this.picAreas.TabStop = false;
			// 
			// cbxCombineMethod
			// 
			this.cbxCombineMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxCombineMethod.FormattingEnabled = true;
			this.cbxCombineMethod.Items.AddRange(new object[] {
            "Averages",
            "Medians"});
			this.cbxCombineMethod.Location = new System.Drawing.Point(242, 30);
			this.cbxCombineMethod.Name = "cbxCombineMethod";
			this.cbxCombineMethod.Size = new System.Drawing.Size(84, 21);
			this.cbxCombineMethod.TabIndex = 35;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(203, 139);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(33, 13);
			this.label6.TabIndex = 34;
			this.label6.Text = "pixels";
			// 
			// nudBackgroundWing
			// 
			this.nudBackgroundWing.Location = new System.Drawing.Point(155, 137);
			this.nudBackgroundWing.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.nudBackgroundWing.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudBackgroundWing.Name = "nudBackgroundWing";
			this.nudBackgroundWing.Size = new System.Drawing.Size(42, 20);
			this.nudBackgroundWing.TabIndex = 33;
			this.nudBackgroundWing.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudBackgroundWing.ValueChanged += new System.EventHandler(this.nudBackgroundWing_ValueChanged);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(29, 137);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(117, 13);
			this.label7.TabIndex = 32;
			this.label7.Text = "Background area wing:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(203, 113);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(33, 13);
			this.label4.TabIndex = 31;
			this.label4.Text = "pixels";
			// 
			// nudAreaWing
			// 
			this.nudAreaWing.Location = new System.Drawing.Point(155, 111);
			this.nudAreaWing.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.nudAreaWing.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudAreaWing.Name = "nudAreaWing";
			this.nudAreaWing.Size = new System.Drawing.Size(42, 20);
			this.nudAreaWing.TabIndex = 30;
			this.nudAreaWing.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.nudAreaWing.ValueChanged += new System.EventHandler(this.nudAreaWing_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(23, 113);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(123, 13);
			this.label2.TabIndex = 29;
			this.label2.Text = "Measurement area wing:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(40, 166);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 13);
			this.label1.TabIndex = 28;
			this.label1.Text = "Background method:";
			// 
			// cbxBackgroundMethod
			// 
			this.cbxBackgroundMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxBackgroundMethod.FormattingEnabled = true;
			this.cbxBackgroundMethod.Items.AddRange(new object[] {
            "Average Background",
            "Median Background"});
			this.cbxBackgroundMethod.Location = new System.Drawing.Point(155, 163);
			this.cbxBackgroundMethod.Name = "cbxBackgroundMethod";
			this.cbxBackgroundMethod.Size = new System.Drawing.Size(175, 21);
			this.cbxBackgroundMethod.TabIndex = 27;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(23, 33);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 13);
			this.label5.TabIndex = 26;
			this.label5.Text = "Combine";
			// 
			// ucFrameInterval
			// 
			this.ucFrameInterval.Location = new System.Drawing.Point(16, 57);
			this.ucFrameInterval.Name = "ucFrameInterval";
			this.ucFrameInterval.Size = new System.Drawing.Size(179, 27);
			this.ucFrameInterval.TabIndex = 23;
			this.ucFrameInterval.Value = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(134, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 13);
			this.label3.TabIndex = 25;
			this.label3.Text = "measurements using ";
			// 
			// nudNumberMeasurements
			// 
			this.nudNumberMeasurements.Location = new System.Drawing.Point(75, 31);
			this.nudNumberMeasurements.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
			this.nudNumberMeasurements.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudNumberMeasurements.Name = "nudNumberMeasurements";
			this.nudNumberMeasurements.Size = new System.Drawing.Size(54, 20);
			this.nudNumberMeasurements.TabIndex = 24;
			this.nudNumberMeasurements.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
			// 
			// btnPrevious
			// 
			this.btnPrevious.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnPrevious.Location = new System.Drawing.Point(297, 300);
			this.btnPrevious.Name = "btnPrevious";
			this.btnPrevious.Size = new System.Drawing.Size(75, 23);
			this.btnPrevious.TabIndex = 18;
			this.btnPrevious.Text = "Cancel";
			this.btnPrevious.UseVisualStyleBackColor = true;
			// 
			// btnNext
			// 
			this.btnNext.Location = new System.Drawing.Point(378, 300);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(75, 23);
			this.btnNext.TabIndex = 17;
			this.btnNext.Text = "Start";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// frmRunMultiFrameSpectroscopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(467, 334);
			this.Controls.Add(this.gbConfig);
			this.Controls.Add(this.btnPrevious);
			this.Controls.Add(this.btnNext);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmRunMultiFrameSpectroscopy";
			this.Text = "Multi-Frame Measurements";
			this.Load += new System.EventHandler(this.frmRunMultiFrameSpectroscopy_Load);
			this.gbConfig.ResumeLayout(false);
			this.gbConfig.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.picAreas)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBackgroundWing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudAreaWing)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbConfig;
		private System.Windows.Forms.Button btnPrevious;
		private System.Windows.Forms.Button btnNext;
		private System.Windows.Forms.Label label5;
		private Astrometry.ucFrameInterval ucFrameInterval;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.NumericUpDown nudNumberMeasurements;
		private System.Windows.Forms.ComboBox cbxBackgroundMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudAreaWing;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown nudBackgroundWing;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox cbxCombineMethod;
		private System.Windows.Forms.PictureBox picAreas;
	}
}