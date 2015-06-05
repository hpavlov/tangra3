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
			this.label5 = new System.Windows.Forms.Label();
			this.ucFrameInterval = new Tangra.VideoOperations.Astrometry.ucFrameInterval();
			this.label3 = new System.Windows.Forms.Label();
			this.nudNumberMeasurements = new System.Windows.Forms.NumericUpDown();
			this.btnPrevious = new System.Windows.Forms.Button();
			this.btnNext = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.gbConfig.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).BeginInit();
			this.SuspendLayout();
			// 
			// gbConfig
			// 
			this.gbConfig.Controls.Add(this.comboBox1);
			this.gbConfig.Controls.Add(this.label5);
			this.gbConfig.Controls.Add(this.ucFrameInterval);
			this.gbConfig.Controls.Add(this.label3);
			this.gbConfig.Controls.Add(this.nudNumberMeasurements);
			this.gbConfig.Location = new System.Drawing.Point(12, 12);
			this.gbConfig.Name = "gbConfig";
			this.gbConfig.Size = new System.Drawing.Size(441, 282);
			this.gbConfig.TabIndex = 19;
			this.gbConfig.TabStop = false;
			this.gbConfig.Text = "Spectroscopy";
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
			this.ucFrameInterval.Location = new System.Drawing.Point(18, 64);
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
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 25;
			this.label3.Text = "measurements";
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
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "Average",
            "Median"});
			this.comboBox1.Location = new System.Drawing.Point(75, 110);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(84, 21);
			this.comboBox1.TabIndex = 27;
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
			this.gbConfig.ResumeLayout(false);
			this.gbConfig.PerformLayout();
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
		private System.Windows.Forms.ComboBox comboBox1;
	}
}