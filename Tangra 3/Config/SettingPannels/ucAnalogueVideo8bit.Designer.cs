namespace Tangra.Config.SettingPannels
{
	partial class ucAnalogueVideo8bit
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
			this.label20 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupControl1 = new System.Windows.Forms.GroupBox();
			this.cbxEnableOsdOcr = new System.Windows.Forms.CheckBox();
			this.cbxColourChannel = new System.Windows.Forms.ComboBox();
			this.cbxRenderingEngineAttemptOrder = new System.Windows.Forms.ComboBox();
			this.nudSaturation8bit = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.cbxOcrEngine = new System.Windows.Forms.ComboBox();
			this.pnlOsdOcr = new System.Windows.Forms.Panel();
			this.cbxAskMeEveryTime = new System.Windows.Forms.CheckBox();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSaturation8bit)).BeginInit();
			this.pnlOsdOcr.SuspendLayout();
			this.SuspendLayout();
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Location = new System.Drawing.Point(13, 78);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(56, 13);
			this.label20.TabIndex = 34;
			this.label20.Text = "Use band:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(135, 13);
			this.label2.TabIndex = 36;
			this.label2.Text = "Preferred rendering engine:";
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.pnlOsdOcr);
			this.groupControl1.Controls.Add(this.cbxEnableOsdOcr);
			this.groupControl1.Controls.Add(this.cbxColourChannel);
			this.groupControl1.Controls.Add(this.cbxRenderingEngineAttemptOrder);
			this.groupControl1.Controls.Add(this.nudSaturation8bit);
			this.groupControl1.Controls.Add(this.label2);
			this.groupControl1.Controls.Add(this.label20);
			this.groupControl1.Controls.Add(this.label1);
			this.groupControl1.Location = new System.Drawing.Point(3, 3);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(277, 295);
			this.groupControl1.TabIndex = 42;
			this.groupControl1.TabStop = false;
			this.groupControl1.Text = "Analogue Video (8 bit)";
			// 
			// cbxEnableOsdOcr
			// 
			this.cbxEnableOsdOcr.AutoSize = true;
			this.cbxEnableOsdOcr.Location = new System.Drawing.Point(16, 187);
			this.cbxEnableOsdOcr.Name = "cbxEnableOsdOcr";
			this.cbxEnableOsdOcr.Size = new System.Drawing.Size(192, 17);
			this.cbxEnableOsdOcr.TabIndex = 43;
			this.cbxEnableOsdOcr.Text = "Read OSD timestamp automatically";
			this.cbxEnableOsdOcr.UseVisualStyleBackColor = true;
			this.cbxEnableOsdOcr.CheckedChanged += new System.EventHandler(this.cbxEnableOsdOcr_CheckedChanged);
			// 
			// cbxColourChannel
			// 
			this.cbxColourChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxColourChannel.Items.AddRange(new object[] {
            "Red",
            "Green",
            "Blue",
            "Gray Scale"});
			this.cbxColourChannel.Location = new System.Drawing.Point(16, 94);
			this.cbxColourChannel.Name = "cbxColourChannel";
			this.cbxColourChannel.Size = new System.Drawing.Size(190, 21);
			this.cbxColourChannel.TabIndex = 45;
			// 
			// cbxRenderingEngineAttemptOrder
			// 
			this.cbxRenderingEngineAttemptOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxRenderingEngineAttemptOrder.Location = new System.Drawing.Point(16, 43);
			this.cbxRenderingEngineAttemptOrder.Name = "cbxRenderingEngineAttemptOrder";
			this.cbxRenderingEngineAttemptOrder.Size = new System.Drawing.Size(190, 21);
			this.cbxRenderingEngineAttemptOrder.TabIndex = 44;
			// 
			// nudSaturation8bit
			// 
			this.nudSaturation8bit.Location = new System.Drawing.Point(120, 142);
			this.nudSaturation8bit.Maximum = new decimal(new int[] {
            256,
            0,
            0,
            0});
			this.nudSaturation8bit.Name = "nudSaturation8bit";
			this.nudSaturation8bit.Size = new System.Drawing.Size(69, 20);
			this.nudSaturation8bit.TabIndex = 42;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 145);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 43;
			this.label1.Text = "Saturation Level:";
			// 
			// cbxOcrEngine
			// 
			this.cbxOcrEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxOcrEngine.Items.AddRange(new object[] {
            "IOTA-VTI Non TV-Safe",
            "IOTA-VTI TV-Safe"});
			this.cbxOcrEngine.Location = new System.Drawing.Point(27, 16);
			this.cbxOcrEngine.Name = "cbxOcrEngine";
			this.cbxOcrEngine.Size = new System.Drawing.Size(190, 21);
			this.cbxOcrEngine.TabIndex = 51;
			// 
			// pnlOsdOcr
			// 
			this.pnlOsdOcr.Controls.Add(this.cbxAskMeEveryTime);
			this.pnlOsdOcr.Controls.Add(this.cbxOcrEngine);
			this.pnlOsdOcr.Location = new System.Drawing.Point(6, 210);
			this.pnlOsdOcr.Name = "pnlOsdOcr";
			this.pnlOsdOcr.Size = new System.Drawing.Size(229, 79);
			this.pnlOsdOcr.TabIndex = 52;
			// 
			// cbxAskMeEveryTime
			// 
			this.cbxAskMeEveryTime.AutoSize = true;
			this.cbxAskMeEveryTime.Location = new System.Drawing.Point(27, 48);
			this.cbxAskMeEveryTime.Name = "cbxAskMeEveryTime";
			this.cbxAskMeEveryTime.Size = new System.Drawing.Size(167, 17);
			this.cbxAskMeEveryTime.TabIndex = 52;
			this.cbxAskMeEveryTime.Text = "Let me choose for each video";
			this.cbxAskMeEveryTime.UseVisualStyleBackColor = true;
			// 
			// ucAnalogueVideo8bit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.groupControl1);
			this.Name = "ucAnalogueVideo8bit";
			this.Size = new System.Drawing.Size(471, 349);
			this.groupControl1.ResumeLayout(false);
			this.groupControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudSaturation8bit)).EndInit();
			this.pnlOsdOcr.ResumeLayout(false);
			this.pnlOsdOcr.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.ComboBox cbxRenderingEngineAttemptOrder;
		private System.Windows.Forms.NumericUpDown nudSaturation8bit;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbxColourChannel;
		private System.Windows.Forms.CheckBox cbxEnableOsdOcr;
		private System.Windows.Forms.Panel pnlOsdOcr;
		private System.Windows.Forms.CheckBox cbxAskMeEveryTime;
		private System.Windows.Forms.ComboBox cbxOcrEngine;
	}
}
