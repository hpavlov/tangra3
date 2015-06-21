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
            this.cbxUseLowPassFilter = new System.Windows.Forms.CheckBox();
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
            this.ucFrameInterval = new Tangra.VideoOperations.Astrometry.ucFrameInterval();
            this.gbMeasurement.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBackgroundWing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAreaWing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAreas)).BeginInit();
            this.gbxAlignment.SuspendLayout();
            this.gbAlignOn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlignTarget)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMeasurement
            // 
            this.gbMeasurement.Controls.Add(this.cbxUseLowPassFilter);
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
            this.gbMeasurement.Size = new System.Drawing.Size(517, 221);
            this.gbMeasurement.TabIndex = 19;
            this.gbMeasurement.TabStop = false;
            this.gbMeasurement.Text = "Measurements";
            // 
            // cbxUseLowPassFilter
            // 
            this.cbxUseLowPassFilter.AutoSize = true;
            this.cbxUseLowPassFilter.Location = new System.Drawing.Point(242, 77);
            this.cbxUseLowPassFilter.Name = "cbxUseLowPassFilter";
            this.cbxUseLowPassFilter.Size = new System.Drawing.Size(119, 17);
            this.cbxUseLowPassFilter.TabIndex = 38;
            this.cbxUseLowPassFilter.Text = "Use Low Pass Filter";
            this.cbxUseLowPassFilter.UseVisualStyleBackColor = true;
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
            this.label6.Location = new System.Drawing.Point(203, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 34;
            this.label6.Text = "pixels";
            // 
            // nudBackgroundWing
            // 
            this.nudBackgroundWing.Location = new System.Drawing.Point(155, 148);
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
            this.label7.Location = new System.Drawing.Point(29, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Background area wing:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(203, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "pixels";
            // 
            // nudAreaWing
            // 
            this.nudAreaWing.Location = new System.Drawing.Point(155, 122);
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
            this.label2.Location = new System.Drawing.Point(23, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Measurement area wing:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 177);
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
            this.cbxBackgroundMethod.Location = new System.Drawing.Point(155, 174);
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
            this.picAreas.Location = new System.Drawing.Point(12, 243);
            this.picAreas.Name = "picAreas";
            this.picAreas.Size = new System.Drawing.Size(517, 74);
            this.picAreas.TabIndex = 36;
            this.picAreas.TabStop = false;
            this.picAreas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picAreas_MouseDown);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(373, 330);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 18;
            this.btnPrevious.Text = "Cancel";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(454, 330);
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
            this.gbxAlignment.Size = new System.Drawing.Size(517, 221);
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
            this.picAlignTarget.Location = new System.Drawing.Point(304, 8);
            this.picAlignTarget.Name = "picAlignTarget";
            this.picAlignTarget.Size = new System.Drawing.Size(210, 210);
            this.picAlignTarget.TabIndex = 41;
            this.picAlignTarget.TabStop = false;
            // 
            // cbxFineAdjustments
            // 
            this.cbxFineAdjustments.AutoSize = true;
            this.cbxFineAdjustments.Location = new System.Drawing.Point(16, 176);
            this.cbxFineAdjustments.Name = "cbxFineAdjustments";
            this.cbxFineAdjustments.Size = new System.Drawing.Size(128, 17);
            this.cbxFineAdjustments.TabIndex = 37;
            this.cbxFineAdjustments.Text = "Use Fine Adjustments";
            this.cbxFineAdjustments.UseVisualStyleBackColor = true;
            // 
            // ucFrameInterval
            // 
            this.ucFrameInterval.Location = new System.Drawing.Point(16, 57);
            this.ucFrameInterval.Name = "ucFrameInterval";
            this.ucFrameInterval.Size = new System.Drawing.Size(179, 27);
            this.ucFrameInterval.TabIndex = 23;
            this.ucFrameInterval.Value = 1;
            // 
            // frmRunMultiFrameSpectroscopy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 365);
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
            ((System.ComponentModel.ISupportInitialize)(this.nudBackgroundWing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudAreaWing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberMeasurements)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picAreas)).EndInit();
            this.gbxAlignment.ResumeLayout(false);
            this.gbxAlignment.PerformLayout();
            this.gbAlignOn.ResumeLayout(false);
            this.gbAlignOn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAlignTarget)).EndInit();
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
        private System.Windows.Forms.CheckBox cbxUseLowPassFilter;
        private System.Windows.Forms.GroupBox gbxAlignment;
        private System.Windows.Forms.RadioButton rbAlignAbsorptionLine;
        private System.Windows.Forms.RadioButton rbAlignEmissionLine;
        private System.Windows.Forms.RadioButton rbAlignZeroOrder;
        private System.Windows.Forms.CheckBox cbxFineAdjustments;
        private System.Windows.Forms.GroupBox gbAlignOn;
        private System.Windows.Forms.PictureBox picAlignTarget;
	}
}