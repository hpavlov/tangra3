namespace Tangra.VideoTools
{
	partial class frmDefineDisplayDynamicRange
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
			this.components = new System.ComponentModel.Container();			
			this.picHistogram = new System.Windows.Forms.PictureBox();
			this.rbScaleLinear = new System.Windows.Forms.RadioButton();
			this.rbScaleLog = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.tbarTo = new System.Windows.Forms.TrackBar();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.pnlSelectedRange = new System.Windows.Forms.Panel();
			this.pnlBackground = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.btnClose = new System.Windows.Forms.Button();
			this.tbarFrom = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbarTo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbarFrom)).BeginInit();
			this.SuspendLayout();
			// 
			// picHistogram
			// 
			this.picHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picHistogram.Location = new System.Drawing.Point(12, 12);
			this.picHistogram.Name = "picHistogram";
			this.picHistogram.Size = new System.Drawing.Size(504, 182);
			this.picHistogram.TabIndex = 2;
			this.picHistogram.TabStop = false;
			// 
			// rbScaleLinear
			// 
			this.rbScaleLinear.AutoSize = true;
			this.rbScaleLinear.Location = new System.Drawing.Point(129, 290);
			this.rbScaleLinear.Name = "rbScaleLinear";
			this.rbScaleLinear.Size = new System.Drawing.Size(54, 17);
			this.rbScaleLinear.TabIndex = 8;
			this.rbScaleLinear.Text = "Linear";
			this.rbScaleLinear.UseVisualStyleBackColor = true;
			this.rbScaleLinear.CheckedChanged += new System.EventHandler(this.rbScaleLinear_CheckedChanged);
			// 
			// rbScaleLog
			// 
			this.rbScaleLog.AutoSize = true;
			this.rbScaleLog.Checked = true;
			this.rbScaleLog.Location = new System.Drawing.Point(50, 290);
			this.rbScaleLog.Name = "rbScaleLog";
			this.rbScaleLog.Size = new System.Drawing.Size(79, 17);
			this.rbScaleLog.TabIndex = 7;
			this.rbScaleLog.TabStop = true;
			this.rbScaleLog.Text = "Logarithmic";
			this.rbScaleLog.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 292);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Scale:";
			// 
			// tbarTo
			// 
			this.tbarTo.Location = new System.Drawing.Point(1, 253);
			this.tbarTo.Name = "tbarTo";
			this.tbarTo.Size = new System.Drawing.Size(525, 45);
			this.tbarTo.TabIndex = 9;
			this.tbarTo.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.tbarTo.ValueChanged += new System.EventHandler(this.DynamicRangeChanged);
			// 
			// timer1
			// 
			this.timer1.Interval = 150;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// pnlSelectedRange
			// 
			this.pnlSelectedRange.BackColor = System.Drawing.Color.Lime;
			this.pnlSelectedRange.Location = new System.Drawing.Point(12, 246);
			this.pnlSelectedRange.Name = "pnlSelectedRange";
			this.pnlSelectedRange.Size = new System.Drawing.Size(504, 6);
			this.pnlSelectedRange.TabIndex = 12;
			// 
			// pnlBackground
			// 
			this.pnlBackground.BackColor = System.Drawing.SystemColors.ControlDark;
			this.pnlBackground.Location = new System.Drawing.Point(15, 246);
			this.pnlBackground.Name = "pnlBackground";
			this.pnlBackground.Size = new System.Drawing.Size(501, 6);
			this.pnlBackground.TabIndex = 14;
			// 
			// tbarFrom
			// 
			this.tbarFrom.LargeChange = 100;
			this.tbarFrom.Location = new System.Drawing.Point(1, 214);
			this.tbarFrom.Maximum = 256;
			this.tbarFrom.Name = "tbarFrom";
			this.tbarFrom.Size = new System.Drawing.Size(525, 45);
			this.tbarFrom.SmallChange = 100;
			this.tbarFrom.TabIndex = 11;
			this.tbarFrom.ValueChanged += new System.EventHandler(this.DynamicRangeChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 198);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(457, 13);
			this.label1.TabIndex = 15;
			this.label1.Text = "NOTE: This is a display range only. The measurements are still performed on the o" +
    "riginal dataset";
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Location = new System.Drawing.Point(441, 290);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 16;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// frmDefineDisplayDynamicRange
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(528, 321);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.rbScaleLinear);
			this.Controls.Add(this.rbScaleLog);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pnlSelectedRange);
			this.Controls.Add(this.pnlBackground);
			this.Controls.Add(this.tbarFrom);
			this.Controls.Add(this.tbarTo);
			this.Controls.Add(this.picHistogram);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmDefineDisplayDynamicRange";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Define Display Dynamic Range";
			((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbarTo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbarFrom)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox picHistogram;
		private System.Windows.Forms.RadioButton rbScaleLinear;
		private System.Windows.Forms.RadioButton rbScaleLog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar tbarFrom;
		private System.Windows.Forms.TrackBar tbarTo;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Panel pnlSelectedRange;
		private System.Windows.Forms.Panel pnlBackground;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnClose;
	}
}