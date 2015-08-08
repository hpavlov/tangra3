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
            this.gbMeasurement = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.nudBackgroundGap = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
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
            this.picAreas = new System.Windows.Forms.PictureBox();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.gbxAlignment = new System.Windows.Forms.GroupBox();
            this.gbAlignOn = new System.Windows.Forms.GroupBox();
            this.rbAlignZeroOrder = new System.Windows.Forms.RadioButton();
            this.rbAlignEmissionLine = new System.Windows.Forms.RadioButton();
            this.rbAlignAbsorptionLine = new System.Windows.Forms.RadioButton();
            this.picAlignTarget = new System.Windows.Forms.PictureBox();
            this.cbxFineAdjustments = new System.Windows.Forms.CheckBox();
            this.gbNormalisation = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.nudExposureSec = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudMultiplier = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.nudDivisor = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.cbxNormalisation = new System.Windows.Forms.CheckBox();
            this.gbMeasurement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackgroundGap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackgroundWing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAreaWing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAreas)).BeginInit();
            this.gbxAlignment.SuspendLayout();
            this.gbAlignOn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlignTarget)).BeginInit();
            this.gbNormalisation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureSec)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDivisor)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMeasurement
            // 
            this.gbMeasurement.Controls.Add(this.label8);
            this.gbMeasurement.Controls.Add(this.nudBackgroundGap);
            this.gbMeasurement.Controls.Add(this.label9);
            this.gbMeasurement.Controls.Add(this.cbxCombineMethod);
            this.gbMeasurement.Controls.Add(this.label6);
            this.gbMeasurement.Controls.Add(this.nudBackgroundWing);
            this.gbMeasurement.Controls.Add(this.label7);
            this.gbMeasurement.Controls.Add(this.label4);
            this.gbMeasurement.Controls.Add(this.nudAreaWing);
            this.gbMeasurement.Controls.Add(this.label2);
            this.gbMeasurement.Controls.Add(this.label1);
            this.gbMeasurement.Controls.Add(this.cbxBackgroundMethod);
            this.gbMeasurement.Controls.Add(this.label5);
            this.gbMeasurement.Controls.Add(this.ucFrameInterval);
            this.gbMeasurement.Controls.Add(this.label3);
            this.gbMeasurement.Controls.Add(this.nudNumberMeasurements);
            this.gbMeasurement.Location = new System.Drawing.Point(542, 12);
            this.gbMeasurement.Name = "gbMeasurement";
            this.gbMeasurement.Size = new System.Drawing.Size(517, 235);
            this.gbMeasurement.TabIndex = 19;
            this.gbMeasurement.TabStop = false;
            this.gbMeasurement.Text = "Measurements";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(203, 153);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(33, 13);
            this.label8.TabIndex = 41;
            this.label8.Text = "pixels";
            // 
            // nudBackgroundGap
            // 
            this.nudBackgroundGap.Location = new System.Drawing.Point(155, 151);
            this.nudBackgroundGap.Maximum = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudBackgroundGap.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBackgroundGap.Name = "nudBackgroundGap";
            this.nudBackgroundGap.Size = new System.Drawing.Size(42, 20);
            this.nudBackgroundGap.TabIndex = 40;
            this.nudBackgroundGap.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudBackgroundGap.ValueChanged += new System.EventHandler(this.nudBackgroundGap_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(29, 151);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(113, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Background area gap:";
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
            this.label6.Location = new System.Drawing.Point(203, 176);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "pixels";
            // 
            // nudBackgroundWing
            // 
            this.nudBackgroundWing.Location = new System.Drawing.Point(155, 174);
            this.nudBackgroundWing.Maximum = new decimal(new int[] {
            25,
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
            this.label7.Location = new System.Drawing.Point(29, 174);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Background area wing:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(203, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "pixels";
            // 
            // nudAreaWing
            // 
            this.nudAreaWing.Location = new System.Drawing.Point(155, 128);
            this.nudAreaWing.Maximum = new decimal(new int[] {
            25,
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
            this.label2.Location = new System.Drawing.Point(23, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Measurement area wing:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 207);
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
            this.cbxBackgroundMethod.Location = new System.Drawing.Point(155, 204);
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
            // picAreas
            // 
            this.picAreas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAreas.Location = new System.Drawing.Point(12, 253);
            this.picAreas.Name = "picAreas";
            this.picAreas.Size = new System.Drawing.Size(517, 74);
            this.picAreas.TabIndex = 36;
            this.picAreas.TabStop = false;
            this.picAreas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picAreas_MouseDown);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(373, 340);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 18;
            this.btnPrevious.Text = "Cancel";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(454, 340);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 17;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // gbxAlignment
            // 
            this.gbxAlignment.Controls.Add(this.gbAlignOn);
            this.gbxAlignment.Controls.Add(this.picAlignTarget);
            this.gbxAlignment.Controls.Add(this.cbxFineAdjustments);
            this.gbxAlignment.Location = new System.Drawing.Point(12, 12);
            this.gbxAlignment.Name = "gbxAlignment";
            this.gbxAlignment.Size = new System.Drawing.Size(517, 235);
            this.gbxAlignment.TabIndex = 20;
            this.gbxAlignment.TabStop = false;
            this.gbxAlignment.Text = "Alignment and Tracking";
            // 
            // gbAlignOn
            // 
            this.gbAlignOn.Controls.Add(this.rbAlignZeroOrder);
            this.gbAlignOn.Controls.Add(this.rbAlignEmissionLine);
            this.gbAlignOn.Controls.Add(this.rbAlignAbsorptionLine);
            this.gbAlignOn.Location = new System.Drawing.Point(16, 37);
            this.gbAlignOn.Name = "gbAlignOn";
            this.gbAlignOn.Size = new System.Drawing.Size(195, 100);
            this.gbAlignOn.TabIndex = 42;
            this.gbAlignOn.TabStop = false;
            this.gbAlignOn.Text = "Align on";
            // 
            // rbAlignZeroOrder
            // 
            this.rbAlignZeroOrder.AutoSize = true;
            this.rbAlignZeroOrder.Checked = true;
            this.rbAlignZeroOrder.Location = new System.Drawing.Point(22, 26);
            this.rbAlignZeroOrder.Name = "rbAlignZeroOrder";
            this.rbAlignZeroOrder.Size = new System.Drawing.Size(108, 17);
            this.rbAlignZeroOrder.TabIndex = 38;
            this.rbAlignZeroOrder.TabStop = true;
            this.rbAlignZeroOrder.Text = "Zero-Order Image";
            this.rbAlignZeroOrder.UseVisualStyleBackColor = true;
            // 
            // rbAlignEmissionLine
            // 
            this.rbAlignEmissionLine.AutoSize = true;
            this.rbAlignEmissionLine.Location = new System.Drawing.Point(22, 65);
            this.rbAlignEmissionLine.Name = "rbAlignEmissionLine";
            this.rbAlignEmissionLine.Size = new System.Drawing.Size(134, 17);
            this.rbAlignEmissionLine.TabIndex = 39;
            this.rbAlignEmissionLine.Text = "Selected Emission Line";
            this.rbAlignEmissionLine.UseVisualStyleBackColor = true;
            // 
            // rbAlignAbsorptionLine
            // 
            this.rbAlignAbsorptionLine.AutoSize = true;
            this.rbAlignAbsorptionLine.Location = new System.Drawing.Point(22, 46);
            this.rbAlignAbsorptionLine.Name = "rbAlignAbsorptionLine";
            this.rbAlignAbsorptionLine.Size = new System.Drawing.Size(143, 17);
            this.rbAlignAbsorptionLine.TabIndex = 40;
            this.rbAlignAbsorptionLine.Text = "Selected Absorption Line";
            this.rbAlignAbsorptionLine.UseVisualStyleBackColor = true;
            // 
            // picAlignTarget
            // 
            this.picAlignTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picAlignTarget.Location = new System.Drawing.Point(296, 15);
            this.picAlignTarget.Name = "picAlignTarget";
            this.picAlignTarget.Size = new System.Drawing.Size(210, 210);
            this.picAlignTarget.TabIndex = 41;
            this.picAlignTarget.TabStop = false;
            // 
            // cbxFineAdjustments
            // 
            this.cbxFineAdjustments.AutoSize = true;
            this.cbxFineAdjustments.Checked = true;
            this.cbxFineAdjustments.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxFineAdjustments.Location = new System.Drawing.Point(18, 173);
            this.cbxFineAdjustments.Name = "cbxFineAdjustments";
            this.cbxFineAdjustments.Size = new System.Drawing.Size(128, 17);
            this.cbxFineAdjustments.TabIndex = 37;
            this.cbxFineAdjustments.Text = "Use Fine Adjustments";
            this.cbxFineAdjustments.UseVisualStyleBackColor = true;
            // 
            // gbNormalisation
            // 
            this.gbNormalisation.Controls.Add(this.label13);
            this.gbNormalisation.Controls.Add(this.nudExposureSec);
            this.gbNormalisation.Controls.Add(this.label12);
            this.gbNormalisation.Controls.Add(this.groupBox1);
            this.gbNormalisation.Location = new System.Drawing.Point(1065, 12);
            this.gbNormalisation.Name = "gbNormalisation";
            this.gbNormalisation.Size = new System.Drawing.Size(517, 235);
            this.gbNormalisation.TabIndex = 37;
            this.gbNormalisation.TabStop = false;
            this.gbNormalisation.Text = "Normalisation";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(149, 134);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 13);
            this.label13.TabIndex = 46;
            this.label13.Text = "sec";
            // 
            // nudExposureSec
            // 
            this.nudExposureSec.DecimalPlaces = 3;
            this.nudExposureSec.Location = new System.Drawing.Point(83, 130);
            this.nudExposureSec.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudExposureSec.Name = "nudExposureSec";
            this.nudExposureSec.Size = new System.Drawing.Size(60, 20);
            this.nudExposureSec.TabIndex = 45;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 133);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 44;
            this.label12.Text = "Exposure:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudMultiplier);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.nudDivisor);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.cbxNormalisation);
            this.groupBox1.Location = new System.Drawing.Point(18, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 90);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "       ";
            // 
            // nudMultiplier
            // 
            this.nudMultiplier.Location = new System.Drawing.Point(79, 51);
            this.nudMultiplier.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudMultiplier.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMultiplier.Name = "nudMultiplier";
            this.nudMultiplier.Size = new System.Drawing.Size(72, 20);
            this.nudMultiplier.TabIndex = 42;
            this.nudMultiplier.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(22, 55);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 41;
            this.label11.Text = "Multiplier:";
            // 
            // nudDivisor
            // 
            this.nudDivisor.Location = new System.Drawing.Point(79, 25);
            this.nudDivisor.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudDivisor.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudDivisor.Name = "nudDivisor";
            this.nudDivisor.Size = new System.Drawing.Size(72, 20);
            this.nudDivisor.TabIndex = 40;
            this.nudDivisor.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 27);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(42, 13);
            this.label10.TabIndex = 39;
            this.label10.Text = "Divisor:";
            // 
            // cbxNormalisation
            // 
            this.cbxNormalisation.AutoSize = true;
            this.cbxNormalisation.Checked = true;
            this.cbxNormalisation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxNormalisation.Location = new System.Drawing.Point(15, 0);
            this.cbxNormalisation.Name = "cbxNormalisation";
            this.cbxNormalisation.Size = new System.Drawing.Size(133, 17);
            this.cbxNormalisation.TabIndex = 38;
            this.cbxNormalisation.Text = "Use Flux Normalisation";
            this.cbxNormalisation.UseVisualStyleBackColor = true;
            this.cbxNormalisation.CheckedChanged += new System.EventHandler(this.cbxNormalisation_CheckedChanged);
            // 
            // frmRunMultiFrameSpectroscopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1589, 374);
            this.Controls.Add(this.gbNormalisation);
            this.Controls.Add(this.gbxAlignment);
            this.Controls.Add(this.picAreas);
            this.Controls.Add(this.gbMeasurement);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRunMultiFrameSpectroscopy";
            this.Text = "Multi-Frame Spectroscopy";
            this.Load += new System.EventHandler(this.frmRunMultiFrameSpectroscopy_Load);
            this.gbMeasurement.ResumeLayout(false);
            this.gbMeasurement.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackgroundGap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackgroundWing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAreaWing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAreas)).EndInit();
            this.gbxAlignment.ResumeLayout(false);
            this.gbxAlignment.PerformLayout();
            this.gbAlignOn.ResumeLayout(false);
            this.gbAlignOn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlignTarget)).EndInit();
            this.gbNormalisation.ResumeLayout(false);
            this.gbNormalisation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudExposureSec)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDivisor)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbMeasurement;
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
        private System.Windows.Forms.GroupBox gbxAlignment;
        private System.Windows.Forms.RadioButton rbAlignAbsorptionLine;
        private System.Windows.Forms.RadioButton rbAlignEmissionLine;
        private System.Windows.Forms.RadioButton rbAlignZeroOrder;
        private System.Windows.Forms.CheckBox cbxFineAdjustments;
        private System.Windows.Forms.GroupBox gbAlignOn;
        private System.Windows.Forms.PictureBox picAlignTarget;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudBackgroundGap;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox gbNormalisation;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cbxNormalisation;
        private System.Windows.Forms.NumericUpDown nudMultiplier;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown nudDivisor;
        private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.NumericUpDown nudExposureSec;
        private System.Windows.Forms.Label label12;
	}
}