namespace Tangra.Config.SettingPannels
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
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.rbOrder3 = new System.Windows.Forms.RadioButton();
			this.rbOrder2 = new System.Windows.Forms.RadioButton();
			this.rbOrder1 = new System.Windows.Forms.RadioButton();
			this.label48 = new System.Windows.Forms.Label();
			this.cbxInstrumentType = new System.Windows.Forms.ComboBox();
			this.label49 = new System.Windows.Forms.Label();
			this.cbxAllowNegativeValues = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.nudMinWavelength = new System.Windows.Forms.NumericUpDown();
			this.nudMaxWavelength = new System.Windows.Forms.NumericUpDown();
			this.nudResolution = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox8.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinWavelength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxWavelength)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudResolution)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.rbOrder3);
			this.groupBox8.Controls.Add(this.rbOrder2);
			this.groupBox8.Controls.Add(this.rbOrder1);
			this.groupBox8.Controls.Add(this.label48);
			this.groupBox8.Location = new System.Drawing.Point(3, 48);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(231, 136);
			this.groupBox8.TabIndex = 32;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Wavelength Calibration";
			// 
			// rbOrder3
			// 
			this.rbOrder3.AutoSize = true;
			this.rbOrder3.Location = new System.Drawing.Point(32, 106);
			this.rbOrder3.Name = "rbOrder3";
			this.rbOrder3.Size = new System.Drawing.Size(72, 17);
			this.rbOrder3.TabIndex = 35;
			this.rbOrder3.Text = "3-rd Order";
			this.rbOrder3.UseVisualStyleBackColor = true;
			// 
			// rbOrder2
			// 
			this.rbOrder2.AutoSize = true;
			this.rbOrder2.Location = new System.Drawing.Point(32, 83);
			this.rbOrder2.Name = "rbOrder2";
			this.rbOrder2.Size = new System.Drawing.Size(75, 17);
			this.rbOrder2.TabIndex = 34;
			this.rbOrder2.Text = "2-nd Order";
			this.rbOrder2.UseVisualStyleBackColor = true;
			// 
			// rbOrder1
			// 
			this.rbOrder1.AutoSize = true;
			this.rbOrder1.Checked = true;
			this.rbOrder1.Location = new System.Drawing.Point(32, 60);
			this.rbOrder1.Name = "rbOrder1";
			this.rbOrder1.Size = new System.Drawing.Size(54, 17);
			this.rbOrder1.TabIndex = 33;
			this.rbOrder1.TabStop = true;
			this.rbOrder1.Text = "Linear";
			this.rbOrder1.UseVisualStyleBackColor = true;
			// 
			// label48
			// 
			this.label48.AutoSize = true;
			this.label48.Location = new System.Drawing.Point(15, 35);
			this.label48.Name = "label48";
			this.label48.Size = new System.Drawing.Size(176, 13);
			this.label48.TabIndex = 9;
			this.label48.Text = "New Calibrations Default Polynomial";
			// 
			// cbxInstrumentType
			// 
			this.cbxInstrumentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxInstrumentType.FormattingEnabled = true;
			this.cbxInstrumentType.Items.AddRange(new object[] {
            "Grating"});
			this.cbxInstrumentType.Location = new System.Drawing.Point(94, 14);
			this.cbxInstrumentType.Name = "cbxInstrumentType";
			this.cbxInstrumentType.Size = new System.Drawing.Size(101, 21);
			this.cbxInstrumentType.TabIndex = 32;
			// 
			// label49
			// 
			this.label49.AutoSize = true;
			this.label49.Location = new System.Drawing.Point(5, 17);
			this.label49.Name = "label49";
			this.label49.Size = new System.Drawing.Size(83, 13);
			this.label49.TabIndex = 10;
			this.label49.Text = "Instrument Type";
			// 
			// cbxAllowNegativeValues
			// 
			this.cbxAllowNegativeValues.AutoSize = true;
			this.cbxAllowNegativeValues.Location = new System.Drawing.Point(21, 200);
			this.cbxAllowNegativeValues.Name = "cbxAllowNegativeValues";
			this.cbxAllowNegativeValues.Size = new System.Drawing.Size(132, 17);
			this.cbxAllowNegativeValues.TabIndex = 33;
			this.cbxAllowNegativeValues.Text = "Allow Negative Values";
			this.cbxAllowNegativeValues.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.nudResolution);
			this.groupBox1.Controls.Add(this.nudMaxWavelength);
			this.groupBox1.Controls.Add(this.nudMinWavelength);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(240, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(231, 136);
			this.groupBox1.TabIndex = 34;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Absolute Flux Calibration";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(46, 83);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 10;
			this.label1.Text = "Resolution:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(21, 35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 13);
			this.label2.TabIndex = 11;
			this.label2.Text = "Min Wavelength:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Max Wavelength:";
			// 
			// nudMinWavelength
			// 
			this.nudMinWavelength.Location = new System.Drawing.Point(116, 32);
			this.nudMinWavelength.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
			this.nudMinWavelength.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			this.nudMinWavelength.Name = "nudMinWavelength";
			this.nudMinWavelength.Size = new System.Drawing.Size(64, 20);
			this.nudMinWavelength.TabIndex = 13;
			this.nudMinWavelength.Value = new decimal(new int[] {
            3000,
            0,
            0,
            0});
			// 
			// nudMaxWavelength
			// 
			this.nudMaxWavelength.Location = new System.Drawing.Point(116, 57);
			this.nudMaxWavelength.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
			this.nudMaxWavelength.Minimum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			this.nudMaxWavelength.Name = "nudMaxWavelength";
			this.nudMaxWavelength.Size = new System.Drawing.Size(64, 20);
			this.nudMaxWavelength.TabIndex = 14;
			this.nudMaxWavelength.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			// 
			// nudResolution
			// 
			this.nudResolution.Location = new System.Drawing.Point(116, 81);
			this.nudResolution.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
			this.nudResolution.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.nudResolution.Name = "nudResolution";
			this.nudResolution.Size = new System.Drawing.Size(64, 20);
			this.nudResolution.TabIndex = 15;
			this.nudResolution.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(181, 36);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(14, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "A";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(181, 62);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(14, 13);
			this.label5.TabIndex = 18;
			this.label5.Text = "A";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(181, 85);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(14, 13);
			this.label6.TabIndex = 19;
			this.label6.Text = "A";
			// 
			// ucSpectroscopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.cbxAllowNegativeValues);
			this.Controls.Add(this.groupBox8);
			this.Controls.Add(this.label49);
			this.Controls.Add(this.cbxInstrumentType);
			this.Name = "ucSpectroscopy";
			this.Size = new System.Drawing.Size(505, 481);
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudMinWavelength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudMaxWavelength)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudResolution)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.ComboBox cbxInstrumentType;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.Label label48;
		private System.Windows.Forms.RadioButton rbOrder3;
		private System.Windows.Forms.RadioButton rbOrder2;
		private System.Windows.Forms.RadioButton rbOrder1;
        private System.Windows.Forms.CheckBox cbxAllowNegativeValues;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown nudMaxWavelength;
		private System.Windows.Forms.NumericUpDown nudMinWavelength;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nudResolution;
	}
}
