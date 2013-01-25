using Tangra.Video;
namespace Tangra.VideoOperations.LightCurves.InfoForms
{
	partial class frmZoomedPixels
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmZoomedPixels));
            this.picTarget4Pixels = new System.Windows.Forms.PictureBox();
            this.picTarget3Pixels = new System.Windows.Forms.PictureBox();
            this.picTarget2Pixels = new System.Windows.Forms.PictureBox();
            this.picTarget1Pixels = new System.Windows.Forms.PictureBox();
			this.lblDisplayBandTitle = new System.Windows.Forms.Label();
			this.lblDisplayedBand = new System.Windows.Forms.Label();
			this.warningProvider = new System.Windows.Forms.ErrorProvider();
			this.infoProvider = new System.Windows.Forms.ErrorProvider();
            this.cbxDrawApertures = new System.Windows.Forms.CheckBox();
            this.rbRawData = new System.Windows.Forms.RadioButton();
            this.rbPreProcessedData = new System.Windows.Forms.RadioButton();
            this.tbIntensity = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.picTarget4Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.infoProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbxDrawApertures)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbRawData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPreProcessedData)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbIntensity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbIntensity)).BeginInit();
			this.SuspendLayout();
			// 
			// picTarget4Pixels
			// 
			this.picTarget4Pixels.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget4Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget4Pixels.Location = new System.Drawing.Point(151, 152);
			this.picTarget4Pixels.Name = "picTarget4Pixels";
			this.picTarget4Pixels.Size = new System.Drawing.Size(140, 140);
			this.picTarget4Pixels.TabIndex = 40;
			this.picTarget4Pixels.TabStop = false;
			// 
			// picTarget3Pixels
			// 
			this.picTarget3Pixels.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget3Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget3Pixels.Location = new System.Drawing.Point(5, 152);
			this.picTarget3Pixels.Name = "picTarget3Pixels";
			this.picTarget3Pixels.Size = new System.Drawing.Size(140, 140);
			this.picTarget3Pixels.TabIndex = 39;
			this.picTarget3Pixels.TabStop = false;
			// 
			// picTarget2Pixels
			// 
			this.picTarget2Pixels.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget2Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget2Pixels.Location = new System.Drawing.Point(151, 6);
			this.picTarget2Pixels.Name = "picTarget2Pixels";
			this.picTarget2Pixels.Size = new System.Drawing.Size(140, 140);
			this.picTarget2Pixels.TabIndex = 38;
			this.picTarget2Pixels.TabStop = false;
			// 
			// picTarget1Pixels
			// 
			this.picTarget1Pixels.BackColor = System.Drawing.SystemColors.ControlDark;
			this.picTarget1Pixels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picTarget1Pixels.Location = new System.Drawing.Point(5, 6);
			this.picTarget1Pixels.Name = "picTarget1Pixels";
			this.picTarget1Pixels.Size = new System.Drawing.Size(140, 140);
			this.picTarget1Pixels.TabIndex = 37;
			this.picTarget1Pixels.TabStop = false;
			// 
			// lblDisplayBandTitle
			// 
			this.lblDisplayBandTitle.AutoSize = true;
			this.lblDisplayBandTitle.Location = new System.Drawing.Point(147, 300);
			this.lblDisplayBandTitle.Name = "lblDisplayBandTitle";
			this.lblDisplayBandTitle.Size = new System.Drawing.Size(84, 13);
			this.lblDisplayBandTitle.TabIndex = 43;
			this.lblDisplayBandTitle.Text = "Displayed Band:";
			// 
			// lblDisplayedBand
			// 
			this.lblDisplayedBand.AutoSize = true;
			this.lblDisplayedBand.Location = new System.Drawing.Point(227, 300);
			this.lblDisplayedBand.Name = "lblDisplayedBand";
			this.lblDisplayedBand.Size = new System.Drawing.Size(55, 13);
			this.lblDisplayedBand.TabIndex = 44;
			this.lblDisplayedBand.Text = "GrayScale";
			// 
			// warningProvider
			// 
			this.warningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.warningProvider.ContainerControl = this;
			this.warningProvider.Icon = ((System.Drawing.Icon)(resources.GetObject("warningProvider.Icon")));
			// 
			// infoProvider
			// 
			this.infoProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.infoProvider.ContainerControl = this;
			this.infoProvider.Icon = ((System.Drawing.Icon)(resources.GetObject("infoProvider.Icon")));
			// 
			// cbxDrawApertures
			// 
			this.cbxDrawApertures.Location = new System.Drawing.Point(3, 297);
			this.cbxDrawApertures.Name = "cbxDrawApertures";
			this.cbxDrawApertures.Text = "Tracking Details";
			this.cbxDrawApertures.Size = new System.Drawing.Size(109, 19);
			this.cbxDrawApertures.TabIndex = 49;
			this.cbxDrawApertures.CheckedChanged += new System.EventHandler(this.cbxDrawApertures_CheckedChanged);
			// 
			// rbRawData
			// 
			this.rbRawData.Location = new System.Drawing.Point(131, 316);
			this.rbRawData.Name = "rbRawData";
			this.rbRawData.Text = "Raw Data";
			this.rbRawData.Size = new System.Drawing.Size(88, 19);
			this.rbRawData.TabIndex = 50;
			// 
			// rbPreProcessedData
			// 
			this.rbPreProcessedData.Location = new System.Drawing.Point(3, 316);
			this.rbPreProcessedData.Name = "rbPreProcessedData";
			this.rbPreProcessedData.Text = "Pre-Processed Data";
			this.rbPreProcessedData.Size = new System.Drawing.Size(122, 19);
			this.rbPreProcessedData.TabIndex = 51;
			this.rbPreProcessedData.CheckedChanged += new System.EventHandler(this.rbPreProcessedData_CheckedChanged);
			// 
			// tbIntensity
			// 
			this.tbIntensity.Location = new System.Drawing.Point(284, 1);
			this.tbIntensity.Name = "tbIntensity";
			this.tbIntensity.LargeChange = 10;
			this.tbIntensity.Maximum = 100;
			this.tbIntensity.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.tbIntensity.SmallChange = 5;
			this.tbIntensity.TickFrequency = 5;
			this.tbIntensity.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.tbIntensity.Size = new System.Drawing.Size(42, 299);
			this.tbIntensity.TabIndex = 52;
			this.tbIntensity.ValueChanged += new System.EventHandler(this.tbIntensity_ValueChanged);
			// 
			// frmZoomedPixels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(328, 337);
			this.Controls.Add(this.rbPreProcessedData);
			this.Controls.Add(this.rbRawData);
			this.Controls.Add(this.cbxDrawApertures);
			this.Controls.Add(this.lblDisplayedBand);
			this.Controls.Add(this.lblDisplayBandTitle);
			this.Controls.Add(this.picTarget4Pixels);
			this.Controls.Add(this.picTarget3Pixels);
			this.Controls.Add(this.picTarget2Pixels);
			this.Controls.Add(this.picTarget1Pixels);
			this.Controls.Add(this.tbIntensity);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmZoomedPixels";
			this.Text = "Measured Pixel Areas";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmZoomedPixels_FormClosing);
			this.Load += new System.EventHandler(this.frmZoomedPixels_Load);
			((System.ComponentModel.ISupportInitialize)(this.picTarget4Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget3Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget2Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.picTarget1Pixels)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.infoProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbxDrawApertures)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbRawData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rbPreProcessedData)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbIntensity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbIntensity)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.PictureBox picTarget4Pixels;
        private System.Windows.Forms.PictureBox picTarget3Pixels;
        private System.Windows.Forms.PictureBox picTarget2Pixels;
        private System.Windows.Forms.PictureBox picTarget1Pixels;
		private System.Windows.Forms.Label lblDisplayBandTitle;
		private System.Windows.Forms.Label lblDisplayedBand;
        private System.Windows.Forms.ErrorProvider warningProvider;
		private System.Windows.Forms.ErrorProvider infoProvider;
        private System.Windows.Forms.RadioButton rbPreProcessedData;
        private System.Windows.Forms.RadioButton rbRawData;
        private System.Windows.Forms.CheckBox cbxDrawApertures;
        private System.Windows.Forms.TrackBar tbIntensity;
	}
}