namespace Tangra.Config.SettingPannels
{
	partial class ucPreProcessingSettings
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
			this.cbxDarkFrameAdjustToMedian = new System.Windows.Forms.CheckBox();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupControl1
			// 
			this.groupControl1.Controls.Add(this.cbxDarkFrameAdjustToMedian);
			this.groupControl1.Location = new System.Drawing.Point(3, 3);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(268, 187);
			this.groupControl1.TabIndex = 51;
			this.groupControl1.TabStop = false;
			this.groupControl1.Text = "Dark Frame Correction";
			// 
			// cbxDarkFrameAdjustToMedian
			// 
			this.cbxDarkFrameAdjustToMedian.Location = new System.Drawing.Point(18, 33);
			this.cbxDarkFrameAdjustToMedian.Name = "cbxDarkFrameAdjustToMedian";
			this.cbxDarkFrameAdjustToMedian.Size = new System.Drawing.Size(212, 19);
			this.cbxDarkFrameAdjustToMedian.TabIndex = 51;
			this.cbxDarkFrameAdjustToMedian.Text = "Adjust Levels to Dark Frame Median";
			// 
			// ucPreProcessingSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupControl1);
			this.Name = "ucPreProcessingSettings";
			this.Size = new System.Drawing.Size(612, 558);
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupControl1;
		private System.Windows.Forms.CheckBox cbxDarkFrameAdjustToMedian;
	}
}
