namespace Tangra
{
	partial class frmTargetPSFViewerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTargetPSFViewerForm));
            this.picPixels = new System.Windows.Forms.PictureBox();
            this.picFIT = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblI0 = new System.Windows.Forms.Label();
            this.lblIMax = new System.Windows.Forms.Label();
            this.lblIAmp = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.lblSNR = new System.Windows.Forms.Label();
            this.lblNoise = new System.Windows.Forms.Label();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblSnrCaption = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblFitVariance = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblFWHM = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.nudMeasuringAperture = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlSplitter = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.tbxSmBG = new System.Windows.Forms.TextBox();
            this.tbxBg = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cbMeaMethod = new System.Windows.Forms.ComboBox();
            this.msSnrCalcType = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSnrFromPeakValues = new System.Windows.Forms.ToolStripMenuItem();
            this.miSnrFromAperture = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.picPixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFIT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeasuringAperture)).BeginInit();
            this.msSnrCalcType.SuspendLayout();
            this.SuspendLayout();
            // 
            // picPixels
            // 
            this.picPixels.Location = new System.Drawing.Point(221, 2);
            this.picPixels.Name = "picPixels";
            this.picPixels.Size = new System.Drawing.Size(276, 276);
            this.picPixels.TabIndex = 3;
            this.picPixels.TabStop = false;
            // 
            // picFIT
            // 
            this.picFIT.Location = new System.Drawing.Point(2, 2);
            this.picFIT.Name = "picFIT";
            this.picFIT.Size = new System.Drawing.Size(213, 276);
            this.picFIT.TabIndex = 2;
            this.picFIT.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 289);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Base = ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 289);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Maximum = ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(300, 289);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "X = ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(357, 289);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Y = ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(193, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Amplitude = ";
            // 
            // lblI0
            // 
            this.lblI0.AutoSize = true;
            this.lblI0.Location = new System.Drawing.Point(43, 289);
            this.lblI0.Name = "lblI0";
            this.lblI0.Size = new System.Drawing.Size(22, 13);
            this.lblI0.TabIndex = 9;
            this.lblI0.Text = "0.0";
            // 
            // lblIMax
            // 
            this.lblIMax.AutoSize = true;
            this.lblIMax.Location = new System.Drawing.Point(145, 289);
            this.lblIMax.Name = "lblIMax";
            this.lblIMax.Size = new System.Drawing.Size(22, 13);
            this.lblIMax.TabIndex = 10;
            this.lblIMax.Text = "0.0";
            // 
            // lblIAmp
            // 
            this.lblIAmp.AutoSize = true;
            this.lblIAmp.Location = new System.Drawing.Point(258, 289);
            this.lblIAmp.Name = "lblIAmp";
            this.lblIAmp.Size = new System.Drawing.Size(22, 13);
            this.lblIAmp.TabIndex = 11;
            this.lblIAmp.Text = "0.0";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblX.Location = new System.Drawing.Point(323, 289);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(22, 13);
            this.lblX.TabIndex = 12;
            this.lblX.Text = "0.0";
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblY.Location = new System.Drawing.Point(380, 289);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(22, 13);
            this.lblY.TabIndex = 13;
            this.lblY.Text = "0.0";
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(422, 283);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 19;
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // lblSNR
            // 
            this.lblSNR.AutoSize = true;
            this.lblSNR.Location = new System.Drawing.Point(265, 311);
            this.lblSNR.Name = "lblSNR";
            this.lblSNR.Size = new System.Drawing.Size(22, 13);
            this.lblSNR.TabIndex = 25;
            this.lblSNR.Text = "0.0";
            // 
            // lblNoise
            // 
            this.lblNoise.AutoSize = true;
            this.lblNoise.Location = new System.Drawing.Point(186, 311);
            this.lblNoise.Name = "lblNoise";
            this.lblNoise.Size = new System.Drawing.Size(22, 13);
            this.lblNoise.TabIndex = 24;
            this.lblNoise.Text = "0.0";
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(76, 311);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(22, 13);
            this.lblBackground.TabIndex = 23;
            this.lblBackground.Text = "0.0";
            // 
            // lblSnrCaption
            // 
            this.lblSnrCaption.AutoSize = true;
            this.lblSnrCaption.Location = new System.Drawing.Point(230, 311);
            this.lblSnrCaption.Name = "lblSnrCaption";
            this.lblSnrCaption.Size = new System.Drawing.Size(42, 13);
            this.lblSnrCaption.TabIndex = 22;
            this.lblSnrCaption.Text = "SNR = ";
            this.lblSnrCaption.Click += new System.EventHandler(this.lblSnrCaption_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(145, 311);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Noise = ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 311);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Background = ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label12.Location = new System.Drawing.Point(121, 311);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(19, 13);
            this.label12.TabIndex = 26;
            this.label12.Text = "1s";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.label6.Location = new System.Drawing.Point(382, 311);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "1s";
            // 
            // lblFitVariance
            // 
            this.lblFitVariance.AutoSize = true;
            this.lblFitVariance.Location = new System.Drawing.Point(454, 311);
            this.lblFitVariance.Name = "lblFitVariance";
            this.lblFitVariance.Size = new System.Drawing.Size(22, 13);
            this.lblFitVariance.TabIndex = 28;
            this.lblFitVariance.Text = "0.0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(406, 311);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "PSF Fit = ";
            // 
            // lblFWHM
            // 
            this.lblFWHM.AutoSize = true;
            this.lblFWHM.Location = new System.Drawing.Point(349, 311);
            this.lblFWHM.Name = "lblFWHM";
            this.lblFWHM.Size = new System.Drawing.Size(22, 13);
            this.lblFWHM.TabIndex = 31;
            this.lblFWHM.Text = "0.0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(302, 311);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 13);
            this.label13.TabIndex = 30;
            this.label13.Text = "FWHM = ";
            // 
            // nudMeasuringAperture
            // 
            this.nudMeasuringAperture.DecimalPlaces = 2;
            this.nudMeasuringAperture.Location = new System.Drawing.Point(158, 342);
            this.nudMeasuringAperture.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudMeasuringAperture.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudMeasuringAperture.Name = "nudMeasuringAperture";
            this.nudMeasuringAperture.Size = new System.Drawing.Size(52, 20);
            this.nudMeasuringAperture.TabIndex = 32;
            this.nudMeasuringAperture.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudMeasuringAperture.ValueChanged += new System.EventHandler(this.nudMeasuringAperture_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(136, 346);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "A.";
            // 
            // pnlSplitter
            // 
            this.pnlSplitter.BackColor = System.Drawing.Color.Black;
            this.pnlSplitter.Location = new System.Drawing.Point(6, 333);
            this.pnlSplitter.Name = "pnlSplitter";
            this.pnlSplitter.Size = new System.Drawing.Size(489, 1);
            this.pnlSplitter.TabIndex = 34;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(216, 346);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(48, 13);
            this.label15.TabIndex = 35;
            this.label15.Text = "S - Bg = ";
            // 
            // tbxSmBG
            // 
            this.tbxSmBG.BackColor = System.Drawing.SystemColors.Info;
            this.tbxSmBG.Location = new System.Drawing.Point(261, 342);
            this.tbxSmBG.Name = "tbxSmBG";
            this.tbxSmBG.ReadOnly = true;
            this.tbxSmBG.Size = new System.Drawing.Size(103, 20);
            this.tbxSmBG.TabIndex = 36;
            // 
            // tbxBg
            // 
            this.tbxBg.BackColor = System.Drawing.SystemColors.Info;
            this.tbxBg.Location = new System.Drawing.Point(400, 342);
            this.tbxBg.Name = "tbxBg";
            this.tbxBg.ReadOnly = true;
            this.tbxBg.Size = new System.Drawing.Size(91, 20);
            this.tbxBg.TabIndex = 38;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(368, 346);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 37;
            this.label14.Text = "Bg = ";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 346);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(19, 13);
            this.label16.TabIndex = 39;
            this.label16.Text = "M.";
            // 
            // cbMeaMethod
            // 
            this.cbMeaMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMeaMethod.FormattingEnabled = true;
            this.cbMeaMethod.Items.AddRange(new object[] {
            "Aperture-Median",
            "Aperture-Average",
            "Aperture-Mode",
            "PSF-PSF",
            "PSF-Median",
            "PSF-Average",
            "PSF-Mode"});
            this.cbMeaMethod.Location = new System.Drawing.Point(25, 342);
            this.cbMeaMethod.Name = "cbMeaMethod";
            this.cbMeaMethod.Size = new System.Drawing.Size(105, 21);
            this.cbMeaMethod.TabIndex = 40;
            this.cbMeaMethod.SelectedIndexChanged += new System.EventHandler(this.cbMeaMethod_SelectedIndexChanged);
            // 
            // msSnrCalcType
            // 
            this.msSnrCalcType.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSnrFromPeakValues,
            this.miSnrFromAperture});
            this.msSnrCalcType.Name = "msSnrCalcType";
            this.msSnrCalcType.Size = new System.Drawing.Size(190, 70);
            // 
            // miSnrFromPeakValues
            // 
            this.miSnrFromPeakValues.Name = "miSnrFromPeakValues";
            this.miSnrFromPeakValues.Size = new System.Drawing.Size(189, 22);
            this.miSnrFromPeakValues.Text = "SNR from Peak Values";
            this.miSnrFromPeakValues.Click += new System.EventHandler(this.miSnr_Clicked);
            // 
            // miSnrFromAperture
            // 
            this.miSnrFromAperture.Name = "miSnrFromAperture";
            this.miSnrFromAperture.Size = new System.Drawing.Size(189, 22);
            this.miSnrFromAperture.Text = "SNR from Aperture";
            this.miSnrFromAperture.Click += new System.EventHandler(this.miSnr_Clicked);
            // 
            // frmTargetPSFViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 367);
            this.Controls.Add(this.cbMeaMethod);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.tbxBg);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.tbxSmBG);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.pnlSplitter);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.nudMeasuringAperture);
            this.Controls.Add(this.lblFWHM);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblFitVariance);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.lblSNR);
            this.Controls.Add(this.lblNoise);
            this.Controls.Add(this.lblBackground);
            this.Controls.Add(this.lblSnrCaption);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lblY);
            this.Controls.Add(this.lblX);
            this.Controls.Add(this.lblIAmp);
            this.Controls.Add(this.lblIMax);
            this.Controls.Add(this.lblI0);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picPixels);
            this.Controls.Add(this.picFIT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTargetPSFViewerForm";
            this.Text = "Target PSF";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTargetPSFViewerForm_FormClosed);
            this.VisibleChanged += new System.EventHandler(this.frmTargetPSFViewerForm_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picPixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFIT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMeasuringAperture)).EndInit();
            this.msSnrCalcType.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox picPixels;
		private System.Windows.Forms.PictureBox picFIT;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblI0;
		private System.Windows.Forms.Label lblIMax;
		private System.Windows.Forms.Label lblIAmp;
		private System.Windows.Forms.Label lblX;
		private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Label lblSNR;
		private System.Windows.Forms.Label lblNoise;
		private System.Windows.Forms.Label lblBackground;
		private System.Windows.Forms.Label lblSnrCaption;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblFitVariance;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label lblFWHM;
		private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nudMeasuringAperture;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel pnlSplitter;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tbxSmBG;
        private System.Windows.Forms.TextBox tbxBg;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.ComboBox cbMeaMethod;
        private System.Windows.Forms.ContextMenuStrip msSnrCalcType;
        private System.Windows.Forms.ToolStripMenuItem miSnrFromPeakValues;
        private System.Windows.Forms.ToolStripMenuItem miSnrFromAperture;
	}
}