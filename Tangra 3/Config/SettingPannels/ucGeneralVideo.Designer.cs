namespace Tangra.Config.SettingPannels
{
	partial class ucGeneralVideo
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
			this.groupControl1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.pnlChooseKnownResponse = new System.Windows.Forms.Panel();
			this.cbxKnownResponse = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cbxCameraResponseFullFrame = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.pnlEnterGammaValue = new System.Windows.Forms.Panel();
			this.label28 = new System.Windows.Forms.Label();
			this.nudGamma = new System.Windows.Forms.NumericUpDown();
			this.cbxGammaTheFullFrame = new System.Windows.Forms.CheckBox();
			this.groupControl1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.pnlChooseKnownResponse.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.pnlEnterGammaValue.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudGamma)).BeginInit();
			this.SuspendLayout();
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.groupBox2);
			this.groupControl1.Controls.Add(this.groupBox1);
			this.groupControl1.Location = new System.Drawing.Point(3, 3);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(268, 210);
			this.groupControl1.TabIndex = 50;
			this.groupControl1.TabStop = false;
			this.groupControl1.Text = "General Video Settings";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.pnlChooseKnownResponse);
			this.groupBox2.Controls.Add(this.cbxCameraResponseFullFrame);
			this.groupBox2.Location = new System.Drawing.Point(11, 115);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(244, 83);
			this.groupBox2.TabIndex = 56;
			this.groupBox2.TabStop = false;
			// 
			// pnlChooseKnownResponse
			// 
			this.pnlChooseKnownResponse.Controls.Add(this.cbxKnownResponse);
			this.pnlChooseKnownResponse.Controls.Add(this.label1);
			this.pnlChooseKnownResponse.Location = new System.Drawing.Point(6, 32);
			this.pnlChooseKnownResponse.Name = "pnlChooseKnownResponse";
			this.pnlChooseKnownResponse.Size = new System.Drawing.Size(232, 31);
			this.pnlChooseKnownResponse.TabIndex = 53;
			// 
			// cbxKnownResponse
			// 
			this.cbxKnownResponse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxKnownResponse.FormattingEnabled = true;
			this.cbxKnownResponse.Items.AddRange(new object[] {
            "",
            "WAT-910HX/BD"});
			this.cbxKnownResponse.Location = new System.Drawing.Point(109, 5);
			this.cbxKnownResponse.Name = "cbxKnownResponse";
			this.cbxKnownResponse.Size = new System.Drawing.Size(108, 21);
			this.cbxKnownResponse.TabIndex = 57;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 13);
			this.label1.TabIndex = 56;
			this.label1.Text = "Known Response:";
			// 
			// cbxCameraResponseFullFrame
			// 
			this.cbxCameraResponseFullFrame.Location = new System.Drawing.Point(12, -1);
			this.cbxCameraResponseFullFrame.Name = "cbxCameraResponseFullFrame";
			this.cbxCameraResponseFullFrame.Size = new System.Drawing.Size(211, 19);
			this.cbxCameraResponseFullFrame.TabIndex = 51;
			this.cbxCameraResponseFullFrame.Text = "Reverse Non-Linear Camera Response";
			this.cbxCameraResponseFullFrame.CheckedChanged += new System.EventHandler(this.cbxCameraResponseFullFrame_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.pnlEnterGammaValue);
			this.groupBox1.Controls.Add(this.cbxGammaTheFullFrame);
			this.groupBox1.Location = new System.Drawing.Point(11, 25);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(244, 83);
			this.groupBox1.TabIndex = 53;
			this.groupBox1.TabStop = false;
			// 
			// pnlEnterGammaValue
			// 
			this.pnlEnterGammaValue.Controls.Add(this.label28);
			this.pnlEnterGammaValue.Controls.Add(this.nudGamma);
			this.pnlEnterGammaValue.Location = new System.Drawing.Point(16, 32);
			this.pnlEnterGammaValue.Name = "pnlEnterGammaValue";
			this.pnlEnterGammaValue.Size = new System.Drawing.Size(200, 31);
			this.pnlEnterGammaValue.TabIndex = 53;
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Location = new System.Drawing.Point(13, 7);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(94, 13);
			this.label28.TabIndex = 50;
			this.label28.Text = "Encoding Gamma:";
			// 
			// nudGamma
			// 
			this.nudGamma.DecimalPlaces = 4;
			this.nudGamma.Location = new System.Drawing.Point(113, 5);
			this.nudGamma.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.nudGamma.Name = "nudGamma";
			this.nudGamma.Size = new System.Drawing.Size(61, 20);
			this.nudGamma.TabIndex = 52;
			this.nudGamma.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// cbxGammaTheFullFrame
			// 
			this.cbxGammaTheFullFrame.Location = new System.Drawing.Point(12, -1);
			this.cbxGammaTheFullFrame.Name = "cbxGammaTheFullFrame";
			this.cbxGammaTheFullFrame.Size = new System.Drawing.Size(192, 19);
			this.cbxGammaTheFullFrame.TabIndex = 51;
			this.cbxGammaTheFullFrame.Text = "Reverse Video Gamma Correction";
			this.cbxGammaTheFullFrame.CheckedChanged += new System.EventHandler(this.cbxGammaTheFullFrame_CheckedChanged);
			// 
			// ucGeneralVideo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupControl1);
			this.Name = "ucGeneralVideo";
			this.Size = new System.Drawing.Size(578, 500);
			this.groupControl1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.pnlChooseKnownResponse.ResumeLayout(false);
			this.pnlChooseKnownResponse.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.pnlEnterGammaValue.ResumeLayout(false);
			this.pnlEnterGammaValue.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudGamma)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.NumericUpDown nudGamma;
		private System.Windows.Forms.CheckBox cbxGammaTheFullFrame;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel pnlEnterGammaValue;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Panel pnlChooseKnownResponse;
		private System.Windows.Forms.ComboBox cbxKnownResponse;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbxCameraResponseFullFrame;

	}
}
