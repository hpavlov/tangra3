namespace Tangra.VideoOperations.LightCurves
{
    partial class ucPreProcessing
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
            this.label8 = new System.Windows.Forms.Label();
            this.nudFrameNo = new System.Windows.Forms.NumericUpDown();
            this.lblMagnX = new System.Windows.Forms.Label();
            this.lblMagn = new System.Windows.Forms.Label();
            this.trackBarWindow = new System.Windows.Forms.TrackBar();
            this.nudStretchingMagnification = new System.Windows.Forms.NumericUpDown();
            this.rbStretchCustom = new System.Windows.Forms.RadioButton();
            this.rbWindow = new System.Windows.Forms.RadioButton();
            this.pnlCustomStretch = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudTo = new System.Windows.Forms.NumericUpDown();
            this.nudFrom = new System.Windows.Forms.NumericUpDown();
            this.gbStretchClipSettings = new System.Windows.Forms.GroupBox();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.rbStretching = new System.Windows.Forms.RadioButton();
            this.rbClipping = new System.Windows.Forms.RadioButton();
            this.rbBrightnessContrast = new System.Windows.Forms.RadioButton();
            this.pnlClipStretch = new System.Windows.Forms.Panel();
            this.pnlBrightnessContrast = new System.Windows.Forms.Panel();
            this.nudContrast = new System.Windows.Forms.NumericUpDown();
            this.nudBrightness = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbContrast = new System.Windows.Forms.TrackBar();
            this.tbBrightness = new System.Windows.Forms.TrackBar();
            this.pnlControlsBrContr = new System.Windows.Forms.Panel();
            this.picHistogram = new System.Windows.Forms.PictureBox();
            this.picStretched = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameNo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStretchingMagnification)).BeginInit();
            this.pnlCustomStretch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrom)).BeginInit();
            this.gbStretchClipSettings.SuspendLayout();
            this.pnlClipStretch.SuspendLayout();
            this.pnlBrightnessContrast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStretched)).BeginInit();
            this.SuspendLayout();
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(4, 83);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Preview Frame No";
            // 
            // nudFrameNo
            // 
            this.nudFrameNo.Enabled = false;
            this.nudFrameNo.Location = new System.Drawing.Point(22, 100);
            this.nudFrameNo.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudFrameNo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFrameNo.Name = "nudFrameNo";
            this.nudFrameNo.Size = new System.Drawing.Size(66, 20);
            this.nudFrameNo.TabIndex = 18;
            this.nudFrameNo.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudFrameNo.ValueChanged += new System.EventHandler(this.nudFrameNo_ValueChanged);
            // 
            // lblMagnX
            // 
            this.lblMagnX.AutoSize = true;
            this.lblMagnX.Location = new System.Drawing.Point(28, 148);
            this.lblMagnX.Name = "lblMagnX";
            this.lblMagnX.Size = new System.Drawing.Size(12, 13);
            this.lblMagnX.TabIndex = 16;
            this.lblMagnX.Text = "x";
            // 
            // lblMagn
            // 
            this.lblMagn.AutoSize = true;
            this.lblMagn.Location = new System.Drawing.Point(15, 126);
            this.lblMagn.Name = "lblMagn";
            this.lblMagn.Size = new System.Drawing.Size(70, 13);
            this.lblMagn.TabIndex = 15;
            this.lblMagn.Text = "Magnification";
            // 
            // trackBarWindow
            // 
            this.trackBarWindow.Enabled = false;
            this.trackBarWindow.LargeChange = 1;
            this.trackBarWindow.Location = new System.Drawing.Point(2, 171);
            this.trackBarWindow.Maximum = 3;
            this.trackBarWindow.Minimum = 1;
            this.trackBarWindow.Name = "trackBarWindow";
            this.trackBarWindow.Size = new System.Drawing.Size(273, 42);
            this.trackBarWindow.TabIndex = 14;
            this.trackBarWindow.Value = 1;
            this.trackBarWindow.ValueChanged += new System.EventHandler(this.trackBarWindow_ValueChanged);
            // 
            // nudStretchingMagnification
            // 
            this.nudStretchingMagnification.Enabled = false;
            this.nudStretchingMagnification.Location = new System.Drawing.Point(42, 142);
            this.nudStretchingMagnification.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudStretchingMagnification.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudStretchingMagnification.Name = "nudStretchingMagnification";
            this.nudStretchingMagnification.Size = new System.Drawing.Size(42, 20);
            this.nudStretchingMagnification.TabIndex = 13;
            this.nudStretchingMagnification.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudStretchingMagnification.ValueChanged += new System.EventHandler(this.nudStretchingMagnification_ValueChanged);
            // 
            // rbStretchCustom
            // 
            this.rbStretchCustom.AutoSize = true;
            this.rbStretchCustom.Location = new System.Drawing.Point(5, 27);
            this.rbStretchCustom.Name = "rbStretchCustom";
            this.rbStretchCustom.Size = new System.Drawing.Size(60, 17);
            this.rbStretchCustom.TabIndex = 21;
            this.rbStretchCustom.Text = "Custom";
            this.rbStretchCustom.UseVisualStyleBackColor = true;
            this.rbStretchCustom.CheckedChanged += new System.EventHandler(this.rgWindowOrCustom_SelectedIndexChanged);
            // 
            // rbWindow
            // 
            this.rbWindow.AutoSize = true;
            this.rbWindow.Checked = true;
            this.rbWindow.Location = new System.Drawing.Point(4, 10);
            this.rbWindow.Name = "rbWindow";
            this.rbWindow.Size = new System.Drawing.Size(64, 17);
            this.rbWindow.TabIndex = 22;
            this.rbWindow.TabStop = true;
            this.rbWindow.Text = "Window";
            this.rbWindow.UseVisualStyleBackColor = true;
            this.rbWindow.CheckedChanged += new System.EventHandler(this.rgWindowOrCustom_SelectedIndexChanged);
            // 
            // pnlCustomStretch
            // 
            this.pnlCustomStretch.Controls.Add(this.label2);
            this.pnlCustomStretch.Controls.Add(this.label1);
            this.pnlCustomStretch.Controls.Add(this.nudTo);
            this.pnlCustomStretch.Controls.Add(this.nudFrom);
            this.pnlCustomStretch.Location = new System.Drawing.Point(4, 165);
            this.pnlCustomStretch.Name = "pnlCustomStretch";
            this.pnlCustomStretch.Size = new System.Drawing.Size(211, 35);
            this.pnlCustomStretch.TabIndex = 23;
            this.pnlCustomStretch.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(131, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "to";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Include from";
            // 
            // nudTo
            // 
            this.nudTo.Location = new System.Drawing.Point(158, 6);
            this.nudTo.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudTo.Name = "nudTo";
            this.nudTo.Size = new System.Drawing.Size(42, 20);
            this.nudTo.TabIndex = 15;
            this.nudTo.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudTo.ValueChanged += new System.EventHandler(this.nudTo_ValueChanged);
            // 
            // nudFrom
            // 
            this.nudFrom.Location = new System.Drawing.Point(83, 6);
            this.nudFrom.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudFrom.Name = "nudFrom";
            this.nudFrom.Size = new System.Drawing.Size(42, 20);
            this.nudFrom.TabIndex = 14;
            this.nudFrom.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudFrom.ValueChanged += new System.EventHandler(this.nudFrom_ValueChanged);
            // 
            // gbStretchClipSettings
            // 
            this.gbStretchClipSettings.Controls.Add(this.rbWindow);
            this.gbStretchClipSettings.Controls.Add(this.rbStretchCustom);
            this.gbStretchClipSettings.Location = new System.Drawing.Point(17, 165);
            this.gbStretchClipSettings.Name = "gbStretchClipSettings";
            this.gbStretchClipSettings.Size = new System.Drawing.Size(75, 48);
            this.gbStretchClipSettings.TabIndex = 24;
            this.gbStretchClipSettings.TabStop = false;
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Checked = true;
            this.rbNone.Location = new System.Drawing.Point(8, 7);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(51, 17);
            this.rbNone.TabIndex = 25;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.rgPreProcessing_SelectedIndexChanged);
            // 
            // rbStretching
            // 
            this.rbStretching.AutoSize = true;
            this.rbStretching.Location = new System.Drawing.Point(8, 22);
            this.rbStretching.Name = "rbStretching";
            this.rbStretching.Size = new System.Drawing.Size(73, 17);
            this.rbStretching.TabIndex = 26;
            this.rbStretching.Text = "Stretching";
            this.rbStretching.UseVisualStyleBackColor = true;
            this.rbStretching.CheckedChanged += new System.EventHandler(this.rgPreProcessing_SelectedIndexChanged);
            // 
            // rbClipping
            // 
            this.rbClipping.AutoSize = true;
            this.rbClipping.Location = new System.Drawing.Point(8, 38);
            this.rbClipping.Name = "rbClipping";
            this.rbClipping.Size = new System.Drawing.Size(62, 17);
            this.rbClipping.TabIndex = 27;
            this.rbClipping.Text = "Clipping";
            this.rbClipping.UseVisualStyleBackColor = true;
            this.rbClipping.CheckedChanged += new System.EventHandler(this.rgPreProcessing_SelectedIndexChanged);
            // 
            // rbBrightnessContrast
            // 
            this.rbBrightnessContrast.AutoSize = true;
            this.rbBrightnessContrast.Location = new System.Drawing.Point(8, 54);
            this.rbBrightnessContrast.Name = "rbBrightnessContrast";
            this.rbBrightnessContrast.Size = new System.Drawing.Size(94, 17);
            this.rbBrightnessContrast.TabIndex = 28;
            this.rbBrightnessContrast.Text = "Brightn./Contr.";
            this.rbBrightnessContrast.UseVisualStyleBackColor = true;
            this.rbBrightnessContrast.CheckedChanged += new System.EventHandler(this.rgPreProcessing_SelectedIndexChanged);
            // 
            // pnlClipStretch
            // 
            this.pnlClipStretch.Controls.Add(this.picHistogram);
            this.pnlClipStretch.Controls.Add(this.picStretched);
            this.pnlClipStretch.Controls.Add(this.pnlCustomStretch);
            this.pnlClipStretch.Controls.Add(this.trackBarWindow);
            this.pnlClipStretch.Location = new System.Drawing.Point(96, -2);
            this.pnlClipStretch.Name = "pnlClipStretch";
            this.pnlClipStretch.Size = new System.Drawing.Size(278, 219);
            this.pnlClipStretch.TabIndex = 29;
            // 
            // pnlBrightnessContrast
            // 
            this.pnlBrightnessContrast.Controls.Add(this.nudContrast);
            this.pnlBrightnessContrast.Controls.Add(this.nudBrightness);
            this.pnlBrightnessContrast.Controls.Add(this.label4);
            this.pnlBrightnessContrast.Controls.Add(this.label3);
            this.pnlBrightnessContrast.Controls.Add(this.tbContrast);
            this.pnlBrightnessContrast.Controls.Add(this.tbBrightness);
            this.pnlBrightnessContrast.Location = new System.Drawing.Point(96, 0);
            this.pnlBrightnessContrast.Name = "pnlBrightnessContrast";
            this.pnlBrightnessContrast.Size = new System.Drawing.Size(278, 219);
            this.pnlBrightnessContrast.TabIndex = 30;
            // 
            // nudContrast
            // 
            this.nudContrast.Location = new System.Drawing.Point(157, 183);
            this.nudContrast.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.nudContrast.Name = "nudContrast";
            this.nudContrast.Size = new System.Drawing.Size(50, 20);
            this.nudContrast.TabIndex = 37;
            this.nudContrast.ValueChanged += new System.EventHandler(this.nudContrast_ValueChanged);
            // 
            // nudBrightness
            // 
            this.nudBrightness.Location = new System.Drawing.Point(157, 79);
            this.nudBrightness.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nudBrightness.Name = "nudBrightness";
            this.nudBrightness.Size = new System.Drawing.Size(50, 20);
            this.nudBrightness.TabIndex = 36;
            this.nudBrightness.Value = new decimal(new int[] {
            128,
            0,
            0,
            0});
            this.nudBrightness.ValueChanged += new System.EventHandler(this.nudBrightness_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(105, 187);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Contrast";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Brightness";
            // 
            // tbContrast
            // 
            this.tbContrast.LargeChange = 16;
            this.tbContrast.Location = new System.Drawing.Point(8, 142);
            this.tbContrast.Maximum = 200;
            this.tbContrast.Name = "tbContrast";
            this.tbContrast.Size = new System.Drawing.Size(262, 42);
            this.tbContrast.TabIndex = 33;
            this.tbContrast.TickFrequency = 10;
            this.tbContrast.Value = 100;
            this.tbContrast.ValueChanged += new System.EventHandler(this.tbContrast_ValueChanged);
            // 
            // tbBrightness
            // 
            this.tbBrightness.LargeChange = 16;
            this.tbBrightness.Location = new System.Drawing.Point(8, 38);
            this.tbBrightness.Maximum = 255;
            this.tbBrightness.Name = "tbBrightness";
            this.tbBrightness.Size = new System.Drawing.Size(262, 42);
            this.tbBrightness.TabIndex = 32;
            this.tbBrightness.TickFrequency = 16;
            this.tbBrightness.Value = 128;
            this.tbBrightness.ValueChanged += new System.EventHandler(this.tbBrightness_ValueChanged);
            // 
            // pnlControlsBrContr
            // 
            this.pnlControlsBrContr.Location = new System.Drawing.Point(4, 126);
            this.pnlControlsBrContr.Name = "pnlControlsBrContr";
            this.pnlControlsBrContr.Size = new System.Drawing.Size(90, 93);
            this.pnlControlsBrContr.TabIndex = 31;
            this.pnlControlsBrContr.Visible = false;
            // 
            // picHistogram
            // 
            this.picHistogram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picHistogram.Enabled = false;
            this.picHistogram.Location = new System.Drawing.Point(14, 7);
            this.picHistogram.Name = "picHistogram";
            this.picHistogram.Size = new System.Drawing.Size(256, 71);
            this.picHistogram.TabIndex = 11;
            this.picHistogram.TabStop = false;
            // 
            // picStretched
            // 
            this.picStretched.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picStretched.Enabled = false;
            this.picStretched.Location = new System.Drawing.Point(14, 84);
            this.picStretched.Name = "picStretched";
            this.picStretched.Size = new System.Drawing.Size(256, 71);
            this.picStretched.TabIndex = 12;
            this.picStretched.TabStop = false;
            // 
            // ucPreProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlControlsBrContr);
            this.Controls.Add(this.pnlBrightnessContrast);
            this.Controls.Add(this.rbBrightnessContrast);
            this.Controls.Add(this.rbClipping);
            this.Controls.Add(this.rbStretching);
            this.Controls.Add(this.rbNone);
            this.Controls.Add(this.gbStretchClipSettings);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nudFrameNo);
            this.Controls.Add(this.lblMagnX);
            this.Controls.Add(this.lblMagn);
            this.Controls.Add(this.nudStretchingMagnification);
            this.Controls.Add(this.pnlClipStretch);
            this.Name = "ucPreProcessing";
            this.Size = new System.Drawing.Size(379, 222);
            ((System.ComponentModel.ISupportInitialize)(this.nudFrameNo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarWindow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStretchingMagnification)).EndInit();
            this.pnlCustomStretch.ResumeLayout(false);
            this.pnlCustomStretch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFrom)).EndInit();
            this.gbStretchClipSettings.ResumeLayout(false);
            this.gbStretchClipSettings.PerformLayout();
            this.pnlClipStretch.ResumeLayout(false);
            this.pnlClipStretch.PerformLayout();
            this.pnlBrightnessContrast.ResumeLayout(false);
            this.pnlBrightnessContrast.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picStretched)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudFrameNo;
        private System.Windows.Forms.Label lblMagnX;
        private System.Windows.Forms.Label lblMagn;
        private System.Windows.Forms.TrackBar trackBarWindow;
        private System.Windows.Forms.NumericUpDown nudStretchingMagnification;
        private System.Windows.Forms.PictureBox picStretched;
        private System.Windows.Forms.PictureBox picHistogram;
        private System.Windows.Forms.RadioButton rbStretchCustom;
        private System.Windows.Forms.RadioButton rbWindow;
        private System.Windows.Forms.Panel pnlCustomStretch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudTo;
        private System.Windows.Forms.NumericUpDown nudFrom;
        private System.Windows.Forms.GroupBox gbStretchClipSettings;
        private System.Windows.Forms.RadioButton rbNone;
        protected internal System.Windows.Forms.RadioButton rbStretching;
        protected internal System.Windows.Forms.RadioButton rbClipping;
        protected internal System.Windows.Forms.RadioButton rbBrightnessContrast;
        private System.Windows.Forms.Panel pnlClipStretch;
        private System.Windows.Forms.Panel pnlBrightnessContrast;
        private System.Windows.Forms.TrackBar tbContrast;
		private System.Windows.Forms.TrackBar tbBrightness;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel pnlControlsBrContr;
        private System.Windows.Forms.NumericUpDown nudContrast;
        private System.Windows.Forms.NumericUpDown nudBrightness;
    }
}
